function [ rmsd ] = runModel(name,numNeurons, runTime,runSettings, savePath, equationParams, weightsMatrix,em_esWeights, isSave )
   % maxNumCompThreads(1)
    % fprintf('set num Threads to 1')
    trials_per_settings=1;
    settings='settings.xls';
    spikingThreshold = 30;
    runSettings= str2num(runSettings);
    net = spikenet(numNeurons,spikingThreshold,settings,savePath,equationParams,weightsMatrix);
    net.setEsEmWeights(em_esWeights);
    rmsd = net.simulate( name, runTime, runSettings, trials_per_settings,isSave,isSave );
    if isSave
    net.save(name);
    end
end 



