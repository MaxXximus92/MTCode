
run_time = 40000; %runtime per trial [ms] %initial 120000
run_settings = '[65;35]'%[ones(1,5).*65;     %start angles
                %0,35,75,105,135]; %target angles
numNeurons=256;
savePath='C:\Users\Maximus Mutschler\Downloads\test';

[weightsM, eqationParams, emes]= getModelParams(numNeurons,'weightsMatrix.mat','EquationParams.mat');
emesW= ones(emes);
name = 'test';
tic
rmsd = runModel(name,numNeurons,run_time,run_settings,savePath, eqationParams, weightsM,emesW, true)
toc
