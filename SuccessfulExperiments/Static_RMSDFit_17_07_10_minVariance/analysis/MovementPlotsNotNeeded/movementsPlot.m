s= dir('dataTrain')
set(0,'DefaulttextInterpreter','none')
 

all=[]

for j = 1:length(s);
    i=string((s(j).name));
    if(length(char(i))>5)
        all= [all i];
    end
    
end



fig1= openfig(['dataTrain/' char(all(1))],'new','visible')
 L1 = findobj(fig1,'type','line');
 set(L1(2),'Color',[0,0,1])
Xdatas= L1(2).XData';
 for k = 2:length(all)
    fig2= openfig(['dataTrain/' char(all(k))])
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
axis square
hold off 

a=4