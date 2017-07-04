function [ ]  = getModelParams(resultPath )
  % maxNumCompThreads(1);
    settings='settings.xls';
    spikingThreshold = 30;
    savePath='something';
    net = spikenet(spikingThreshold,settings,savePath);
    neuronTypes =  net.getNeuronTypes();
    
    types = unique(neuronTypes);  %'D' =0, 'EM'=1, 'ES'=2, 'IM'=3 'IS'=4
    typNums= nan(size(neuronTypes));
    for typeNumber=1:length(types)
       typNums(strcmp(neuronTypes,types(typeNumber))) = typeNumber; 
    end
    neuronTypes = typNums;
    
    saveNeuronTypesSync(resultPath, neuronTypes);
    quit; %Wichtig nur bei test auskommentieren
end

