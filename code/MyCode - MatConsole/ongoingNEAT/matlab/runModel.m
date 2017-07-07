function [ ] = runModel(name,numNeurons, runTime,runSettings, savePath, equationParamsPath, weightsMatrixPath,esEmConnectionsPath,resultPath, syncPath )
% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht

weightsMatrix = loadWeightsMatrix(weightsMatrixPath); % vllt v5 einstellen
equationParams = loadEquationParams(equationParamsPath);


settings='settings.xls';
spikingThreshold = 30;
runSettings= str2num(runSettings);
numNeurons =str2double(numNeurons);
runTime =str2double(runTime);
% todo read from file
%isSave =str2num(isSave);
isSave = false;
%logfile= [syncPath(1:end-4) '_log.txt'];
simcount=0;

net = spikenet(numNeurons,spikingThreshold,settings,savePath,equationParams,weightsMatrix);

while (true)
    
    sync = readSync(syncPath); %% matlab ready -> finished  matlab-> working%% c# -> simulate  %% c# close close
    sync = strtrim(strread(sync, '%s', 'delimiter', sprintf('\n')));
    if(~isempty(sync))
        if(strcmp(sync{1},'simulate') || strcmp(sync{1},'simulate_plot'))
            simcount= simcount+1;
            writeSync(syncPath,'working','w');
            saveName= name;
            if(strcmp(sync{1},'simulate_plot'))
                saveName = sync{2};
                isSave= true;
            end
            
            esEmConnectionsStruct = loadSync(esEmConnectionsPath);
            esEmConnections=esEmConnectionsStruct.esemConnections;
            
            net.setEsEmConnections(esEmConnections);
            
            if isSave
                net.save(saveName);
            end
            
            % tic
            rmsd = net.simulate(saveName, runTime, runSettings,isSave);
            % toc 16.2445, 16.2445
            %a=memory; %1670
            % a.MemUsedMATLAB
            
            saveRmsdSync(resultPath,rmsd)
            isSave= false;
            %  writeSync(syncPath,'finished','w');
            
            % mems = memory;
            % mem_used = mems.MemUsedMATLAB;
            % mem_available = mems.MemAvailableAllArrays;
            %message = sprintf(' iteration: %d \n used: %d \n available %d \n',(simcount),(mem_used),(mem_available))
            %writeSync(logfile,message,'a');
            
        elseif(strcmp(sync,'close'))
            delete(syncPath)
            pause(0.1);
            break;
        end
    else
        pause(0.1);
    end
    
end
quit;
end



