classdef prosthesis < handle
    %PROSTHESIS virtual prosthesis with 1 DOF
    %   input: minimum angle and maximum angle
    
    properties
        spike_count=0;          %relative spike count (extensor-flexor)
        spike_count_previous=0; %previous relative spike count (extensor-flexor)
        
        angle_min;              %minimum angle in degrees
        angle_max;              %maximum angle in degrees
        angle_target;           %target angle in degrees
        angle_history;          %list of previous angles [timepoint, angle]
        angle_start;            %start angle
        angles_to_simulate;     %list of target angles used for simulation-phase
        time_per_angle;         %simulation-duration for each angle (in ms)
        reached_angles;         %memorize reached target angles
        
        mode_start_time;        %memorize start-time of learning/simulation phase
        
        fig;                    %axes object to update plot in correct figure
    end
    
    properties(Constant=true)
        DEBUG = true;               %debug flag
        PEAKS_PER_ONE_DEGREE = 1;   %number of EM spikes causing 1 degree of angle change
        PLOT_INSTANTLY = false;     %plot will be updated instantly during simulation (increases runtime!)
        DRAW_PLOT = true;           %draw plot
    end
    
    methods
        function this = prosthesis(angle_min,angle_max,angle_start,angles_to_simulate,time_per_angle)
            this.angle_min = angle_min;
            this.angle_max = angle_max;
            this.angle_target = angle_min;
            
            this.angles_to_simulate = angles_to_simulate;
            this.time_per_angle = time_per_angle;
            
            this.reached_angles = zeros(1,length(this.angles_to_simulate));
            
            this.mode_start_time=1;
            
            this.angle_start = angle_start;
            this.angle_history = [0,this.angle_start];
            
            if this.DRAW_PLOT, this.plotProsthesis(); end
        end
        
        % update arm position and plot current state
        function update(this, net, data)
            %disable learning and start test movement
            if data.time == this.mode_start_time;
                if this.DEBUG, fprintf('Start simulation...\n'); end
            end
            this.changeTargetAngleOverTime(data,net,[((0:length(this.angles_to_simulate))*this.time_per_angle)',[this.angles_to_simulate,-1]']);
                        
            %stimulate difference cells by current and target angle
            if mod(data.time,50) == 0, net.v(net.getCellsOfType('D')) = net.rmpValues('D'); end
            net.stimulateDCellsByCurrent(mod(data.time,50), 50, this.angle_history(end,2), this.angle_min, this.angle_max);
            net.stimulateDCellsByTarget(mod(data.time,50), 50, this.angle_target, this.angle_min, this.angle_max);
            dCells = net.v(net.getCellsOfType('D'));
            dCells(dCells<net.spikingThreshold) = net.rmpValues('D');
            net.v(net.getCellsOfType('D'))=dCells;
            
            %sum up spike count in 50ms time window
            if mod(data.time,50) < 50 % <- time-window size
                this.spike_count = this.spike_count + (data.exSpikes-data.flSpikes);
            end
            
            % update arm position every 50 ms
            if mod(data.time,50) == 0                
                % calculate new angle and store in history
                next_angle = min(this.angle_max,...
                                 max(this.angle_min,...
                                     this.angle_history(end,2)...
                                         + round(this.spike_count_previous/this.PEAKS_PER_ONE_DEGREE)));
                this.angle_history = [this.angle_history;data.time,next_angle];
                if this.isTargetReached()
                    this.reached_angles(this.angles_to_simulate==this.angle_target)=1;
                end
                % plot next angle
                if this.DRAW_PLOT, this.plotNextAngle(data.time); end
                
                % backup spike_counts (to send spike counts 50ms later)
                this.spike_count_previous = this.spike_count;
                this.spike_count = 0;
            end
        end
        
        % automatically change the target-angle, pre-defined within the path
        function changeTargetAngleOverTime(this, data, net, path)
            time = data.time - this.mode_start_time - 1;
            angle = path(path(:,1)==time,2);
            if angle == -1, net.break_run = true; return; end
            if isempty(angle), return; end
            if this.DEBUG, fprintf('New target angle: %.0f\n',angle); end
            this.angle_target = angle;
        end
        
        % check whether target has been reached or not
        function target_reached = isTargetReached(this)
            prev_angle = this.angle_history(end-1,2);
            new_angle = this.angle_history(end,2);
            target_reached = ...
                (prev_angle <= this.angle_target && this.angle_target <= new_angle) ...
                || (new_angle <= this.angle_target && this.angle_target <= prev_angle) ...
                || this.angle_target == new_angle;
        end
        
        % create figure object for prosthesis simulation and plot target angle
        function plotProsthesis(this)
            if this.PLOT_INSTANTLY, figvisible = 'on';
            else figvisible = 'off'; end
            this.fig = figure('Name','Prosthesis Simulation','NumberTitle','off','PaperSize',[25,10],'Units','centimeters','PaperPosition',[0 0 25 10],'visible',figvisible);
            ylim([this.angle_min,this.angle_max]);
            xlabel('Time [ms]');
            ylabel('Angle [°]');
            title('Prosthesis Simulation')
            if this.PLOT_INSTANTLY, drawnow; end
        end
        
        % plot next angle at timepoint time
        function plotNextAngle(this, time)
            %draw new position to plot
            set(0,'CurrentFigure',this.fig);
            hold on;
            line([time-50,time], [this.angle_history(end-1,2),this.angle_history(end,2)]);
            line([time-50,time], [this.angle_target,this.angle_target],'Color','green','LineStyle','--','LineWidth',2);
            if this.PLOT_INSTANTLY, drawnow; end
            hold off;
        end
    end
end