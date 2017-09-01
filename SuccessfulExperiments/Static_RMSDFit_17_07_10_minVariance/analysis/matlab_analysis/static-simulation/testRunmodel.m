

savePath='C:\Users\Maximus Mutschler\Downloads\test';
netPath='C:\Users\Maximus Mutschler\Downloads\test\staticTest_aftsim.mat';
getModelParams('dEsNum.mat');
name = 'test_static_simulation';
dEsNumstruct =load('dEsNum.mat');
dEsNum =dEsNumstruct.dEsNum;

dEsConnections = ones(dEsNum);
save('dESConnections.mat','dEsConnections');
%writeSync("syncfile.txt","close");
writeSync("syncfile.txt","simulate_plot \n staticTest");

angle_start ='65';
angles_to_simulate = '[30,90,0,60,135,120]';
time_per_angle = '30000';

%tic
% wichtig, code endet nie, da kein zweiter thread implementiert um write
% snyc = close zu setzen
%  runModel(name, savePath, equationParamsPath, weightsMatrixPath,DEsConnectionsPath,resultPath, angle_start, angles_to_simulate, time_per_angle )
runModel(name,'result_rmsd',netPath,angle_start, angles_to_simulate, time_per_angle )

%toc
rmsd = load('result_rmsd');
