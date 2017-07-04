function [ created ] = generateModel(numNeurons,spikingThreshold)
%UNTITLED Summary of this function goes here
%   Detailed explanation goes here
     net = spikenet(256,30,'settings.xls'); 
     created=true;   
end

