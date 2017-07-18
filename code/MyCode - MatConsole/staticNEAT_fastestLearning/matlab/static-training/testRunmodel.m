function [ ] = testRunmodel()
run_time = '120000'; %runtime per trial [ms] %initial 120000

savePath='C:\Users\Maximus Mutschler\Downloads\test';

getModelParams('dEsNum.mat');
name = 'test';
dEsNumstruct =load('dEsNum.mat');
dEsNum =dEsNumstruct.dEsNum;

w=load('weightsMatrix.mat');
w= w.weightsMatrix;

%dEsConnections = ones(dEsNum);
dEsConnections = double(w(1:96,145:240)~=0) % D ES
save('dESConnections.mat','dEsConnections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write
% snyc = close zu setzen
%runModel(name, max_runTime, savePath, equationParamsPath, weightsMatrixPath,DEsConnectionsPath,resultPath, syncPath )
runModel(name,run_time,savePath,'EquationParams.mat', 'weightsMatrix.mat','dESConnections.mat','result_rmsd', 'syncfile.txt')
toc
rmsd = load('result_rmsd');
end
