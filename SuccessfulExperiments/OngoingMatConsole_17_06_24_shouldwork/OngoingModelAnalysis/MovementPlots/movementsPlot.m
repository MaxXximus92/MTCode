s= dir('data')
set(0,'DefaulttextInterpreter','none')
 

t0=[];
t35=[];
t75=[];
t105=[];
t135=[];

for j = 1:length(s);
    i=string((s(j).name));
    if(length(i)>30)
    end
    switch 1 
        case contains(i,"target 0")
            t0=[t0 i] 
        case contains(i,"target 35")
            t35=[t35 i]
        case contains(i,"target 75")
            t75=[t75 i]
        case contains(i,"target 105")
            t105=[t105 i]
        case contains(i,"target 135")
            t135=[t135 i]
    end
    
end
all= {t0 t35 t75 t105 t135}

for ti = all
ti=ti{1};
fig1= openfig(['data/' char(ti(1))],'new','visible')
 L1 = findobj(fig1,'type','line');
 set(L1(2),'Color',[0,0,1])
Xdatas= L1(2).XData';
 for k = 2:length(ti)
    fig2= openfig(['data/' char(ti(k))])
     L = findobj(fig2,'type','line');
     set(L(2),'Color'   ,[0,0,1])
     Xdatas= [Xdatas L(2).XData'];
     copyobj(L,findobj(fig1,'type','axes'));
 end
 
ylabel(findobj(fig1,'type','axes'),'Angle [°]');
meandata= mean(Xdatas');
figure(fig1)
hold on
plot(meandata,L1(2).YData,'r');
%axis square
hold off 
end
a=4