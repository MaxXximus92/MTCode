s=what('matfiles');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
for i = 1:numel(matfiles)
net= load(['matfiles/' char(matfiles(i))]);
net=net.net;
numNeurons= net.numNeurons;
[prob,neuronTypes,numConnections]= getConnectionProb(net);
savename=char(matfiles(i));
savename= savename(1:end-4)
diary(['probs/' savename '_prob.txt']);
diary on
numNeurons
neuronTypes
numConnections
prob
diary off
save(['probs/' savename '_prob.mat'],'numNeurons','neuronTypes','numConnections','prob');
end