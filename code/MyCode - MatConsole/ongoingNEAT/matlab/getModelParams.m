function [ ]  = getModelParams( numNeurons,resultPath )
  % maxNumCompThreads(1);
    settings='settings.xls';
    spikingThreshold = 30;
    numNeurons =str2double(numNeurons);
    savePath='something';
    net = spikenet(numNeurons,spikingThreshold,settings,savePath);
    esemNum =  net.getEsEmNum();
    saveEsEmNumSync(resultPath, esemNum);
    quit;
end

