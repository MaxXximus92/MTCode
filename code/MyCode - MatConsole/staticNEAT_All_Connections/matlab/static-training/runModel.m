function [ ] = runModel(name, max_runTime, savePath, equationParamsPath,connectionsPath,resultPath, syncPath )
%todo numNeurons vllt drin lassen und nicht verwenden um C# code nicht zu
%ändern, aber eigentlich egal 2 min aufwand

% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht


equationParams = loadEquationParams(equationParamsPath);
eql= size(equationParams);
weightsMatrix = zeros(eql(1),eql(1));

settings='settings.xls';
spikingThreshold = 30;

max_runTime =str2double(max_runTime);
% todo read from file
%isSave =str2num(isSave);
isSave = false;

net = spikenet(spikingThreshold,settings,savePath,equationParams,weightsMatrix);

while (true)
    sync = readSync(syncPath); %% matlab ready -> finished  matlab-> working%% c# -> simulate  %% c# close close
    sync = strtrim(strread(sync, '%s', 'delimiter', sprintf('\n')));
    if(~isempty(sync))
        if(strcmp(sync{1},'simulate') || strcmp(sync{1},'simulate_plot'))
            writeSync(syncPath,'working');
            saveName= name;
            if(strcmp(sync{1},'simulate_plot'))
                saveName = sync{2};
                isSave= true;
            end
            
            connectionsStruct = loadSync(connectionsPath);
            connections=connectionsStruct.connections;
            
            net.setConnections(connections);
            
            fitness = net.train(saveName, max_runTime,isSave);
            if isSave
                net.save(saveName);
            end
            saveFitnessSync(resultPath,fitness)
            isSave= false;
            writeSync(syncPath,'finished');
        elseif(strcmp(sync,'close'))
            delete(syncPath)
            break;
        end
    else
         pause(0.1);
    end
end
quit;
end



