function [] = saveRmsdSync( path, rmsd)
done = false;
%s= [varName, ' = ', mat2str(var), ';' ];
%eval(s);%TODO quite likely to get seg fault here
while done == false
    try
        save(path,'rmsd'); 
        done= true;
    catch ME
        done=false;
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end




