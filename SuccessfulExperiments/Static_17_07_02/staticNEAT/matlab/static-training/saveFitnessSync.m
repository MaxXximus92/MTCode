function [] = saveFitnessSync( path, fitness)
done = false;
%s= [varName, ' = ', mat2str(var), ';' ];
%eval(s);
while true
    try
        save(path,'fitness'); 
        return
    catch ME
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end