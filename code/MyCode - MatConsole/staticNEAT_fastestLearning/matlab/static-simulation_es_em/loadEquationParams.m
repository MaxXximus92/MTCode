function eqParams= loadEquationParams(path)
if(exist(path,'file')==2)
    eqParamsStruct = loadSync(path);
    a=eqParamsStruct.a;
    b=eqParamsStruct.b;
    d=eqParamsStruct.d;
    eqParams = [a,b,d];
else
    msgID = 'loadEquationParams:FileDoesNotExist';
    msg = ['FileDoesNotExist: ' path];
    throw(MException(msgID,msg));
end
end

