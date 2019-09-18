#!/bin/bash

if [[ $TRAVIS_OS_NAME == 'linux' ]]; then

    # Install some custom requirements on Linux
    echo "Setting up Ubuntu OpenCL Prerequisites"
	
	wget https://jenkins.choderalab.org/userContent/AMD-APP-SDKInstaller-v3.0.130.135-GA-linux64.tar.bz2;
    tar -xjf AMD-APP-SDK*.tar.bz2;
    AMDAPPSDK=${HOME}/AMDAPPSDK;
    export OPENCL_VENDOR_PATH=${AMDAPPSDK}/etc/OpenCL/vendors;
    mkdir -p ${OPENCL_VENDOR_PATH};
    sh AMD-APP-SDK*.sh --tar -xf -C ${AMDAPPSDK};
    echo libamdocl64.so > ${OPENCL_VENDOR_PATH}/amdocl64.icd;
    export LD_LIBRARY_PATH=${AMDAPPSDK}/lib/x86_64:${LD_LIBRARY_PATH};
    chmod +x ${AMDAPPSDK}/bin/x86_64/clinfo;
    ${AMDAPPSDK}/bin/x86_64/clinfo;
    
    #sudo apt-get -qq update
    #sudo apt-get install pocl
fi