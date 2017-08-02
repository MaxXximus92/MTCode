function [ scaledArray ] = scaleRep( array, scaleFactor )
%scaleRep 
%Repeats elements according to the scaleFactor. Leads to
%floor(length(array)*scale) elements
%   

cv=0;
repVec= zeros(floor(length(array)),1);
for i = 1:length(array)
cv =cv+scaleFactor;
repVec(i)= floor(cv);
cv= cv-floor(cv);
end
scaledArray=repelem(array,repVec);
end

