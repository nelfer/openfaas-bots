#!/bin/bash
for file in $(ls *.yml);  do
	faas -f $file push
done
