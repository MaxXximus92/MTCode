
xvalES = repelem(-0.25,96)
yvalES =  -0.5:1/96:(0.5-1/96)   
xvalEM = repelem(0.25,48)
yvalEM =  -0.5:1/48:(0.5-1/48)  
fig =figure()
hold on
scatter(xvalES,yvalES,'rx')
scatter(xvalEM,yvalEM,'gx')
xlim([-0.6,0.6])
ylim([-0.6,0.6])
xlabel('x')
ylabel('y')
ax = gca;
ax.XAxisLocation = 'origin';
ax.YAxisLocation = 'origin';
axis equal
legend('96 ES neuron positions','48 EM neuron positions','Location','southoutside')
hold off