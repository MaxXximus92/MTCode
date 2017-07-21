s=what('CPPNValues');
matfiles=s.mat;
set(0,'DefaulttextInterpreter','none')
    xlabs=["D_Positions","ES_Positions","IS_Positions","IS_Positions","ES_Positions","EM_Positions","IM_Positions","IM_Positions"];
    ylabs=["ES_Positions","IS_Positions","IS_Positions","ES_Positions","EM_Positions","IM_Positions","IM_Positions","EM_Positions"];
    headings=["D_ES","ES_IS","IS_IS","IS_ES","ES_EM","EM_IM","IM_IM","IM_EM"];

for i = 1:numel(matfiles)
    cppnMatrices= load(['CPPNValues/' char(matfiles(i))]);
    fields= fieldnames(cppnMatrices);

    % fig =  figure() ;
    
     for j = 1:8
        
        weightM=cppnMatrices.(fields{j});
        numIn =size(weightM,1);
        numOut=size(weightM,2);
        threshold=0.75;
        
        positionsx= 0:1/(numIn-1):1 ;
        positionsy= 0:1/(numOut-1):1 ;
        [X,Y]= meshgrid(positionsx,positionsy);
        Z=weightM'; % Transpose very important da werte spaltenweise y const x steigend gescannt werden
        C=double(Z < threshold)+1; %verdreht da sonst  plots die nie über den threshold kommen grün werden
        cmap=[0 1 0
              0 0 1];
        %subplot(2,2,j) 
        fig =  figure();
        axis('square')
        hold on
        s=surf(X,Y,Z,C)%'FaceAlpha',0.9);
        colormap(cmap);
        
        
        
        %s.EdgeColor = 'none';
        %s.EdgeAlpha=0.6;
        %camlight right;
        %lighting phong
        xlab=xlabs(j);
        ylab=ylabs(j);
        xlabel(xlab);
        ylabel(ylab);
        zlabel('CPPN outputs');
        %h=legend('CPPN outputs','bsdf');%,'Location','eastoutside');%'Location','southwest');
        filename =char(matfiles(i));
        filename = filename(1:end-4)
        title([filename ' ' headings(j)]);
        hold off
        savefig(['figures/' filename '_' char(headings(j)) '.fig'])
       % close(fig);
     end
   % suptitle(char(matfiles(i)))
end



