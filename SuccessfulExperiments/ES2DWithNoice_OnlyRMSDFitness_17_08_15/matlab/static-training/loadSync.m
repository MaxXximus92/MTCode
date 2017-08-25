function [ resultstruct ] = loadSync( path )
if(exist(path,'file')==2)
while true
    try
        resultstruct = load(path);
        return
    catch ME
        fprintf('Error while reading from %s  \n with message %s \n trying again \n',path,ME.message);
    end
end
else
    msgID = 'loadEquationParams:FileDoesNotExist';
    msg = ['FileDoesNotExist: ' path];
    throw(MException(msgID,msg));
end
end