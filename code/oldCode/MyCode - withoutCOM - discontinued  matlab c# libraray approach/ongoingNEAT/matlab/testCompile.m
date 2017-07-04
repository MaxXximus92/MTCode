function [ rmsds ] = testCompile()
%TESTCOMPILE Summary of this function goes here
%   Detailed explanation goes here
num_models = 1;%100;
run_time = 40000; %runtime per trial [ms] %initial 120000
run_settings = [65;35]%[ones(1,5).*65;     %start angles
%0,35,75,105,135]; %target angles
trials_per_settings = 1;
numNeurons=2;
%% Start simulation
for model_number=1:num_models
    %Construct network
    savePath='C:\Users\Maximus Mutschler\Downloads\test';
    weightsM = ones(numNeurons,numNeurons);
    equationParams = [ones(numNeurons,1),ones(numNeurons,1).*2,ones(numNeurons,1).*3];
    %net = spikenet(256,30,'settings.xls',savePath,equationParams,weightsM);
    net = spikenet(numNeurons,30,'settings.xls',savePath,equationParams,weightsM);
    % net.save('net');
    
    %!matlab -r !! new matlab instance
    % !powershell -Command "& {dir REGISTRY::HKEY_CLASSES_ROOT\CLSID -include PROGID -recurse | foreach {$_.GetValue(""""")} }"
    
    % a=net.getEsEmNum();
    %b= net.getEsEmCells();
    %esEmWeights = net.getEsEmWeights();
    %esEmWeights = ones(size(esEmWeights)); % just for testing
    %net.setEsEmWeights(esEmWeights);
    
    %net.setEsEmConnections(esEmWeights);
    % esEmWeights = net.getEsEmWeights();
    name= 'OngoingExperiment_BestGen_Generation_0_Fitness_158';
    tic
    rmsds = net.simulate( 'OngoingExperiment_BestGen_Generation_0_Fitness_158', run_time, run_settings, trials_per_settings,true,true );
    toc
    %% save model
    modelname = sprintf('Model %03.0f - RMSDs',model_number);
    for i=1:length(rmsds)
        modelname = strcat(modelname, sprintf(' - %.0f=%.3f.mat', run_settings(2,i),rmsds(i)));
    end
    net.save(name);
end

end

