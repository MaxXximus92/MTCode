s= what('data')
set(0,'DefaulttextInterpreter','none')
matfiles=s.mat;



RMSDs=[];
anglesReachTimes= [];
totalRMSDs=[];
totalRMSDsFulltime=[];
for i = 1:numel(matfiles)
data= load(['data/' char(matfiles(i))]);
RMSDs = vertcat(RMSDs,data.angleRMSDs);
anglesReachTimes = vertcat(anglesReachTimes,data.anglesReachTimes);
totalRMSDs = [totalRMSDs data.totalRmsd];
totalRMSDsFulltime =[totalRMSDsFulltime data.totalRMSDfullTime];
end
anglestoSimulate = data.anglesToSimulate;

figure
hold on
boxplot(RMSDs,'Labels',anglestoSimulate)
%boxplot([t0',t35',t75',t105',t135'],'Labels',{'0','35','75','105','135'})

ylabel('RMSD [°]')
xlabel('Target Angle [°]')
ylim([0,135])

baseline= [82.4492   17.5024   87.2912   30.1461  103.5705  110.3999] 
bw=0.4;
for k = 1 : length(baseline)
    line([k-bw,k+bw],[baseline(k),baseline(k)],'Color','green','LineStyle','-')
end    
axis square
hold off 



figure
hold on
boxplot(anglesReachTimes/1000,'Labels',anglestoSimulate)
%boxplot([t0',t35',t75',t105',t135'],'Labels',{'0','35','75','105','135'})

ylabel('Time neaded to reach [s]')
xlabel('Target Angle [°]')
ylim([0,30])

baseline= [-1,100,-1, 30000, -1,-1]/1000;  % 20 eig 30000
bw=0.4;
for k = 1 : length(baseline)
    if(baseline(k) ~= -1/1000)
    line([k-bw,k+bw],[baseline(k),baseline(k)],'Color','green','LineStyle','-')
    end
end    
axis square
hold off 



figure
hold on
boxplot(totalRMSDs)
ylabel('RMSD[°] t=10s-30s')
xlabel('30 best static models')
ylim([0,70])
xlim([0.8,1.2])
bw=0.1;
base= 80.1538;
line([1-bw,1+bw],[base,base],'Color','green','LineStyle','-')
%axis square
hold off 

figure
hold on
boxplot(totalRMSDsFulltime)
ylabel('RMSD[°] t=0s-30s')
xlabel('30 best static models')
ylim([0,70])
xlim([0.8,1.2])
bw=0.1;
base= 79.2993;
line([1-bw,1+bw],[base,base],'Color','green','LineStyle','-')
%axis square
hold off 

BestMeanRMSD = min(mean(RMSDs'))
BestMeanAnglesReachTime = min(mean(anglesReachTimes'))/1000

