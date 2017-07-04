function weightsMatrix = loadWeightsMatrix(path)
if(exist(path,'file')==2)
    weightsMatrixStruct = load(path);
    weightsMatrix= weightsMatrixStruct.weightsMatrix;
else
    msgID = 'loadWeighsMatrix:FileDoesNotExist';
    msg = ['FileDoesNotExist: ' path];
    throw(MException(msgID,msg));
end
end
