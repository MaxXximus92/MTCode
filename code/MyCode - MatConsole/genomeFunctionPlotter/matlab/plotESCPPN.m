s=what('ESCPPNValues');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')

for q = 1:numel(matfiles)
    data = load(['ESCPPNValues/' char(matfiles(q))]);
    filename =char(matfiles(q));
    filename = filename(1:end-4);
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
    
    %
    % fig1 = figure;
    % axis('square')
    % hold on
    %  scatter3(X(:),Y(:),Z(:),36,'blue','.'); %36 default point size
    
    %Plot  Neurons into Graph
    %connectionWeightsT .. TODO;
    [a,b] = find(connectionWeights~=0);
    d = sub2ind(size(connectionWeights),a,b);
    %[X2,Y2]= meshgrid(neuronsPos(a),neuronsPos(b));
    %Z2=connectionWeights(d);
    scatter3(neuronsPos(a),neuronsPos(b),connectionWeights(d),36,'red','.');
    xlab='x';
    ylab='y';
    xlabel(xlab);
    ylabel(ylab);
    zlabel('CPPN Output');
    title([char(matfiles(q)), 'CPPN Output']);
    hold off
    
    savefig(['figures/' filename '_' 'CPPN_Output' '.fig'])
    
    % build color matrix
    colors = zeros(length(neuronsType),3);
    colorarray={[0,0,1],[1,0,0],[0,1,0],[1.0,0.4,0],[0.4,1,0.8]}  %D EM ES IM IS
    for i = 0:4
        indx = find(neuronsType == i);
        for k = indx ,
            colors(k,:) = colorarray{i+1};
        end
    end
    
    CodeValues= {cppnTypeCodeValues1,cppnTypeCodeValues2,cppnTypeCodeValues3}
    CodeNeurons = {cppnTypeCodeNeurons1,cppnTypeCodeNeurons2,cppnTypeCodeNeurons3}
    titles = ["CPPN Output Neuron 2","CPPN Output Neuron 3","CPPN Output Neuron 4"]
    % Plot NeuCode1 values
    fig2 = figure;
    for  i = 1:3
       
        subplot(3,1,i)
         hold on
        line([-1,1],[0,0],'Color','black','LineStyle','--')
        h1=plot(pos,CodeValues{i},10,[0,0,0],'-.','Color','black');%'MarkerFaceAlpha',0.1,'MarkerEdgeAlpha',0.1);
        xlim([-1,1]);
        ylim([-1,1]);
        handles =[];
        for j = 0:4
            indx = find(neuronsType == j);
            cneu= CodeNeurons{i};
            h=scatter(neuronsPos(indx),cneu(indx),50,colors(indx,:),'d','filled');
            handles = [handles h];
        end
        xlabel('Substrate Position');
        ylabel('CPPN Output');
        legend(handles,'D','EM','ES','IM','IS','Location','eastoutside');
        title([''; titles(i)]);
        hold off;
    end
    suptitle(char(matfiles(q)))
    savefig(['figures/' filename '_' 'Neuron_Encodings' '.fig'])
end
a=3