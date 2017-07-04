% parameters
num_models = 200;
max_run_time = 1800000; %[ms]
spiking_threshold = 30; %[mV]
settings_file = 'settings.xls';

%% Start simulation
for model_number=1:num_models
    net = spikenet(spiking_threshold,settings_file);
    successfully = net.train(model_number, max_run_time);
    
    if successfully, successfully='successfully';
    else successfully='unsuccessfully'; end
    save(sprintf('Model %03.0f (trained %s)',model_number,successfully),'net');
end