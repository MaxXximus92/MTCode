function [ ] = runModel(name, max_trainTime, savePath, equationParamsPath, weightsMatrixPath,DEsConnectionsPath,resultPath, syncPath )
%todo numNeurons vllt drin lassen und nicht verwenden um C# code nicht zu
%aendern, aber eigentlich egal 2 min aufwand

% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht

weightsMatrix = loadWeightsMatrix(weightsMatrixPath); % vllt v5 einstellen
equationParams = loadEquationParams(equationParamsPath);


settings='settings.xls';
spikingThreshold = 30;

max_trainTime =str2double(max_trainTime);

isSave = false;


net = spikenet(spikingThreshold,settings,savePath,equationParams,weightsMatrix);

while (true)
    sync = readSync(syncPath); %% matlab ready -> finished  matlab-> working c# -> simulate  c# close 
    sync = strtrim(strread(sync, '%s', 'delimiter', sprintf('\n')));
    if(~isempty(sync))
        if(strcmp(sync{1},'simulate') || strcmp(sync{1},'simulate_plot'))
            writeSync(syncPath,'working');
            saveName= name;
            if(strcmp(sync{1},'simulate_plot'))
                saveName = sync{2};
                isSave= true;
            end
          
            DEsConnectionsStruct = loadSync(DEsConnectionsPath);
            DEsConnections=DEsConnectionsStruct.dEsConnections;

            net.setDEsConnections(DEsConnections);


            fitness = net.train(saveName, max_trainTime,isSave);


            % mems = memory;
             %mem_used = mems.MemUsedMATLAB;
             %mem_available = mems.MemAvailableAllArrays;
             %fprintf(' \n memory used: %d \n memory available %d \n difference %d \n',(mem_used),(mem_available),mem_used-lastmems.MemUsedMATLAB )
            % lastmems =mems;
            
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



