function [  ] = writeSync( path, message,permission)
done = false;
while done == false
    try
        fid=fopen(path,permission); % w for overwrite a for append
        fprintf(fid,message);
        fclose(fid);
        done = true;
    catch ME
        done=false;
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end

