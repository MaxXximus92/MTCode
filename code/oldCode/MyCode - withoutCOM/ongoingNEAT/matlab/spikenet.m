classdef spikenet < handle
    %SPIKENET spiking neuronal network
    %   input: total number of neurons
    events
      nextTimeStep;
    end
    properties
        numNeurons;         %total number of neuron
        initWeights;        %initial type-to-type weights
        rmpValues;          %resting membrane potentials for all neuronTypes (0=don't reset)
        neuronRatios;       %ratios of neuron types
        spikingThreshold;   %spiking threshold [mV]
        neuronTypes;        %lookup list to get the neuron type by its index
        probabilities;      %neuron-to-neuron connection probabilities
        weightsMatrix;      %neuron-to-neuron connection weights (also indicates the presence of a connection (>0)) i,j = post zu pre
        es_em;              %indizes of synapses that can be eligibility tagged: ES -> EM
        eligibility;        %one entry for each ES->EM synapse, values < 0: waiting for post-synaptic peak; values > 0: eligbility tagged
                            % n*3 matrix (idxrow,idxcolumofWeightsM, value)
                            %               pre, post
        weightsScale;       %weights scale matrix, for reward / punishment
        noiseWeights;       %noise weights one column with noise weights
        noiseWeightsScale;  %noise weights scale (one column type for each cell)
        noiseRevPotential;  %noise reversal potential 1*265
        connections;        %list of all neuron-to-neuron connections
        exProprioceptive;   %indizes of proprioceptive cells: (extensor)
        flProprioceptive;   %indizes of proprioceptive cells: (flexor)
        exMotoric;          %indizes of exciatory motor cells (extensor)
        flMotoric;          %indizes of exciatory motor cells (flexor)
        v;                  %membrane potential
        
        emCells;            %indices of emCells
        esCells;            %indecies of esCells
        prost;              %current prosthesis
       
        %same for all trials! in constrat to sbastians Model
        aParams;             %a param of voltage eqation
        bParams;             %b param of voltage eqation
        dParams;             %d param of voltage eqation
        
        savePath;           %path where to safe results
        
        randomNoiceSeedCounter =0; % used as seed for Random Numbers, so that every instance of this model gets the same "random numbers"
        randomPopCodeSeedCounter=0;
    end
    
    properties(Constant = true)
        % debug flag
        DEBUG = false;
        
        % Maximum weights scale factor
        MAX_WEIGHTS_SCALE = 5.0;
    end
    
    methods

        %% constructor

        function this = spikenet(numNeurons,spikingThreshold,settingsFile,savePath,equationParams, weightsMatrix)
           % second constructor provding euationparams and weightsmarti
            if(nargin>4)
                this.weightsMatrix  = weightsMatrix;
                this.aParams= equationParams(:,1);
                this.bParams= equationParams(:,2);
                this.dParams= equationParams(:,3);
            end    
            
            if (nargin > 0)
                
                 if(savePath(end) ~= filesep)
                     savePath=[savePath filesep];
                 end
                this.savePath         =savePath;
                this.numNeurons       = numNeurons;
                this.spikingThreshold = spikingThreshold;
                
                this.loadSettingsFile(settingsFile);
                         
                this.weightsMatrix   = this.getWeightMatrix();
                this.weightsScale    = ones(this.numNeurons,this.numNeurons); %ones
                this.weightsScale(this.weightsMatrix > 0) = 1.0; %@M setze alle sca les zu denen es ein Gewicht gibt auf 1
                
                this.connections     = find(this.weightsMatrix > 0);%@M find returns all indices fullfilling the condition 
                %connections = single indices 1-... first column than second
                this.emCells = find(this.getCellsOfType('EM'));
                this.esCells = find(this.getCellsOfType('ES'));
                
                this.setEligibility();
                
                this = this.splitExtensorFlexorEqually();
                

                equparams=this.getEquationParameters();
                 this.aParams=equparams(:,1);
                 this.bParams=equparams(:,2);
                 this.dParams=equparams(:,3);
            end
        end
        
        function setEligibility(this)
            % Get ES_EM connections in a way that's not dependend
            % on the weight
            helpM = zeros(this.numNeurons,this.numNeurons);
            helpM(this.esCells,this.emCells) = this.weightsMatrix(this.esCells,this.emCells);
            this.es_em            = find(helpM >0);
            % this.es_em           = find(this.weightsMatrix == this.initWeights('ES_EM'));
            
            [rowIdx, colIdx]     = ind2sub(size(this.weightsMatrix),this.es_em);
            this.eligibility     = [rowIdx, colIdx, zeros(length(colIdx),1)];
        end
        
        %% C# Communication
        function save(this,name)
            net= this;
            save([this.savePath name],'net');
        end
        
        function [esCells,emCells] = getEsEmCells(this)
                  esCells=this.emCells;
                  emCells=this.esCells;
        end
        
        function numEsEm = getEsEmNum(this)
                  numEsEm(1)= length(this.esCells);
                  numEsEm(2)= length(this.emCells);
        end
        
        
        function setEsEmWeights(this, weights)
                 [esl,eml]= size(weights);
                 if esl== length(this.esCells) && eml== length(this.emCells)
                 this.weightsMatrix(this.esCells,this.emCells)=weights;  
                 this.connections  = find(this.weightsMatrix > 0);
                 this.setEligibility();
                 else
                 error('EsEnWeightmatrix size is (%4.2f,%4.2f) but should be (%4.2f,%4.2f)',esl,eml,length(this.esCells),length(this.emCells));    
                 end    
        end
        %connection indicated by a value 1 no connction by 0
        function setEsEmConnections(this,connectionsMatrix)
                 connectionsMatrix= connectionsMatrix.*this.initWeights('ES_EM');  
                 this.setEsEmWeights(connectionsMatrix);
        end
        function esEmWeights = getEsEmWeights(this)
                 esEmWeights = this.weightsMatrix(this.esCells,this.emCells);
        end
        


        

        
 
            
        
        %% Simulation
        % changed to return mean of rmsds
        function rmsd = simulate( this, model_name, run_time, run_settings, trials_per_settings,save_trail_figures)
%             if nargin < 6
%                 save_mean_figures = false;
%             end
            if nargin < 5
                save_trail_figures = false;
            end
            rmsds = zeros(1,size(run_settings,2));

            % run all settings
            for run_number=1:size(run_settings,2)
                movements = zeros(run_time/50+1,trials_per_settings+1);
                movements(:,1)=0:50:run_time;

                %% Construct prosthesis
                prost = prosthesis(run_settings(1,run_number),0,135,run_settings(2,run_number),[0,run_time]);
                this.prost=prost;
                % function this = prosthesis(angle_start,angle_min,angle_max,target,x_lim)
                % add listener to handle prosthesis movements
                lh = addlistener(this,'nextTimeStep',@prost.update);

                for trial_number=1:trials_per_settings
                    if (this.DEBUG) fprintf(strcat('##########################\n',...
                                   'Model %s - Run %.0f - Trial %.0f\n',...
                                   '##########################\n'),model_name,run_number,trial_number);
                    end
                    %% Run trial
                    output = this.run(run_time);
                    movements(:,trial_number+1)=prost.angle_history(:,2);

                    %% Plot Data
                    if save_trail_figures
                        figure('PaperSize',[25,20],'Units','centimeters','PaperPosition',[0 0 25 20],'visible','off');
                        this.plotAllFirings(output);
                        box on;
                        set(gca,'fontsize',10);
                        saveas(gcf,[this.savePath sprintf('Model %s - Run %.0f - Trial %.0f - firings (start %.0f - target %.0f).pdf',model_name,run_number,trial_number,run_settings(1,run_number),run_settings(2,run_number))], 'pdf');

                        figure('Name','Prosthesis Simulation','NumberTitle','off','PaperSize',[25,10],'Units','centimeters','PaperPosition',[0 0 25 10],'visible','off');
                        plot(prost.angle_history(:,1)/1000,prost.angle_history(:,2));
                        xlim(prost.x_lim/1000);
                        ylim([prost.angle_min,prost.angle_max]);
                        xlabel('Time [s]');
                        ylabel('Angle [�]');
                        title('Prosthesis Simulation')
                        set(gca,'fontsize',10);
                        xrange=prost.x_lim/1000;
                        line(xrange,[run_settings(2,run_number), run_settings(2,run_number)],'Color','green','LineStyle','--','LineWidth',2);
                        saveas(gcf,[this.savePath sprintf('Model %s - Run %.0f - Trial %.0f - movement (start %.0f - target %.0f).pdf',model_name,run_number,trial_number,run_settings(1,run_number),run_settings(2,run_number))], 'pdf');
                        close all hidden
                    end

                    %reset for next trial
                    this.reset();
                    %prost.reset();
                end

                %% remove listener
                delete(lh);

                %% Save mean movement for all trials
%                 if save_mean_figures
%                     figure('Name','Prosthesis Simulation','NumberTitle','off','PaperSize',[25,10],'Units','centimeters','PaperPosition',[0 0 25 10],'visible','off');
%                     plot(movements(:,1)/1000,movements(:,2:end),'Color','blue');
%                     hold on;
%                     plot(movements(:,1)/1000,mean(movements(:,2:end),2),'Color','red','LineWidth',2);
%                     xlim(prost.x_lim/1000);
%                     ylim([prost.angle_min,prost.angle_max]);
%                     box on;
%                     xlabel('Time [s]');
%                     ylabel('Angle [�]');
%                     title('Prosthesis Simulation')
%                     set(gca,'fontsize',10);
%                     line(prost.x_lim/1000,[prost.angle_target, prost.angle_target],'Color','green','LineStyle','--','LineWidth',2);
%                     saveas(gcf,[this.savePath sprintf('Model %s - Run %.0f - mean movement (start %.0f - target %.0f).pdf',model_name,run_number,run_settings(1,run_number),run_settings(2,run_number))], 'pdf');
%                 end
                %% calculate average RMSD
                 %rmsds(run_number) = mean(sqrt(mean((prost.angle_target - movements(movements(:,1)>=20000,2:end)).^2)));  %error 20s <t<120s
                    rmsds(run_number) = mean(sqrt(mean((prost.angle_target - movements(movements(:,1)>=run_time*0.16,2:end)).^2)));
                    rmsd = mean(rmsds);
            end
        end
        
        % reset weights scale and elgigibilty for next run
          function reset(this)
               this.prost.reset();
               this.weightsScale(this.es_em) = 1.0;
               this.eligibility(:,3) = 0;
               this.randomNoiceSeedCounter =0;
               this.randomPopCodeSeedCounter=0;     
          end
        
        % perform run with duration of time [ms]
        function [time_neuron, t] = run(this, time)
            if (this.DEBUG), fprintf('Start simulation...'); end
            time_neuron = zeros(time*this.numNeurons, 2);
            num_firings = 0;
            a= this.aParams;
            b= this.bParams;
            d= this.dParams;
            this.v = this.getInitVoltage();
            u = b.*this.v;
            % start run in 1 ms time-steps
            for t=1:time
                if mod(t,100) == 0
                if (this.DEBUG) fprintf('time ellapsed %d of %d \n',t,time); end
                end    
                % check for spikes
                fired = find(this.v >= this.spikingThreshold);
                % decrease eligibility time-window
                this.eligibility(this.eligibility(:,3) < 0, 3) = this.eligibility(this.eligibility(:,3) < 0, 3) + 1;
                this.eligibility(this.eligibility(:,3) > 0, 3) = this.eligibility(this.eligibility(:,3) > 0, 3) - 1;
                % check for presynaptic peaks, and eligibility-tag synapses
                % for 100 ms
                %@M post-pre
                this.eligibility(this.getPrePostEligibility('tag',fired),3) = 100; %@M 100 ms kommt mir schon viel vor..
                % set potential eligibility time-window to 100ms %@M remember fireing for next tag search pre-> post
                this.eligibility(this.getPrePostEligibility('potential',fired),3) = -100;
                
                % count extensor and flexor spikes
                exSpikes = sum(this.v(this.exMotoric) >= this.spikingThreshold);
                flSpikes = sum(this.v(this.flMotoric) >= this.spikingThreshold);
                % notify about exciatory motor spikes
                this.v(this.getCellsOfType('P')) = this.resetMembranePotential(this.getCellsOfType('P')); %@M ? warum
                notify(this,'nextTimeStep',spikingEvent(t,exSpikes,flSpikes));
                %@M firing P cells get v=spikingthreshold, later on v is
                %changed one step... thats not good. Might also decrease
                %the value. ?? warum? ok -> siehe method firePCellsByAngle
                
                
                % add spikes at time t to output. 
                if (~isempty(fired))
                    time_neuron(num_firings+1:num_firings+length(fired),: ) = [t+0*fired,fired];
                    num_firings = num_firings+length(fired);
                end
                
                % reset membrane potential of fired neurons
                this.v(fired) = this.resetMembranePotential(fired);
                
                % update u and v
                u(fired)=u(fired)+d(fired);
                % @max  hier fehlt doch der v input der presynaptischen
                % neuronen oder? ne quatsch ist ja alles oder nichts
                % hier... keine aktivierung wie in herk�mmlichen ANN
                synapticInput = sum((this.weightsMatrix(fired,:).*this.weightsScale(fired,:)),1)';
                noise = this.getRandomNoise(this.v); %noise removed to
                %ensure dame values for each run
                this.v = this.v+0.5*(0.04*this.v.^2+5*this.v+140-u+synapticInput+noise); % 2* halbsekunden schritt
                this.v = this.v+0.5*(0.04*this.v.^2+5*this.v+140-u+synapticInput+noise);
                u=u+a.*(b.*this.v-u);
            end
            % skip empty entries
            time_neuron = time_neuron(1:num_firings, :);
            if (this.DEBUG), fprintf('Done.\n'); end
        end
        
        % get indizes of (pre-)postsynapses which can be (potentially) 
        % eligibility-tagged
        function idx = getPrePostEligibility(this, type, fired)
            idx = zeros(length(this.eligibility),1);
            idx_counter = 1;
            
            for i=1:length(fired)
                if strcmp(type,'tag')
                    % neuron i is post-synaptic: search for pre-synaptic
                    % spike (values < 0)
                    foundIdx = find(this.eligibility(:,2)==fired(i) & this.eligibility(:,3) < 0);%@M ist eligibility nicht eine n*3 matrix?
                                            %@M :,2 = Presynapsen :,1 = Postsynapsen 
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
        
        % send reward to eligibility-tagged synapses (raward=true), else
        % send punishment (reward=false)
        function reward(this, reward)
            linearIdx = sub2ind(size(this.weightsScale), this.eligibility(this.eligibility(:,3) > 0, 1),this.eligibility(this.eligibility(:,3) > 0, 2));
            %M coordinate to linear index
            eligWeights = this.eligibility(this.eligibility(:,3) > 0,3)./20; %M entspricht we in der MT p42
            if reward % reward
                this.weightsScale(linearIdx) = ...
                    min(this.MAX_WEIGHTS_SCALE, this.weightsScale(linearIdx) ...
                    + (1-this.weightsScale(linearIdx) ./ this.MAX_WEIGHTS_SCALE) .* eligWeights);
            else % punishment
                this.weightsScale(linearIdx) = ...
                    max(0, this.weightsScale(linearIdx) ...
                    - (this.weightsScale(linearIdx) ./ this.MAX_WEIGHTS_SCALE) .* eligWeights);
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
        
        % get random noise -> but same noise for all experiments
        function noise = getRandomNoise(this, voltage)
            % todo save runsetting*runtime random generated interger.
            % Generated with same seed.  Use this seed consecuent to
            % produce randn.  Easier... just take a inreasing counter.

            
            noise = zeros(this.numNeurons, 1);
            rng(this.randomNoiceSeedCounter);
            ra = randn(this.numNeurons, 1);
            idx = (0.5+0.5*ra) < 0.24; %approx 300Hz  %M naja eher 400
            noise(idx) = noise(idx) ... //@M noise +  gives no sense 0 anyway
                + (this.noiseWeights(idx,1) .* (1 - voltage(idx) ./ this.noiseRevPotential(idx,1)));
                    %@M -> MT Sebastian p. 40   ruhepotential der zelle
                    % warum wird nur noise f�r EM zellen verwendet?
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
            end
        end   

            
        
        % get vector of length this.numNeurons, indicating the neuron types
        function neuronTypes = getNeuronTypes(this)
            if (~isempty(this.neuronTypes))
                neuronTypes = this.neuronTypes;
            else
                types = keys(this.neuronRatios);
                neuronTypes = cell(1,this.numNeurons);
                current = 0;
                for i=1:length(types)
                    prev = current + 1;  
                    current = current + round(this.numNeurons*this.neuronRatios(types{i}));
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
        function idxs = getCellsOfType(this, type) %@M returns ones at fitting position
            idxs = strcmp(this.neuronTypes, type);
        end
        
        % load settings file (weights, etc.)
        function loadSettingsFile(this, filename)
            % load synapses weights
            [weights, types] = xlsread(char(filename), 'Connection Weights', '', 'basic'); %@M file ,sheet,range, basic import mode
            [row, col] = ind2sub(size(weights), find(weights~=0)); %@M find(weights) returns only one dimension ind2sub returns position in weights matrix
            this.initWeights = containers.Map;
            for idx=1:length(row)
                this.initWeights(strcat(char(types(row(idx)+1,1)),'_', char(types(1,col(idx)+1)))) = weights(row(idx),col(idx));
                %@M key="TypA_TypB"
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
            
            % load ratios of celltypes => sum(neuronRatios) = 1
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
        
        % split both proprioceptiv and exciatory motoric cells equally into
        % two groups (extensor and flexor)
        function this = splitExtensorFlexorEqually(this)
            pCells = find(this.getCellsOfType('P'));
            emCells = find(this.getCellsOfType('EM'));
            this.exProprioceptive = pCells(1:round(end/2));
            this.flProprioceptive = pCells(round(end/2)+1:end);
            this.exMotoric = emCells(1:round(end/2));
            this.flMotoric = emCells(round(end/2)+1:end);
        end
                
        % fire pCells
        function pCellsToFire = firePCellsByAngle (this, angle_min, angle_max, angle)
            angle_norm = angle/(angle_max-angle_min);
            
            pCells = find(this.getCellsOfType('P'))';
            numPCells = length(pCells);
            muPCells = (0:0.5:((numPCells-1)*0.5))';
            % annahme dass zellen 0.5 auseinander liegen
            probsPCells = 2*normpdf(angle_norm*(numPCells-1)*0.5, muPCells, 0.8);%-> max bei 0.5 deshalb *2
            %@M gausverteilungen werden verschoben -> identisch zu einem gaus
            %und die werte der zellen drum rum ablesen.
             %@M population coding -> how much influence has an angle for each p cell
            rng(this.randomPopCodeSeedCounter); 
            pCellsToFire = pCells.*(rand(length(probsPCells),1) <= probsPCells);
                                    %@M numPcells*1 array
            pCellsToFire = pCellsToFire(pCellsToFire~=0);
            this.v(pCellsToFire) = this.spikingThreshold;
            this.randomPopCodeSeedCounter = this.randomPopCodeSeedCounter+1;
         %   static pop coding 
         %   aktivate all in radius around angle mean
%             windowsize= 5; % look at norm distribution with sigma =0.8 and 0.5 neuron distance
%             angle_norm = angle/(angle_max-angle_min);
%             pCells = find(this.getCellsOfType('P'))';
%             numPCells = length(pCells);
%             centerPcell = round(angle_norm*(numPCells-1));
%             pCellsToFire=(centerPcell-windowsize:1:centerPcell+windowsize);
%             pCellsToFire= pCellsToFire(pCellsToFire>0 & pCellsToFire <=numPCells);
%             pCellsToFire = pCellsToFire+ pCells(1)-1;
%             this.v(pCellsToFire) = this.spikingThreshold;
            
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

