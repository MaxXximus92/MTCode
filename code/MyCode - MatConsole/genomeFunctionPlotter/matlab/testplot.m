xrange=[0,1];
yrange=[0,1];
connectionPositionsX=[0:0.01:1];
connectionPositionsY=[0:0.01:1];
connectionValues=(connectionPositionsX-0.5).^2+(connectionPositionsY-0.5).^2;
threshold=0.25;

positions =[0:0.005:1];
posx= repmat(positions,1,length(positions));
posy= repelem(positions,length(positions),1);
posy = posy(:)';
values= (posx-0.5).^2+(posy-0.5).^2;
%[posx2,posy2,z]= meshgrid(positions,positions,values);
%rng(42)
%values = rand(1,length(posx))

[X,Y]= meshgrid(positions);
Z= reshape(values,[length(positions),length(positions)]);
xlab='D\_Positions';
ylab='ES\_Positions';
%data =[posx;posy;values]';
C=double(Z >= threshold)+1;
cmap=[0 0 1
      0 1 0];
fig1 = figure;
hold on
%s=surf(X,Y,Z,C,'FaceAlpha',0.9);
%colormap(cmap)
%s.EdgeColor = 'none';
%camlight left; 
%lighting phong
scatter3(posx,posy,values,36,'blue','.'); %36 default point size
% plot neurons
scatter3(connectionPositionsX,connectionPositionsY, connectionValues,50,'red','.');
% plot neuron on x axes
scatter3(connectionPositionsX,zeros(length(connectionPositionsX),1), zeros(length(connectionPositionsX),1),36,'red','.');
scatter3(zeros(length(connectionPositionsX),1),connectionPositionsY, zeros(length(connectionPositionsX),1),36,'red','.');
xlabel(xlab);
ylabel(ylab);
zlabel('CPPN outputs');
legend('CPPN outputs','Value at neuron positions');%,'Location','eastoutside');%'Location','southwest');
hold off
remove(fig1)
% figure()
% posxg = meshgrid([0:1/96:1-1/96]);
% posx = posxg(:);
% posy = posxg';
% posy = posy(:);
% values = Z';
% values = values(:);
% scatter3(posx,posy,values,36,'blue','.');
a=2;