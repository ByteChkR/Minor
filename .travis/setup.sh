#!/bin/bash

if [[ $TRAVIS_OS_NAME == 'linux' ]]; then

    # Install some custom requirements on Linux
    echo "Setting up Ubuntu OpenCL Prerequisites"
	
	sudo apt-get install -qq fglrx=2:8.960-0ubuntu1 opencl-headers
    
    #sudo apt-get -qq update
    #sudo apt-get install pocl
fi