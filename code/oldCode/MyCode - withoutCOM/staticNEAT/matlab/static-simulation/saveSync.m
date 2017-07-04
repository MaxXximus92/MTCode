function [] = saveSync( path, var ,varName)
done = false;
s= [varName, ' = ', mat2str(var), ';' ];
eval(s);
while done == false
    try
        save(path,varName); 
        done= true;
    catch ME
        done=false;
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end




