function [ ]  = getModelParams(resultPath )
  % maxNumCompThreads(1);
    settings='settings.xls';
    spikingThreshold = 30;
    savePath='something';
    net = spikenet(spikingThreshold,settings,savePath);
    dEsNum =  net.getDEsNum();
    saveDEsNumSync(resultPath, dEsNum);
   % quit; %Wichtig nur bei test auskommentieren
end

