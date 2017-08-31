function [ ] = runModel(name,resultPath,netPath,angle_start, angles_to_simulate, time_per_angle )




angles_to_simulate= str2num(angles_to_simulate);
angle_start =str2double(angle_start);
time_per_angle =str2double(time_per_angle);

net = load(netPath);
net=net.net;

saveName = name;

fitness = net.simulate(saveName, angle_start, angles_to_simulate, time_per_angle);

%net.save(saveName);

saveFitnessSync(resultPath,fitness)

quit;




