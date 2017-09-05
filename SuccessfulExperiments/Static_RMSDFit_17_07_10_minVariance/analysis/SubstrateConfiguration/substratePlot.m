numNeu= 96;
positions= [0:1/96:1-1/96]
figure()
hold off
axis off
ax1= gca;
  
axsd=axes('Position',ax1.Position+[-0.15,0.0,0,0],...
             'NextPlot','add',...           %# Add subsequent plots to the axes,
             'DataAspectRatio',[1 1 1],...  %#   match the scaling of each axis,
             'XLim',[0 eps],...               %#   set the x axis limit,
             'Color','none',...               %#   and don't use a background color
             'YLim',[0 1]);             %#   set the y axis limit (tiny!),
plot(zeros(numNeu,1),positions ,'xr')
ylabel('D Neuron Position')

ax2 = axes('Position',ax1.Position+[0.15,0.0,0,0],...
             'NextPlot','add',...           %# Add subsequent plots to the axes,
             'DataAspectRatio',[1 1 1],...  %#   match the scaling of each axis,
             'XLim',[0 eps],...               %#   set the x axis limit,
             'Color','none',...               %#   and don't use a background color
             'YLim',[0 1]);             %#   set the y axis limit (tiny!),
%axes(handles.ax2); 

plot(ax2,zeros(numNeu,1),positions ,'xg')
ylabel('ES Neuron Position')
ax2.YAxisLocation = 'right';

