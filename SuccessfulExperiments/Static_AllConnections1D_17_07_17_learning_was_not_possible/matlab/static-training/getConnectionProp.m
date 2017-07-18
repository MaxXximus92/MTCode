function conProb = getConnectionProp(weights,con1, con2)
%UNTITLED Summary of this function goes here
%   Detailed explanation goes here

 a=weights(con1,con2)~=0;
 c1= sum(a(:));
 c2= length(con1)*length(con2);
 conProb = c1/c2;
end

