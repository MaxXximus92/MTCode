function [ ] = testRunmodel()
train_time = '8000'; %runtime per trial [ms] %initial 120000

savePath='C:\Users\Maximus Mutschler\Downloads\test';

getModelParams('dEsNum.mat');
name = 'test';
dEsNumstruct =load('dEsNum.mat');
dEsNum =dEsNumstruct.dEsNum;

net2 = load('StaticExperiment_LastGeneration_Fitness_0.62843_order_2.mat');
net2=net2.net; 
weights = net2.getDEsWeights();
dEsConnections = weights >0;
save('dESConnections.mat','dEsConnections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write
% snyc = close zu setzen
%runModel(name, max_runTime, savePath, equationParamsPath, weightsMatrixPath,DEsConnectionsPath,resultPath, syncPath )
runModel(name,train_time,savePath,'EquationParams.mat', 'weightsMatrix.mat','dESConnections.mat','result_rmsd', 'syncfile.txt')
toc
rmsd = load('result_rmsd');
end
