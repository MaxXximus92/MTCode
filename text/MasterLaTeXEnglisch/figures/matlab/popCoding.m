


normPDF(2,0.5)
p=1.5;
f1=figure();
hold on
l1=fplot(@(x)normpdf(x,2,0.8)*2,[0,5],'b');
l2=fplot(@(x)normpdf(x,2.5,0.8)*2,[0,5],'r');
l3=fplot(@(x)normpdf(x,3,0.8)*2,[0,5],'color',[1,0.5,0]);
pl=line([p,p], [0, 1]);
pl.Color = 'green';
pl.LineStyle = '--';
plot(p,normpdf(p,2,0.8)*2,'bx')
plot(p,normpdf(p,2.5,0.8)*2,'rx')
plot(p,normpdf(p,3,0.8)*2,'x','color',[1,0.5,0])
%plot(2,0,'bx')
%plot(2.5,0,'rx')
%plot(3,0,'x','color',[1,0.5,0])
% fplot(@(x)normpdf(x,p,0.8)*2,[0,5],'b');
% pl=line([2,2], [0, 1]);
% pl=line([2.5,2.5], [0, 1]);
% pl=line([3,3], [0, 1]);
l=legend([l1,l2,l3],'$\mathcal{N}(2,0.8)*2$','$\mathcal{N}(2.5,0.8)*2$','$\mathcal{N}(3,0.8)*2$' )
set(l,'Interpreter','Latex');
hold off
normpdf(p,2,0.8)*2
normpdf(p,2.5,0.8)*2
normpdf(p,3,0.8)*2

ans =

    0.8204


ans =

    0.4566


ans =

    0.1720