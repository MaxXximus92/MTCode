s=what('CPPNValues');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
for i = 1:numel(matfiles)
cppnValues= load(['CPPNValues/' char(matfiles(i))]);
cppnValues = cppnValues.dEsConnections;

numIn =96;
numOut=96;
threshold=0.75;

positions= [0:1/96:1-1/96] ;
[X,Y]= meshgrid(positions);
Z=cppnValues;
C=double(Z >= threshold)+1;
cmap=[0 0 1
      0 1 0];
fig =  figure() ;
hold on
s=surf(X,Y,Z,C,'FaceAlpha',0.9);
colormap(cmap);
%s.EdgeColor = 'none';
%s.EdgeAlpha=0.1;
%camlight right; 
%lighting phong
xlab='D_Positions';
ylab='ES_Positions';
xlabel(xlab);
ylabel(ylab);
zlabel('CPPN outputs');
legend('CPPN outputs','Value at neuron positions');%,'Location','eastoutside');%'Location','southwest');
title(char(matfiles(i)));
hold off
savefig(['figures/' char(matfiles(i)) '.fig'])
close(fig);
end



