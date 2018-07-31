#!/bin/bash
for file in $(ls *.yml);  do
	docker pull $(grep -h "image:" $file | cut --delimiter=: -f 2-)
done
