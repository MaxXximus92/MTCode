function [ message ] = readSync( path )
done = false;
while done == false
    try
        message = fileread(path);
        done = true;
    catch ME
        done=false;
        fprintf('Error while reading from %s  \n with message %s \n',path,ME.message);
    end
end
end

