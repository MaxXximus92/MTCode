% parameters
num_models = 1;%100;
run_time = 120000; %runtime per trial [ms]
run_settings = [ones(1,5).*65;     %start angles
                35,35,75,105,135]; %target angles
trials_per_settings = 2;

%% Start simulation
for model_number=1:num_models
    %Construct network
    net = spikenet(256,30,'settings.xls');
    rmsds = net.simulate( model_number, run_time, run_settings, trials_per_settings );
    
    %% save model
    modelname = sprintf('Model %03.0f - RMSDs',model_number);
    for i=1:length(rmsds)
        modelname = strcat(modelname, sprintf(' - %.0f=%.3f', run_settings(2,i),rmsds(i)));
    end
    save(modelname,'net');
end