function [] = saveDEsNumSync( path, dEsNum)

%s= [varName, ' = ', mat2str(var), ';' ];
%eval(s);
while true
    try
        save(path,'dEsNum'); 
        return
    catch ME
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end




