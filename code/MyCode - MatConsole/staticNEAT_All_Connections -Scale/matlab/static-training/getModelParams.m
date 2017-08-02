function [ numNeu ]  = getModelParams(resultPath,scaleFactors ) % 5 entries, one for each celltype
  % maxNumCompThreads(1);
    scaleFactors =str2num(scaleFactors);
    settings='settings.xls';
    spikingThreshold = 30;
    savePath='something';
    net = spikenet(spikingThreshold,settings,savePath,scaleFactors);
    neuronTypes =  net.getNeuronTypes();
    types = unique(neuronTypes);  %'D' =0, 'EM'=1, 'ES'=2, 'IM'=3 'IS'=4
    typNums= nan(size(neuronTypes));
    for typeNumber=1:length(types)
       typNums(strcmp(neuronTypes,types(typeNumber))) = typeNumber-1; 
    end
    neuronTypes = typNums;
    numNeu= size(neuronTypes,2);
    saveNeuronTypesSync(resultPath, neuronTypes);
    quit; %Wichtig nur bei test auskommentieren
end

