s=what('probs');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
probs =[];
for i = 1:numel(matfiles)
    data= load(['probs/' char(matfiles(i))]);
    probs =[probs data.prob.D_ES*100];
end

figure
hold on
%boxplot([t0'],'Labels',{'0'})
boxplot(probs,'Labels',{'30 best static models'});
ylabel('D-ES connection ratio [%]');


ylim([0,100])
bw=0.1;
line([1-bw,1+bw],[20,20],'Color','green','LineStyle','-');

mean(probs)