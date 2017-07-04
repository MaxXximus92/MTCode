
run_time = '120000'; %runtime per trial [ms] %initial 120000
run_settings = '[65;35]';%[ones(1,5).*65;     %start angles
                %0,35,75,105,135]; %target angles
numNeurons='256';
savePath='C:\Users\Maximus Mutschler\Downloads\test';

getModelParams(numNeurons,'esemNum.mat');
name = 'test';
esemNumstruct =load('esemNum.mat');
esemNum =esemNumstruct.esemNum;

esemConnections = ones(esemNum);
save('esemConnections.mat','esemConnections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n randNoiceAndPOP2");

tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write
% snyc = close zu setzen
runModel(name,numNeurons,run_time,run_settings,savePath,'EquationParams.mat', 'weightsMatrix.mat','esemConnections.mat','result_rmsd', 'syncfile.txt')
%function [ ] = runModel(name,numNeurons, runTime,runSettings, savePath, equationParamsPath, weightsMatrixPath,esEmConnectionsPath,resultPath, isSave )
toc
rmsd = load('result_rmsd');
rmsd