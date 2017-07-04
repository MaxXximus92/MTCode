function [netWeights, eqParams, esemNum]  = getModelParams( numNeurons, weightPath, equParamsPath )
  % maxNumCompThreads(1);
    settings='settings.xls';
    spikingThreshold = 30;
    savePath='something';
    net = spikenet(numNeurons,spikingThreshold,settings,savePath);
    netWeights= loadWeightsMatrix(weightPath);
    eqParams = loadEquationParams(equParamsPath);
    esemNum =  net.getEsEmNum();
end

