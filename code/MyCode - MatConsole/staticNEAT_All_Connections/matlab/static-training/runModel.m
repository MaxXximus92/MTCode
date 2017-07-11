function [ ] = runModel(name, max_trainTime, savePath, equationParamsPath,connectionsPath,resultPath, syncPath,angles_to_learn,angles_to_simulate,angle_simulation_time )
%todo numNeurons vllt drin lassen und nicht verwenden um C# code nicht zu
%aendern, aber eigentlich egal 2 min aufwand

% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht


equationParams = loadEquationParams(equationParamsPath);


settings='settings.xls';
spikingThreshold = 30;

max_trainTime =str2double(max_trainTime);
angles_to_learn = str2num(angles_to_learn);
angles_to_simulate = str2num(angles_to_simulate);
angle_simulation_time =str2double(angle_simulation_time);
isSave = false;

initial_weights= zeros(size(equationParams,1),size(equationParams,1));
net = spikenet(spikingThreshold,settings,savePath,equationParams,initial_weights);


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
            
            if isSave
                net.save(saveName);
            end
            
            fitness = net.train(saveName, max_trainTime,angles_to_learn,angles_to_simulate,angle_simulation_time,isSave);

            if isSave
                net.save([saveName '_aftsim']);
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



