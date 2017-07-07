function [  ] = writeSync( path, message )
while true
    try
        fid=fopen(path,'w'); % w for overwrite a for append
        fprintf(fid,message);
        fclose(fid);
        return
    catch ME
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end

