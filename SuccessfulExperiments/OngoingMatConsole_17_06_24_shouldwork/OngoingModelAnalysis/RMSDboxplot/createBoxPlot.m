s= dir('data')
set(0,'DefaulttextInterpreter','none')
 

t0=[];
t35=[];
t75=[];
t105=[];
t135=[];

for j = 1:length(s);
    i=(s(j).name);
    if(length(i)>30)
    start=find(i== '=');
    rmds = str2double(i(start+1:end-4))
    end
    switch 1 
        case contains(i,"target 0")
            t0=[t0 rmds] 
        case contains(i,"target 35")
            t35=[t35 rmds]
        case contains(i,"target 75")
            t75=[t75 rmds]
        case contains(i,"target 105")
            t105=[t105 rmds]
        case contains(i,"target 135")
            t135=[t135 rmds]
    end
    
end
all=[t0 t35 t75 t105 t135];
figure()
x0=10;
y0=10;
width=600;
height=600;
set(gcf,'units','points','position',[x0,y0,width,height])
hold on


boxplot([t0',t35',t75',t105',t135'],'Labels',{'0','35','75','105','135'})

ylabel('RMSD [°]')
xlabel('Target Angle [°]')

ylim([0,70])

%baseline: [1.40895,7.56571,7.70776,9.72355,10.85780]
%               0      35     75      105      135
baseline= [1.40895,7.56571,7.70776,9.72355,10.85780]

bw=0.4;
for k = 1 : length(baseline)
    line([k-bw,k+bw],[baseline(k),baseline(k)],'Color','green','LineStyle','-')
end    
axis square
hold off 

diary('totalRMSDs')
diary on
RMSDBaseline= sqrt(sum(baseline.^2)/length(baseline))
all=[t0 t35 t75 t105 t135];
RMSDALL=  mean(all)
diary off

figure()
boxplot(all, 'positions',1,'labels','mean RMSD over all angles')
bw=0.15;
k=1
xlim([0.8,1.2])
line([k-bw,k+bw],[RMSDBaseline,RMSDBaseline],'Color','green','LineStyle','-')
ylabel('RMSD [°]')
x0=10;
y0=10;
width=150;
height=550;
set(gcf,'units','points','position',[x0,y0,width,height])

% means = [mean(t0) mean(t35) mean(t75) mean(t105) mean(t135)];
% RMSDMEAN=sqrt(sum(means.^2)/length(means))
