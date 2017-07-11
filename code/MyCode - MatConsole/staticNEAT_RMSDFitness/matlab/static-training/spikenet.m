classdef spikenet < handle
    %SPIKENET spiking neuronal network
    %   input: spiking threshold [mV] and settings-file (.xls)
    events
      learn;
    end
    properties
        numNeurons;         %total number of neuron
        initWeights;        %initial type-to-type weights
        rmpValues;          %resting membrane potentials for all neuronTypes (0=don't reset)
        neuronRatios;       %ratios of neuron types
        spikingThreshold;   %spiking threshold [mV]
        neuronTypes;        %lookup list to get the neuron type by its index
        probabilities;      %neuron-to-neuron connection probabilities
        weightsMatrix;      %neuron-to-neuron connection weights (also indicates the presence of a connection (>0))
        d_es;               %indizes of D -> ES synapses
        es_em;              %indizes of ES -> EM synapses
        eligibility;        %one entry for each D->ES synapse, values < 0: waiting for post-synaptic peak; values > 0: eligbility tagged
        weightsScale;       %weights scale matrix, for reward / punishment
        noiseWeights;       %noise weights (one row for each noise type)
        noiseWeightsScale;  %noise weights scale (one row for each noise type)
        noiseRevPotential;  %noise reversal potential (one row for each noise type)
        connections;        %list of all neuron-to-neuron connections
        exMotoric;          %indizes of exciatory motor cells (extensor)
        flMotoric;          %indizes of exciatory motor cells (flexor)
        v;                  %membrane potential
        break_run;          %interupt flag
        new_connections;    %counter of reconnected synapses
      
        correctMoves;       %correct moves into the direction fo the target angle
        
        aParams;             %a param of voltage eqation
        bParams;             %b param of voltage eqation
        dParams;             %d param of voltage eqation
        
        savePath;           %path where to safe results
        randomNoiceSeedCounter =0; % used as seed for Random Numbers, so that every instance of this model gets the same "random numbers"
        randomPopCodeSeedCounter=0;
    end
    
    properties(Constant = true)
        DEBUG = false;               % debug flag
        MAX_WEIGHTS_SCALE = 5.0;    % maximum weights scale factor
        ELIG_WINDOW = 100;          % eligibility trace time-window
    end
    
    methods
        % constructor  // TODO maybe num neurons hinzufuegen je nach zuk.
        % experimenten
        function this = spikenet(spikingThreshold,settingsFile,savePath,equationParams, weightsMatrix)
            if(nargin>3)
                this.weightsMatrix  = weightsMatrix;
                this.aParams= equationParams(:,1);
                this.bParams= equationParams(:,2);
                this.dParams= equationParams(:,3);
            end 
            
            if (nargin > 0)
                 if(savePath(end) ~= filesep)
                     savePath=[savePath filesep];
                 end
                this.savePath         = savePath;
                this.spikingThreshold = spikingThreshold;
                
                this.loadSettingsFile(settingsFile);
                this.new_connections = 0;
                         
                this.weightsMatrix   = this.getWeightMatrix();
                this.weightsScale    = ones(this.numNeurons,this.numNeurons);% @m was zeros, but this way one doesn't has to reset it when a weight change occurs
                this.weightsScale(this.weightsMatrix > 0) = 1.0;
                
                this.refreshConnections();
                
                this.splitExtensorFlexorEqually();
                
                 equparams=this.getEquationParameters();
                 this.aParams=equparams(:,1);
                 this.bParams=equparams(:,2);
                 this.dParams=equparams(:,3);
            end
        end
        
        %% c# Communication
        function save(this,name)
            net= this;
            save([this.savePath name '.mat'],'net');
        end        
        
        
        function numdEs = getDEsNum(this)

                  numdEs(1)= sum(this.getCellsOfType('D'));% add logical ones
                  numdEs(2)= sum(this.getCellsOfType('ES'));
        end
        
        function setDEsWeights(this, weights)
            [dl,esl]= size(weights);
            dCells= this.getCellsOfType('D');
            esCells=this.getCellsOfType('ES');
            
            if dl== sum(dCells) && esl== sum(esCells)
                this.weightsMatrix(dCells,esCells)=weights;
                this.refreshConnections();
                this.setEligibilitySynapses(this.d_es);
            else
                error('EsEnWeightmatrix size is (%4.2f,%4.2f) but should be (%4.2f,%4.2f)',sl,esl,length(this.esCells),length(this.emCells));
            end
        end
        
                %connection indicated by a value 1 no connction by 0
        function setDEsConnections(this,connectionsMatrix)
                 connectionsMatrix= connectionsMatrix.*this.initWeights('D_ES');  
                 this.setDEsWeights(connectionsMatrix);
        end
        
        
       function DEsWeights = getDEsWeights(this)
            dCells= this.getCellsOfType('D');
            esCells=this.getCellsOfType('ES');      
           DEsWeights =  this.weightsMatrix(dCells,esCells);
       end
        
               % reset weights scale and elgigibilty for next run
          function reset(this)
              % prost gets reinizialized every train
               this.weightsScale(this.d_es) = 1.0;
               this.eligibility(:,3) = 0;
               this.randomNoiceSeedCounter =0;
               this.randomPopCodeSeedCounter=0;
               this.correctMoves = 0;
          end
        
        %% train model
        function fitness = train(this, model_name, max_train_time,angles_to_learn,angles_to_simulate,angle_simulation_time,save_figures)
            if nargin < 3
                save_figures = false;
            end
            
            this.correctMoves = 0;
            %% construct prosthesis
            prost = prosthesis(0,135,max_train_time,angles_to_learn,angles_to_simulate,angle_simulation_time,save_figures); %@ m ok wichtig!!!! nicht 180°
            lh1 = addlistener(this,'learn',@prost.learn);
            
            %% run trial
            if(this.DEBUG), fprintf(strcat('#########\nModel %s \n#########\n'),model_name);end
            max_run_time = max_train_time+length(prost.angles_to_simulate)*prost.angle_simulation_time;
            [output, time] = this.run(max_run_time,prost); this.break_run=0;
            delete(lh1);
           
            
            %% plot data
            if(save_figures)
                if(this.DEBUG),fprintf('save plots...\n');end;
                figure('PaperSize',[25,20],'Units','centimeters','PaperPosition',[0 0 25 20],'visible','off');
                this.plotAllFirings(output);
                xlim([0,time]);
                box on;
                saveas(gcf,[this.savePath sprintf('%s (firings).pdf',model_name)], 'pdf');
                set(0,'CurrentFigure',prost.fig);
                xlim([0,time]);
                box on;
                saveas(gcf,[this.savePath sprintf('%s (movement).pdf',model_name)], 'pdf');
                close all hidden
            end
            % calc fitness
            % fitness_training -> normalized amount of steps in right
            % direction
            % fitness_angle -> reached angles
            % fitness_variance -> varaince to simulation target
            % fintess = (fitness_training+ fitness_angle+ fitness_time)/3

            trainingTime =prost.mode_start_time;
            if (trainingTime == 1), trainingTime = max_train_time; end
            fitness_correctTrainingMoves = this.correctMoves/(trainingTime/50); %/50 since reward is send only every 50 steps
            fitness_reachedAngles = sum(prost.reached_angles)/length(prost.reached_angles);
            successfully = sum(prost.reached_angles)==length(prost.reached_angles); % reached angles during simulation
            fitness_SimRmsd=0; % = not successful

            if(successfully)
                angle_start_time=prost.mode_start_time;
                squared_sum=0;
                num_angles=0;
                ahistory= prost.angle_history();
                for angle_target= prost.angles_to_simulate
                   maxt=angle_start_time+prost.angle_simulation_time;
                   angles=ahistory(ahistory(:,1)>=angle_start_time & ahistory(:,1)<maxt,2);
                   angle_start_time =maxt;
                   squared_sum= squared_sum+(sum((angles-angle_target).^2));
                   num_angles = num_angles + length(angles);
                end  
                %num_angles2 = length(prost.angles_to_simulate)*prost.angle_simulation_time/50;
                rmsd= sqrt(squared_sum/num_angles);
                norm_rmsd = rmsd/(prost.angle_max-prost.angle_min);
                %calc variance
                fitness_SimRmsd = 1-norm_rmsd; % wert zwischen 0 und 1. 0 schlecht 1 gut
            end
            
            rmsd_scale=0.7;
            angle_scale=0.2;
            training_scale =0.1;
            fitness = (training_scale*fitness_correctTrainingMoves+angle_scale*fitness_reachedAngles+rmsd_scale*fitness_SimRmsd);
            this.reset();
        end
        
        % get indizes of pre->post synapses
        function connections = getConnections(this,pre,post)
            pre_cells = find(this.getCellsOfType(pre));
            post_cells = find(this.getCellsOfType(post));
            submatrix = this.weightsMatrix(pre_cells,post_cells);
            [i,j] = ind2sub(size(submatrix),find(submatrix ~= 0));
            i = i+pre_cells(1)-1;
            j = j+post_cells(1)-1;
            connections=sub2ind(size(this.weightsMatrix), i, j);
        end
        
        % set synapses which should be rewarded/punished
        function setEligibilitySynapses(this, connectionIdxs)
            if isempty(connectionIdxs)
                this.eligibility = [];
            else
                [rowIdx, colIdx] = ind2sub(size(this.weightsMatrix),connectionIdxs);
                this.eligibility     = [rowIdx, colIdx, zeros(length(colIdx),1)];
            end
        end
        
        % perform run with duration of time [ms]
        function [time_neuron, t] = run(this, time,prost)
            time_neuron = zeros(time*this.numNeurons, 2);
            num_firings = 0;
            a= this.aParams;
            b= this.bParams;
            d= this.dParams;
            this.v = this.getInitVoltage();
            u = b.*this.v;
            % start run in 1 ms time-steps
            for t=1:time
                % interrupt if flag = true
                if this.break_run, break; end
                % check for spikes
                fired = find(this.v >= this.spikingThreshold);
                % decrease eligibility time-window
                if ~isempty(this.eligibility)
                    this.eligibility(this.eligibility(:,3) < 0, 3) = this.eligibility(this.eligibility(:,3) < 0, 3) + 1;
                    this.eligibility(this.eligibility(:,3) > 0, 3) = this.eligibility(this.eligibility(:,3) > 0, 3) - 1;
                    % check for presynaptic peaks, and eligibility-tag synapses
                    this.eligibility(this.getPrePostEligibility('tag',fired),3) = this.ELIG_WINDOW;
                    % set potential eligibility time-window to 100ms
                    this.eligibility(this.getPrePostEligibility('potential',fired),3) = -1 * this.ELIG_WINDOW;
                end
                
                % count extensor and flexor spikes
                exSpikes = sum(this.v(this.exMotoric) >= this.spikingThreshold);
                flSpikes = sum(this.v(this.flMotoric) >= this.spikingThreshold);
                
                % notify prosthesis about EM spikes
                %notify(this,'learn',spikingEvent(t,exSpikes,flSpikes));
                prost.learn(this,spikingEvent(t,exSpikes,flSpikes))
                % add spikes at time t to output. 
                if (~isempty(fired))
                    time_neuron(num_firings+1:num_firings+length(fired), :) = [t+0*fired,fired];
                    num_firings = num_firings+length(fired);
                end
                
                % reset membrane potential of fired neurons
                this.v(fired) = this.resetMembranePotential(fired);
                
                % update u and v
                u(fired)=u(fired)+d(fired);
                synapticInput = sum((this.weightsMatrix(fired,:).*this.weightsScale(fired,:)),1)';
                noise = this.getRandomNoise(this.v);
                this.v = this.v+0.5*(0.04*this.v.^2+5*this.v+140-u+synapticInput+noise);
                this.v = this.v+0.5*(0.04*this.v.^2+5*this.v+140-u+synapticInput+noise);
                u=u+a.*(b.*this.v-u);
            end
            % skip empty entries
            time_neuron = time_neuron(1:num_firings, :);
            if (this.DEBUG), fprintf('Done.\n'); end
        end
        
        % reconnect all synapses with weight-scale-factors below 0.2
        %da Zufall... entfernen
%         function formNewConnections(this, pre, post)
%             tag = strcat(pre,'_',post);
%             if ~isKey(this.initWeights, tag), return; end
%             currentConnections = this.getConnections(pre,post);
%             disconnect = currentConnections(this.weightsScale(currentConnections) <= 0.2);
%             if isempty(disconnect), return; end
%             this.weightsScale(disconnect) = 0;
%             this.weightsMatrix(disconnect) = 0;
%             [preIds, postIds] = ind2sub(size(this.weightsMatrix),disconnect);
%             for i=1:length(preIds)
%                 eligRowIdx = find(ismember(this.eligibility(:,1:2),[preIds(i),postIds(i)]),1);
%                 while true
%                     newPostId = randsample(find(this.getCellsOfType(post)),1);
%                     if this.weightsMatrix(preIds(i),newPostId) == 0, break; end
%                 end
%                 this.weightsMatrix(preIds(i),newPostId) = this.initWeights(tag);
%                 this.weightsScale(preIds(i),newPostId) = 1.0;
%                 this.eligibility(eligRowIdx,2:3) = [newPostId,0];
%             end
%             this.refreshConnections();
%             this.new_connections = this.new_connections + length(preIds);
%         end
        
        % refresh indizes of synapses
        function refreshConnections(this)
            this.connections     = find(this.weightsMatrix ~= 0);
            this.es_em           = this.getConnections('ES','EM');
            this.d_es            = this.getConnections('D','ES');
        end
        
        % get indizes of (pre-)postsynapses which can be (potentially) 
        % eligibility-tagged
        function idx = getPrePostEligibility(this, type, fired)
            idx = zeros(length(this.eligibility),1);
            idx_counter = 1;
            if ~isempty(this.eligibility)
                for i=1:length(fired)
                    if strcmp(type,'tag')
                        % neuron i is post-synaptic: search for pre-synaptic
                        % spike (values < 0)
                        foundIdx = find(this.eligibility(:,2)==fired(i) & this.eligibility(:,3) < 0);
                    else
                        % neuron i is pre-synaptic: get all outgoing
                        % connections, which are not already eligibility-tagged
                        foundIdx = find(this.eligibility(:,1)==fired(i) & this.eligibility(:,3) <= 0);
                    end
                    idx(idx_counter:idx_counter+length(foundIdx)-1) = foundIdx;
                    idx_counter = idx_counter+length(foundIdx);
                end
                idx = unique(idx(idx ~= 0));
            end
        end
        
        % send reward to eligibility-tagged synapses (raward=true), else
        % send punishment (reward=false)
        function reward(this, reward)
            if ~isempty(this.eligibility)
                linearIdx = sub2ind(size(this.weightsScale), this.eligibility(this.eligibility(:,3) > 0, 1),this.eligibility(this.eligibility(:,3) > 0, 2));
                eligWeights = this.eligibility(this.eligibility(:,3) > 0,3)./20; %dynamic eligibility weight
                if reward
                    this.correctMoves = this.correctMoves+1;
                    this.weightsScale(linearIdx) = ...
                        min(this.MAX_WEIGHTS_SCALE, this.weightsScale(linearIdx) ...
                        + (1-this.weightsScale(linearIdx) ./ this.MAX_WEIGHTS_SCALE) .* eligWeights);
                else % punishment
                    this.weightsScale(linearIdx) = ...
                        max(0, this.weightsScale(linearIdx) ...
                        - (this.weightsScale(linearIdx) ./ this.MAX_WEIGHTS_SCALE) .* eligWeights);
                end
            end
        end
        
        % get equation parameters (see Izhikevich, 2003)
        function equationParams = getEquationParameters(this)
            if (~isempty(this.aParams) && ~isempty(this.bParams)&&  ~isempty(this.dParams))
                equationParams=[this.aParams,this.bParams,this.dParams];
            else
                a=zeros(this.numNeurons,1);
                b=zeros(this.numNeurons,1);
                d=zeros(this.numNeurons,1);
                
                rnd = rand(this.numNeurons,1);
                
                inhibitory = logical(this.getCellsOfType('IS') + this.getCellsOfType('IM'));
                exciatory = logical(this.getCellsOfType('ES') + this.getCellsOfType('EM'));
                a(inhibitory) = 0.02+0.08.*rnd(inhibitory);
                a(exciatory) = 0.02;
                b(inhibitory) = 0.25-0.05.*rnd(inhibitory);
                b(exciatory) = 0.2;
                d(inhibitory) = 2;
                d(exciatory) = 8-6.*rnd(exciatory).^2;
                equationParams=[a,b,d];
            end
        end
        
        % reset voltage to resting membrane potential
        function voltage = resetMembranePotential(this, idx)
            types = this.neuronTypes(idx);
            voltage = zeros(length(types), 1);
            uniqueTypes = unique(types);
            for idx=1:length(uniqueTypes)
                voltage(strcmp(types, uniqueTypes(idx))) = this.rmpValues(char(uniqueTypes(idx)));
            end
        end
        
        % get random noise 

        function noise = getRandomNoise(this, voltage)
            noise = zeros(this.numNeurons, 1);
            rng(this.randomNoiceSeedCounter);
            idx = (0.5+0.5*randn(this.numNeurons, 1)) < 0.24; %approx 300Hz per neuron
            noise(idx) = noise(idx) ...
                + (this.noiseWeights(idx,1) .* (1 - voltage(idx) ./ this.noiseRevPotential(idx,1)));
            noise(isnan(noise)) = 0;
            this.randomNoiceSeedCounter = this.randomNoiceSeedCounter+1;
        end
        
        % get weight matrix (or generate initial one)
        function weightsMatrix = getWeightMatrix(this)
            if (~isempty(this.weightsMatrix))
                weightsMatrix = this.weightsMatrix;
            else
                
                if (this.DEBUG), fprintf('Generate weights matrix...'); end
                weightsMatrix = zeros(this.numNeurons,this.numNeurons);
                types = unique(this.neuronTypes);
                for pre=1:length(types)
                    for post=1:length(types)
                        connection = strcat(char(types(pre)),'_',char(types(post)));
                        if ~isKey(this.initWeights, connection)
                            continue
                        end
                        idx_pre = strcmp(types(pre),this.neuronTypes);
                        idx_post = strcmp(types(post),this.neuronTypes);
                        probs = rand(length(find(idx_pre==1)),length(find(idx_post==1)));
                        weightsMatrix(idx_pre,idx_post) = (probs<this.probabilities(connection)).*this.initWeights(connection);
                    end
                end
                if (this.DEBUG), fprintf('Done.\n'); end
               
            end
        end
        
        % get vector of length this.numNeurons, indicating the neuron types
        function neuronTypes = getNeuronTypes(this)
            if (~isempty(this.neuronTypes))
                neuronTypes = this.neuronTypes;
            else
                types = keys(this.neuronRatios);
                this.numNeurons = sum(cell2mat(values(this.neuronRatios)));
                neuronTypes = cell(1,this.numNeurons);
                current = 0;
                for i=1:length(types)
                    prev = current + 1;
                    current = current + round(this.neuronRatios(types{i}));
                    if (i == length(types))
                        neuronTypes(prev:end) = types(i);
                    else
                        neuronTypes(prev:current) = types(i);
                    end
                end
            end
        end
       
        % get membrane potential of all neurons (or generate initial
        % voltages)
        function voltage = getInitVoltage(this)
            voltage = this.resetMembranePotential(ones(this.numNeurons, 1));
        end
        
        % find cells of type and return indices
        function idxs = getCellsOfType(this, type)
            idxs = strcmp(this.neuronTypes, type);
        end
        
        % load settings file (weights, etc.)
        function loadSettingsFile(this, filename)
            % load synaptic weights
            [weights, types] = xlsread(char(filename), 'Connection Weights', '', 'basic');
            [row, col] = ind2sub(size(weights), find(weights~=0));
            this.initWeights = containers.Map;
            for idx=1:length(row)
                this.initWeights(strcat(char(types(row(idx)+1,1)),'_', char(types(1,col(idx)+1)))) = weights(row(idx),col(idx));
            end
            
            % load connection probabilities
            [data, types] = xlsread(char(filename), 'Connection Probabilities', '', 'basic');
            [row, col] = ind2sub(size(data), find(data~=0));
            this.probabilities = containers.Map;
            for idx=1:length(row)
                this.probabilities(strcat(char(types(row(idx)+1,1)),'_', char(types(1,col(idx)+1)))) = data(row(idx),col(idx));
            end
            
            % load resting potentials
            [rmp, types] = xlsread(char(filename), 'Resting Potentials', '', 'basic');
            this.rmpValues = containers.Map;
            for idx=1:length(rmp)
                this.rmpValues(char(types(idx))) = rmp(idx);
            end
            
            % load numbers of different celltypes
            [ratios, types] = xlsread(char(filename), 'Neuron Ratios', '', 'basic');
            this.neuronRatios = containers.Map;
            for idx=1:length(ratios)
                this.neuronRatios(char(types(idx))) = ratios(idx);
            end
            
            % load neuron types
            this.neuronTypes = this.getNeuronTypes();
            
            % load noise weights
            [weights, types] = xlsread(char(filename), 'Noise Weights', '', 'basic');
            uniqueTypes = types(2:end,1);
            this.noiseWeights = zeros(this.numNeurons, length(uniqueTypes));
            for noiseType=1:size(weights,1)
                for cellType=1:size(weights,2)
                    this.noiseWeights(this.getCellsOfType(types(1,cellType+1)), strcmp(uniqueTypes,char(types(noiseType+1,1)))) = ...
                        weights(noiseType,cellType);
                end
            end
            this.noiseWeightsScale = double(this.noiseWeights > 0);
            
            % load noise reversal potential
            [revpot, types] = xlsread(char(filename), 'Noise Reversal Potential', '', 'basic');
            uniqueTypes = types(2:end,1);
            this.noiseRevPotential = zeros(this.numNeurons, length(uniqueTypes));
            for noiseType=1:size(revpot,1)
                for cellType=1:size(revpot,2)
                    this.noiseRevPotential(this.getCellsOfType(types(1,cellType+1)), strcmp(uniqueTypes,char(types(noiseType+1,1)))) = ...
                        revpot(noiseType,cellType);
                end
            end
        end
        
        % excitatory motoric cells are split equally into two groups 
        % (extensor and flexor)
        function splitExtensorFlexorEqually(this)
            emCells = find(this.getCellsOfType('EM'));
            this.exMotoric = emCells(1:round(end/2));
            this.flMotoric = emCells(round(end/2)+1:end);
        end
        
        % stimulate D cells by target angle with each interval
        function dCellsToFire = stimulateDCellsByTarget (this, time, interval, angle_target, angle_min, angle_max)
            angle_norm = angle_target/(angle_max-angle_min);
            fireTime = round((interval-1)*angle_norm);
            if time == fireTime
                dCellsToFire = find(this.getCellsOfType('D'));
                stimulateVoltage = abs(this.spikingThreshold)+abs(this.rmpValues('D'));
                this.v(dCellsToFire) = this.v(dCellsToFire)+0.5*stimulateVoltage;
            end
        end
        
        % stimulate D cells by current angle
        function dCellsToFire = stimulateDCellsByCurrent (this, time, interval, angle_current, angle_min, angle_max)
            time_norm=time/interval;
            angle_to_fire = round(angle_max-angle_current+time_norm*(angle_max-angle_min));
            dCellsToFire = this.stimulateDCellsByAngle(angle_to_fire, angle_min, angle_max);
        end
        
        % helper function: stimulate D cells by current angle
        function dCellsToFire = stimulateDCellsByAngle (this, angle_target, angle_min, angle_max)
            angle_norm = angle_target/(2*(angle_max-angle_min));
            
            dCells = find(this.getCellsOfType('D'))';
            numDCells = length(dCells);
            muDCells = (0:0.5:((numDCells-1)*0.5))';
            probsDCells = 2*normpdf(angle_norm*(numDCells-1)*0.5, muDCells, 0.8);
            
            rng(this.randomPopCodeSeedCounter); 
            dCellsToFire = dCells.*(rand(length(probsDCells),1) <= probsDCells);
            dCellsToFire = dCellsToFire(dCellsToFire~=0);
            stimulateVoltage = abs(this.spikingThreshold)+abs(this.rmpValues('D'));
            this.v(dCellsToFire) = this.rmpValues('D')+0.5*stimulateVoltage;
            this.randomPopCodeSeedCounter = this.randomPopCodeSeedCounter+1;
        end
        
        % fire D cells by the pre-calculated distance
        function dCellsToFire = fireDCellsByDifference (this, angle_target, angle_current, angle_min, angle_max)
            angle_diff = angle_target - angle_current + angle_max;
            angle_norm = angle_diff/(2*angle_max-angle_min);
            
            dCells = find(this.getCellsOfType('D'))';
            numDCells = length(dCells);
            muDCells = (0:0.5:((numDCells-1)*0.5))';
            probsACells = 2*normpdf(angle_norm*(numDCells-1)*0.5, muDCells, 0.8);
            rng(this.randomPopCodeSeedCounter); 
            dCellsToFire = dCells.*(rand(length(probsACells),1) <= probsACells);
            dCellsToFire = dCellsToFire(dCellsToFire~=0);
            this.v(dCellsToFire) = this.spikingThreshold;
            this.randomPopCodeSeedCounter = this.randomPopCodeSeedCounter+1;
        end
        
        % plot firings of all neurons over the whole simulation time
        function plotAllFirings(this, firings)
            this.plotFirings(firings, [min(firings(:,1)), max(firings(:,1))]);
        end
        
        % plot firings of all neurons between timelim = [lower upper]
        function plotFirings(this, firings, timelim)
            filter = firings(:,1) >= timelim(1) & firings(:,1) <= timelim(2);
            time = firings(filter,1);
            neurons = firings(filter,2);
            types = unique(this.neuronTypes);
            boarders = zeros(length(types),1);
            minX = timelim(1);
            maxX = timelim(2);
            ylim([1,this.numNeurons]);
            cmap = hsv(length(types)+5);
            for idx=1:length(types)
                hold on;
                idx_lower = find(this.getCellsOfType(types(idx)),1);
                idx_upper = find(this.getCellsOfType(types(idx)),1,'last');
                idx_between = neurons>=idx_lower & neurons<=idx_upper;
                scatter(time(idx_between),neurons(idx_between),3,cmap(idx,:),'p');
                boarders(idx) = round(mean([idx_lower, idx_upper]));
                if idx~=length(types)
                    line([minX maxX], [idx_upper+0.5 idx_upper+0.5],'Color','black');
                end
            end
            set(gca,'YTick',boarders,'YTicklabel',types);
            xlabel('Time [ms]');
            ylabel('Neuron Type');
            hold off;
        end
    end
    
end

