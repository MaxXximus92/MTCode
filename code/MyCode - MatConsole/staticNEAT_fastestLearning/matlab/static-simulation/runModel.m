function [ ] = runModel(name, savePath, equationParamsPath, weightsMatrixPath,DEsConnectionsPath,resultPath, angle_start, angles_to_simulate, time_per_angle )
%todo numNeurons vllt drin lassen und nicht verwenden um C# code nicht zu
%ändern, aber eigentlich egal 2 min aufwand

% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht

weightsMatrix = loadWeightsMatrix(weightsMatrixPath); % vllt v5 einstellen
equationParams = loadEquationParams(equationParamsPath);
angles_to_simulate= str2num(angles_to_simulate);
angle_start =str2double(angle_start);
time_per_angle =str2double(time_per_angle);

settings='settings.xls';
spikingThreshold = 30;


net = spikenet(spikingThreshold,settings,savePath,equationParams,weightsMatrix);
% spikenet(spikingThreshold,settingsFile,savePath,equationParams, weightsMatrix)
saveName = name;

DEsConnectionsStruct = loadSync(DEsConnectionsPath);
DEsConnections=DEsConnectionsStruct.dEsConnections;

net.setDEsConnections(DEsConnections);

fitness = net.simulate(saveName, angle_start, angles_to_simulate, time_per_angle);

net.save(saveName);

saveFitnessSync(resultPath,fitness)

quit;




