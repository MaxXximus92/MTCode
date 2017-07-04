function [ ]  = getModelParams( numNeurons,resultPath )
  % maxNumCompThreads(1);
    settings='settings.xls';
    spikingThreshold = 30;
    numNeurons =str2double(numNeurons);
    savePath='something';
    net = spikenet(spikingThreshold,settings,savePath);
    esemNum =  net.getEsEmNum();
    saveSync(resultPath, esemNum,'esemNum');
end

