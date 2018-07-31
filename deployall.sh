#!/bin/bash
for file in $(ls *.yml);  do
	faas -f $file deploy
done
