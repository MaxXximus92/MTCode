classdef spikenet < handle
    %SPIKENET spiking neuronal network
    %   input: spiking threshold [mV] and settings-file (.xls)
    events
      update;
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
    end
    
    properties(Constant = true)
        DEBUG = true;               % debug flag
        MAX_WEIGHTS_SCALE = 5.0;    % maximum weights scale factor
        ELIG_WINDOW = 100;          % eligibility trace time-window
    end
    
    methods
        % constructor
        function this = spikenet()
        end
        
        % train model
        function successfully = simulate(this, model_number, angle_start, angles_to_simulate, time_per_angle)
            %% construct prosthesis
            prost = prosthesis(0,135,angle_start,angles_to_simulate,time_per_angle);
            lh1 = addlistener(this,'update',@prost.update);

            %% run trial
            fprintf(strcat('#########\nModel %.0f\n#########\n'),model_number);
            [output, time] = this.run(time_per_angle*length(angles_to_simulate)); this.break_run=0;
            delete(lh1);

            %% plot data
            fprintf('save plots...\n');
            figure('PaperSize',[25,20],'Units','centimeters','PaperPosition',[0 0 25 20],'visible','off');
            this.plotAllFirings(output);
            xlim([0,time]);
            box on;
            saveas(gcf,sprintf('Model %03.0f (firings).pdf',model_number), 'pdf');
            set(0,'CurrentFigure',prost.fig);
            xlim([0,time]);
            box on;
            saveas(gcf,sprintf('Model %03.0f (movement).pdf',model_number), 'pdf');

            %% return result
            successfully = prost.reached_angles;
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
        
        % perform run with duration of time [ms]
        function [time_neuron, t] = run(this, time)
            time_neuron = zeros(time*this.numNeurons, 2);
            num_firings = 0;
            [a,b,d] = this.getEquationParameters();
            this.v = this.getInitVoltage();
            u = b.*this.v;
            % start run in 1 ms time-steps
            for t=1:time
                % interrupt if flag = true
                if this.break_run, break; end
                % check for spikes
                fired = find(this.v >= this.spikingThreshold);
                
                % count extensor and flexor spikes
                exSpikes = sum(this.v(this.exMotoric) >= this.spikingThreshold);
                flSpikes = sum(this.v(this.flMotoric) >= this.spikingThreshold);
                
                % notify prosthesis about EM spikes
                notify(this,'update',spikingEvent(t,exSpikes,flSpikes));
                
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
        
        % get equation parameters (see Izhikevich, 2003)
        function [a,b,d] = getEquationParameters(this)
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
            idx = (0.5+0.5*randn(this.numNeurons, 1)) < 0.24; %approx 300Hz per neuron
            noise(idx) = noise(idx) ...
                + (this.noiseWeights(idx,1) .* (1 - voltage(idx) ./ this.noiseRevPotential(idx,1)));
            noise(isnan(noise)) = 0;
        end
        
        % get weight matrix (or generate initial one)
        function weightsMatrix = getWeightMatrix(this)
            if (~isempty(this.weightsMatrix))
                weightsMatrix = this.weightsMatrix;
            else
                tic;
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
                toc;
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
        
        % excitatory motoric cells are split equally into two groups 
        % (extensor and flexor)
        function this = splitExtensorFlexorEqually(this)
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
            
            dCellsToFire = dCells.*(rand(length(probsDCells),1) <= probsDCells);
            dCellsToFire = dCellsToFire(dCellsToFire~=0);
            stimulateVoltage = abs(this.spikingThreshold)+abs(this.rmpValues('D'));
            this.v(dCellsToFire) = this.rmpValues('D')+0.5*stimulateVoltage;
        end
        
        % fire D cells by the pre-calculated distance
        function dCellsToFire = fireDCellsByDifference (this, angle_target, angle_current, angle_min, angle_max)
            angle_diff = angle_target - angle_current + angle_max;
            angle_norm = angle_diff/(2*angle_max-angle_min);
            
            dCells = find(this.getCellsOfType('D'))';
            numDCells = length(dCells);
            muDCells = (0:0.5:((numDCells-1)*0.5))';
            probsACells = 2*normpdf(angle_norm*(numDCells-1)*0.5, muDCells, 0.8);
            
            dCellsToFire = dCells.*(rand(length(probsACells),1) <= probsACells);
            dCellsToFire = dCellsToFire(dCellsToFire~=0);
            this.v(dCellsToFire) = this.spikingThreshold;
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