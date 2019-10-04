#!/bin/bash

if [ -d "Minor" ]; then
	echo "Deleting folder Minor"
	rm -rf Minor
fi

echo "Branch to Use: "

read branchname

git clone --single-branch --branch $branchname --recurse-submodules https://github.com/ByteChkR/Minor
