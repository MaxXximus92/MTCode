function [ resultstruct ] = loadSync( path )
done = false;
if(exist(path,'file')==2)
while done == false
    try
        resultstruct = load(path);
        done = true;
    catch ME
        done=false;
        fprintf('Error while reading from %s  \n with message %s \n trying again \n',path,ME.message);
    end
end
else
    msgID = 'loadEquationParams:FileDoesNotExist';
    msg = ['FileDoesNotExist: ' path];
    throw(MException(msgID,msg));
end
end