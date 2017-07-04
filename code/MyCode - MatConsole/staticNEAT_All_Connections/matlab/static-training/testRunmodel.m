
run_time = '800'; %runtime per trial [ms] %initial 120000

savePath='C:\Users\Maximus Mutschler\Downloads\test';

getModelParams('neuronTypes.mat');
name = 'test_all_connections';
neuronTypesStruct =load('neuronTypes.mat');
neuronTypes =neuronTypesStruct.neuronTypes;
ns= size(neuronTypes);
connections = ones(ns(2),ns(2));
save('connections.mat','connections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write

runModel(name,run_time,savePath,'EquationParams.mat','connections.mat','result_fitness', 'syncfile.txt')
toc
rmsd = load('result_fitness');
