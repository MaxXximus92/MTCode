s=what('matfiles');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
for i = 1:numel(matfiles)
net= load(['matfiles/' char(matfiles(i))]);
net=net.net;
prob= getConnectionProb(net);
diary(['probs/' char(matfiles(i)) '_prob.txt']);
diary on
prob.EM_IM
diary off
save(['probs/' char(matfiles(i)) '_prob.mat'],'-struct','prob');
end