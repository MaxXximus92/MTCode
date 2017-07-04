function [ ] = runModel(name, savePath, equationParamsPath,connectionsPath,resultPath, angle_start, angles_to_simulate, time_per_angle )
%todo numNeurons vllt drin lassen und nicht verwenden um C# code nicht zu
%�ndern, aber eigentlich egal 2 min aufwand

% maxNumCompThreads(1)
% fprintf('set num Threads to 1')
% beim compilieren single thread use angeben ! und beim starten des
% programms auch -> hilft unter windows beides nicht

equationParams = loadEquationParams(equationParamsPath);
eql= size(equationParams);
weightsMatrix = zeros(eql(1),eql(1));

angles_to_simulate= str2num(angles_to_simulate);
angle_start =str2double(angle_start);
time_per_angle =str2double(time_per_angle);

settings='settings.xls';
spikingThreshold = 30;

% todo read from file
%isSave =str2num(isSave);

% todo constructor fehlt...
net = spikenet(spikingThreshold,settings,savePath,equationParams,weightsMatrix);
% spikenet(spikingThreshold,settingsFile,savePath,equationParams, weightsMatrix)
saveName = name;

            connectionsStruct = loadSync(connectionsPath);
            connections=connectionsStruct.connections;
            
            net.setConnections(connections);

fitness = net.simulate(saveName, angle_start, angles_to_simulate, time_per_angle);

net.save(saveName);

saveFitnessSync(resultPath,fitness)

quit;




