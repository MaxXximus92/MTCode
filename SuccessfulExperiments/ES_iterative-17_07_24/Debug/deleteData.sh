#!/bin/bash
# Delete all generated Data
echo -n "Sure to delete all generated data y/n ?"
read answer

if [ "$answer" == "y" ]; then
echo "OK going to delete"
rm -r communication/*
rm -r generationInfo/*
rm -r genomeImages/*
rm -r genomes/*
rm -r matlabSave/*
echo "deletion done"
else
echo "Chanceling"
fi
