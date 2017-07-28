
train_time = '400'; %runtime per trial [ms] %initial 120000

angles_to_learn='[0,135]';%'[0,135]';
angles_to_simulate='[10,123,61]';%'[10,62.5,135]'
angle_simulation_time='200'; 


savePath=[pwd '/testsave'];


name = 'test';
%neuronTypesStruct =load('neuronTypes.mat');
%types =neuronTypesStruct.neuronTypes;
net=load('StaticExperiment_LastGeneration_Fitness_0.62843_order_2.mat');
net= net.net;
neuronTypes = net.neuronTypes;
    types = unique(neuronTypes);  %'D' =0, 'EM'=1, 'ES'=2, 'IM'=3 'IS'=4
    typNums= nan(size(neuronTypes));
    for typeNumber=1:length(types)
       typNums(strcmp(neuronTypes,types(typeNumber))) = typeNumber-1; 
    end
    types = typNums;


connections = net.weightsMatrix ~=0;
%rng(0)
%types=randi([0,4],1,100);
%types=sort(types);
%rng(0)
%connections = randi([0,1],length(types));

%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");
save('data.mat','connections','types');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write

runModel(name,train_time,savePath,'data_com_2.mat','result_rmsd', 'syncfile.txt',angles_to_learn,angles_to_simulate,angle_simulation_time )
toc
rmsd = load('result_fitness');
