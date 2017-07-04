classdef prosthesis < handle
    %PROSTHESIS virtual prosthesis with 1 DOF
    
    properties
        spike_count=0;          %relative spike count (extensor-flexor)
        spike_count_previous=0; %previous relative spike count (extensor-flexor)
        
        angle_min;              %minimum angle in degrees
        angle_max;              %maximum angle in degrees
        angle_target;           %target angle in degrees
        angle_history;          %list of previous angles [timepoint, angle]
        angle_start;            %start angle
        angle_counter;          %count angle changes
        
        fig;                    %axes object to update plot in correct figure
        x_lim;                  %limit for x-axis [lower, upper]
    end
    
    properties(Constant=true)
        PEAKS_PER_ONE_DEGREE = 1;
        PLOT_INSTANTLY = false;
        ENABLE_LEARNING = true;
    end
    
    methods
        function this = prosthesis(angle_start,angle_min,angle_max,target,x_lim)
            this.angle_min = angle_min;
            this.angle_max = angle_max;
            this.angle_target = target;
            this.x_lim = x_lim;
            this.angle_counter = 1;
            
            this.angle_start = angle_start;
            this.angle_history = [0,this.angle_start;
                                  zeros(x_lim(2)/50,2)];
            
            if this.PLOT_INSTANTLY, this.plotProsthesis(); end
        end
        
        % reset changes
        function reset(this)
            this.angle_counter = 1;
            this.angle_history = [0,this.angle_start;
                                  zeros(this.x_lim(2)/50,2)];
            this.spike_count=0;
            this.spike_count_previous=0;
        end
        
        % update arm position, send reward/punish, and plot current state
        function update(this, net, data) % @M data= spiking event
            % send p-cell activity delayed
            if (mod(data.time-25,50) == 0) && (data.time ~= 25)
                net.firePCellsByAngle(this.angle_min,this.angle_max,this.angle_history(this.angle_counter,2));
            end
            
            %sum up spike count in 50ms time window
            if mod(data.time,50) < 50 % <- time-window
                this.spike_count = this.spike_count + (data.exSpikes-data.flSpikes);
            end
            
            % update arm position every 50 ms
            if mod(data.time,50) == 0
                %count angle changes
                this.angle_counter = this.angle_counter + 1;
                
                %calculate new angle and store in history
                next_angle = min(this.angle_max,...
                                 max(this.angle_min,...
                                     this.angle_history(this.angle_counter-1,2)...
                                         + round(this.spike_count_previous/this.PEAKS_PER_ONE_DEGREE)));
                this.angle_history(this.angle_counter,:) = [data.time,next_angle];
                
                %plot next angle if instant plotting is enabled
                if this.PLOT_INSTANTLY, this.plotNextAngle(); end
                
                % reward/punish
                if this.ENABLE_LEARNING
                    %mean target distance of previous angles
                    previous_angles = this.angle_history(max(1,this.angle_counter-1):this.angle_counter-1,2);
                    previous_distance = abs(mean(previous_angles-this.angle_target));
                    %target distance of current angle
                    new_distance = abs(this.angle_history(this.angle_counter,2)-this.angle_target);
                  
                    %M cause of missing noice a non moving state of the
                    %network can occur and this state is unleaveable.
                    %->punish neurons when the network is not moving but
                    %only if it is not the target angle

                  if previous_distance ~= new_distance
                  %if this.angle_history(this.angle_counter,2) ~= this.angle_target;
                        net.reward(previous_distance > new_distance); %@M warum ist der reward nicht auch um 25 ms verzögert?
                  end
                end
                
                % backup spike_counts (to send spike counts 50ms later)
                this.spike_count_previous = this.spike_count;
                this.spike_count = 0;
            end
        end
        
        % create figure object for prosthesis simulation and plot target
        % angle (black line)
        function plotProsthesis(this)
            this.fig = figure('Name','Prosthesis Simulation','NumberTitle','off','visible','on');
            xlim(this.x_lim);
            box on;
            ylim([this.angle_min,this.angle_max]);
            xlabel('Time [ms]');
            ylabel('Angle [°]');
            title('Prosthesis Simulation')
            drawnow;
        end
        
        % plot next angle at timepoint time
        function plotNextAngle(this, time)
            %draw new position to plot
            set(0,'CurrentFigure',this.fig);
            hold on;
            line([time-50,time], [this.angle_history(this.angle_counter-1,2),this.angle_history(this.angle_counter,2)]);
            line([time-50,time], [this.angle_target,this.angle_target],'Color','red');
            drawnow;
            hold off;
        end
    end
end

