function saveWeightsMatrix(weightsMatrix,path)
if(exist(path)==0)
    save(path,'weightsMatrix');
else
    msgID = 'saveWeightsMatrix:FileAllreadyExists';
    msg = ['File allready exisits:' path];
    throw(MException(msgID,msg));
end
end

%if exist([this.savePath 'EquationParams.mat'],'file')==2