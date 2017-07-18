

savePath='C:\Users\Maximus Mutschler\Downloads\test';

getModelParams('dEsNum.mat');
name = 'test_static_simulation';
dEsNumstruct =load('dEsNum.mat');
dEsNum =dEsNumstruct.dEsNum;
w=load('weightsMatrix.mat');
w= w.weightsMatrix;

%dEsConnections = ones(dEsNum);
dEsConnections = double(w(1:96,145:240)~=0) % D ES
save('dESConnections.mat','dEsConnections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

angle_start ='135';
angles_to_simulate = '[60,30,0,80,135,15,100,40,75]';
time_per_angle = '30000';

%tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write
% snyc = close zu setzen
%  runModel(name, savePath, equationParamsPath, weightsMatrixPath,DEsConnectionsPath,resultPath, angle_start, angles_to_simulate, time_per_angle )
runModel(name,savePath,'EquationParams.mat', 'weightsMatrix.mat','dESConnections.mat','result_rmsd',angle_start, angles_to_simulate, time_per_angle )
%toc
rmsd = load('result_rmsd');
