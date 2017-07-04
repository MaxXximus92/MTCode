% parameters
num_models = 1;%100;
run_time = 1200; %runtime per trial [ms] %initial 120000
run_settings = [1;35];%[ones(1,5).*65;     %start angles
                %35,35,75,105,135]; %target angles
trials_per_settings = 2;

%% Start simulation
for model_number=1:num_models
    %Construct network
    savePath='C:\Users\Maximus Mutschler\Downloads\test';
    net = spikenet(256,30,'settings.xls',savePath);
    net.save('net');
    
    %!matlab -r !! new matlab instance
    % !powershell -Command "& {dir REGISTRY::HKEY_CLASSES_ROOT\CLSID -include PROGID -recurse | foreach {$_.GetValue(""""")} }"
    
   % a=net.getEsEmNum();
    %b= net.getEsEmCells();
    esEmWeights = net.getEsEmWeights();
    esEmWeights = ones(size(esEmWeights)); % just for testing
    net.setEsEmWeights(esEmWeights);
   % esEmWeights = net.getEsEmWeights();
    rmsds = net.simulate( model_number, run_time, run_settings, trials_per_settings,true,true );
    
    %% save model
    modelname = sprintf('Model %03.0f - RMSDs',model_number);
    for i=1:length(rmsds)
        modelname = strcat(modelname, sprintf(' - %.0f=%.3f.mat', run_settings(2,i),rmsds(i)));
    end
     net.save(modelname);
end