s=what('ESCPPNValues');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
data = load(['ESCPPNValues/' char(matfiles(1))]);
resolution = 1024;

connectionWeights = data.connectionWeights;
cppnWeightValues =data.cppnWeightValues;
cppnTypeCodeNeurons1=data.cppnTypeCodeNeurons1;
cppnTypeCodeNeurons2=data.cppnTypeCodeNeurons2;
cppnTypeCodeNeurons3=data.cppnTypeCodeNeurons3;
cppnTypeCodeValues1=data.cppnTypeCodeValues1;
cppnTypeCodeValues2=data.cppnTypeCodeValues2;
cppnTypeCodeValues3=data.cppnTypeCodeValues3;
neuronsPos=data.neuronsPos;
neuronsType=data.neuronsType;

% Plot weight values (function)

pos= -1:2/(resolution-1):1;
[X,Y]= meshgrid(pos,pos);
Z=cppnWeightValues';

fig1 = figure;
axis('square')
hold on
s=surf(X,Y,Z)
colormap([0,0,1]);
s.EdgeColor = 'none';
%s.EdgeAlpha=0.01;


% fig1 = figure;
% axis('square')
% hold on
%  scatter3(X(:),Y(:),Z(:),36,'blue','.'); %36 default point size

%Plot  Neurons into Graph
connectionWeightsT =connectionWeights';
[a,b] = find(connectionWeightsT~=0);
d = sub2ind(size(connectionWeightsT),a,b);
%[X2,Y2]= meshgrid(neuronsPos(a),neuronsPos(b));
%Z2=connectionWeights(d);
scatter3(neuronsPos(b),neuronsPos(a),connectionWeightsT(d),36,'red','.');

hold off
a=2


% Pl