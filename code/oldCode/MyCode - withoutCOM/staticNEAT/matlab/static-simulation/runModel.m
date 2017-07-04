function [ ] = runModel(name,numNeurons, runTime,runSettings, savePath, equationParamsPath, weightsMatrixPath,esEmConnectionsPath,resultPath, syncPath )
% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht

weightsMatrix = loadWeightsMatrix(weightsMatrixPath); % vllt v5 einstellen
equationParams = loadEquationParams(equationParamsPath);

trials_per_settings=1;
settings='settings.xls';
spikingThreshold = 30;
runSettings= str2num(runSettings);
numNeurons =str2double(numNeurons);
runTime =str2double(runTime);
% todo read from file
%isSave =str2num(isSave);
isSave = false;

net = spikenet(numNeurons,spikingThreshold,settings,savePath,equationParams,weightsMatrix);

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
            
            esEmConnectionsStruct = loadSync(esEmConnectionsPath);
            esEmConnections=esEmConnectionsStruct.esemConnections;
            
            net.setEsEmConnections(esEmConnections);
            
            rmsd = net.simulate(saveName, runTime, runSettings, trials_per_settings,isSave);
            
            if isSave
                net.save(saveName);
            end
            saveSync(resultPath,rmsd,'rmsd')
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



