function [] = saveFitnessSync( path, fitness)
done = false;
%s= [varName, ' = ', mat2str(var), ';' ];
%eval(s);
while done == false
    try
        save(path,'fitness'); 
        done= true;
    catch ME
        done=false;
        fprintf('Error while wrting to %s  \n with message %s \n',path,ME.message);
    end
end
end