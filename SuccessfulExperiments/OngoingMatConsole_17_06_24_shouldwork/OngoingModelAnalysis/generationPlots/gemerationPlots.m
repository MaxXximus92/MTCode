s= dir('data')
set(0,'DefaulttextInterpreter','none')

files= [];
for j = 1:length(s);
    files = [files {s(j).name}];
end
files=natsortfiles(files);
meanfit=[];
bestfit=[];
complexity=[];

for j = 1:length(files);
    i=files{j};
    if(length(i)>5)
        text=fileread(['data/' i]);
        a=strsplit(text,{'\n' '='},'CollapseDelimiters',true);
        meanfit=[meanfit str2double(a(4))];
        bestfit=[bestfit str2double(a(6))];
        complexity=[complexity str2double(a(10))];
    end
    
end
meanfit= meanfit-45;
bestfit = bestfit-45;
figure();
x0=10;
y0=10;
width=600;
height=600;
set(gcf,'units','points','position',[x0,y0,width,height])
plot(meanfit,'b')
ylim([100,135])
xlim([0,450])
ylabel('mean fitness')
xlabel('generation')
axis  square

figure();
x0=10;
y0=10;
width=600;
height=600;
set(gcf,'units','points','position',[x0,y0,width,height])
plot(bestfit,'b')
ylim([120,135])
xlim([0,450])
ylabel('best fitness')
xlabel('generation')
axis square

figure();
x0=10;
y0=10;
width=600;
height=600;
set(gcf,'units','points','position',[x0,y0,width,height])
plot(complexity,'b')
xlim([0,450])
ylabel('mean complexity')
xlabel('generation')
axis  square
