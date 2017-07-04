% parameters
angle_start = 65;
angles_to_simulate = [0,90,70,10,120];
time_per_angle = 20000; %[ms]

%% Start simulation
uiopen('load');net.break_run=0;
model_idx = 1;
successfully = net.simulate(model_idx,angle_start,angles_to_simulate,time_per_angle);