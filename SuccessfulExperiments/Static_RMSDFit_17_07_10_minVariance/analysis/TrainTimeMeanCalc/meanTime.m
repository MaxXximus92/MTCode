s= what('data')
set(0,'DefaulttextInterpreter','none')
matfiles=s.mat;


times=[];
for i = 1:numel(matfiles)
data= load(['data/' char(matfiles(i))]);
times =[times data.trainingTime];
end
mean(times)

