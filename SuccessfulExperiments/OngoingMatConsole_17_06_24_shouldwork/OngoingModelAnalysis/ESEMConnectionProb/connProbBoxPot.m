s=what('data');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
probs =[];
for i = 1:numel(matfiles)
    data= load(['data/' char(matfiles(i))]);
    probs =[probs data.prob.ES_EM*100];
end

figure
hold on
%boxplot([t0'],'Labels',{'0'})
boxplot(probs,'Labels',{'30 best ongoing models'});
ylabel('ES-EM connection ratio [%]');

xlim([0.8,1.2])
ylim([0,100])
bw=0.1;
line([1-bw,1+bw],[8,8],'Color','green','LineStyle','-');