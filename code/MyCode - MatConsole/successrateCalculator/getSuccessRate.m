%get sucessrate
startflag= 'Run Experiment';
startReached= false;
endFlag = 'Evolution Done';
sucessThr= 0.3;

fid = fopen('consoleLog_ES_Iterative.txt');

tline = fgets(fid);
lineCounter =1;
runsCounter =0;
sucessfulCounter=0;
generationCounter=0;
sumedFitness=0;
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
                sucessfulCounter=sucessfulCounter+1;
                sumedFitness= sumedFitness+fitness;
            end
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
sumedFitness;

sucessRate = sucessfulCounter/runsCounter
meanFitness = sumedFitness/sucessfulCounter
%0.3 zu 0.7
approxRmsd = 135-(meanFitness -0.3)/0.7*135
%balanced
%approxRmsd = 135-(meanFitness -2/3)/(1/3)*135
%ongoing
%meanFitness = meanFitness/180*130
%approxRmsd =135-meanFitness
%
vv=1;