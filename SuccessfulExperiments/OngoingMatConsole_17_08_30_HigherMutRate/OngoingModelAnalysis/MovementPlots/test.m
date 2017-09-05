
f = @(x) (2/ (1 + exp(-4.9 * x))) - 1
g = @(x) 2*x
figure()
hold on
fplot(f,[-0.5,0.5])
fplot(g,[-0.5,0.5])
fplot(f,[-1,1])
fplot(g,[-1,1])