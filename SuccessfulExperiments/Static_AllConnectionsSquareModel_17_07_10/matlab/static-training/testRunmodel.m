function [ ] = testRunmodel()
train_time = '4000'; %runtime per trial [ms] %initial 120000

angles_to_learn='[0,135]';%'[0,135]';
angles_to_simulate='[10,123,61]';%'[10,62.5,135]'
angle_simulation_time='2000'; 


savePath='C:\Users\Maximus Mutschler\Downloads\test';

getModelParams('neuronTypes.mat');
name = 'test';
neuronTypesStruct =load('neuronTypes.mat');
typNums =neuronTypesStruct.neuronTypes;

net2 = load('StaticExperiment_LastGeneration_Fitness_0.62843_order_2.mat');
net2=net2.net; 
weights = net2.weightsMatrix;
connections = weights ~=0;

%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");
save('connections.mat','connections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write

runModel(name,train_time,savePath,'EquationParams.mat','connections.mat','result_rmsd', 'syncfile.txt',angles_to_learn,angles_to_simulate,angle_simulation_time )
toc
rmsd = load('result_fitness');
end