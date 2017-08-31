fig =openfig('Model randNoiceAndPOP2 - Run 2  - movement (start 65 - target 95)RMSD=19.82513.fig')
set(fig,'Visible','on')

axes = get(fig, 'children');
plot = get(axes, 'children');
set(plot(2),'Color',[0,0,1]);
%set(plot(2),'LineWidth',2);
%set(plot(2),'LineStyle',':');
xlabel('test')