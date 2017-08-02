classdef (ConstructOnLoad) spikingEvent < event.EventData
    %PROSTHESIS Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        time;
        flSpikes;
        exSpikes;
    end
    
    methods
        function this = spikingEvent(time,exSpikes,flSpikes)
            this.time = time;
            this.flSpikes = flSpikes;
            this.exSpikes = exSpikes;
        end
    end
    
end

