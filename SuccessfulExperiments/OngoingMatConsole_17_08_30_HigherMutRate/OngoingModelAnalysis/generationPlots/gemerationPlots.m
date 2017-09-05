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
figure();
plot(meanfit,'b')
ylim([80,135])
xlim([0,450])
ylabel('mean fitness')
xlabel('generation')
axis  square

figure();
plot(bestfit,'b')
ylim([120,135])
xlim([0,450])
ylabel('best fitness')
xlabel('generation')
axis square

figure();
plot(complexity,'b')
xlim([0,450])
ylabel('mean complexity')
xlabel('generation')
axis  square
