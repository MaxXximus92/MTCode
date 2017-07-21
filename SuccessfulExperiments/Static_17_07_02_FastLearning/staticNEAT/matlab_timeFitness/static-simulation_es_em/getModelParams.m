function [ ]  = getModelParams(resultPath )
  % maxNumCompThreads(1);
    settings='settings.xls';
    spikingThreshold = 30;
    savePath='something';
    net = spikenet(spikingThreshold,settings,savePath);
    EsEmNum =  net.getEsEmNum();
    saveDEsNumSync(resultPath, EsEmNum);
    quit; %Wichtig nur bei test auskommentieren
end

