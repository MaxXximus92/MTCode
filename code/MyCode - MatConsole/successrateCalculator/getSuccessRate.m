%get sucessrate
startflag= 'Run Experiment';
startReached= false;
endFlag = 'Evolution Done';
sucessThr= 0.3;

fid = fopen('consoleLogStatic_RMSDFit_17_07_10_minVariance.txt');

tline = fgets(fid);
lineCounter =1;
runsCounter =0;
sucessfulCounter=0;
generationCounter=0;
while ischar(tline)
    if ~startReached
        if contains(tline,startflag),
            startReached=true; end
    else
        %disp(tline)
        a= strsplit(tline,' ');
        index =find(strcmp(a,'fitness'))+1;
        if length(index)==1 && ~contains(a(index-2),'est') % cases 'best fitness' 'Best Fitness'
            fitness =str2double(a(index));
            runsCounter=runsCounter+1;
            generationCounter=generationCounter +1;
            if fitness >= sucessThr,
                sucessfulCounter=sucessfulCounter+1;end
        end
        if length(index)>1
            error('2 times fitness in 1 line');
        end
    end
    if contains(tline,endFlag), break; end;
    %if contains(tline,'Generation:')
   % disp(lineCounter);    
    %disp(generationCounter);
   % generationCounter=0;
   % end;
    
    tline = fgets(fid);
    lineCounter = lineCounter+1;
end
fclose(fid);
runsCounter
sucessfulCounter

sucessRate = sucessfulCounter/runsCounter

vv=1;