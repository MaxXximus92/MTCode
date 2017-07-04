

savePath='C:\Users\Maximus Mutschler\Downloads\test';
name = 'test';
neuronTypesStruct =load('neuronTypes.mat');
neuronTypes =neuronTypesStruct.neuronTypes;
ns= size(neuronTypes);
connections = ones(ns(2),ns(2));
save('connections.mat','connections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

angle_start ='65';
angles_to_simulate = '[10,30,60,40,100,90,120,65]';
time_per_angle = '1000';

%tic
runModel(name,savePath,'EquationParams.mat','connections.mat','result_fitness',angle_start, angles_to_simulate, time_per_angle )
%toc
rmsd = load('result_fitness');
