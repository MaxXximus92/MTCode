%get sucessrate

startflag= 'Run Experiment';
startReached= false;
endFlag = 'Evolution Done';
sucessThr= 1/3;

fid = fopen('consoleLog.txt');

tline = fgets(fid);
lineCounter =1;
runsCounter =0;

fitnesses =[];
genNumbers =[];
while ischar(tline)
    if(lineCounter >= 43412 && lineCounter <= 43503)
    a= strsplit(tline,' '); 
    index =find(strcmp(a,'fitness'));
    fitness =str2double(a(index+1));
    genNumber=str2double(a(index-1));
    fitnesses = [fitnesses fitness];
    genNumbers = [genNumbers genNumber];
    end    
     tline = fgets(fid)
    lineCounter = lineCounter +1;  
end
fclose(fid);
runsCounter
sucessfulCounter

sucessRate = sucessfulCounter/runsCounter
vv=1;