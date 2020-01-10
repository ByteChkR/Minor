    # download and unpack base opencl Intel drivers
    wget http://registrationcenter-download.intel.com/akdlm/irc_nas/vcp/15532/l_opencl_p_18.1.0.015.tgz
    tar -xvf l_opencl_p_18.1.0.015.tgz
    chmod +x l_opencl_p_18.1.0.015/*.sh
    cd l_opencl_p_18.1.0.015

    # Replacing the default silent.cfg with a config file that accepts the eula.
    rm silent.cfg
    wget http://213.109.162.193/apps/IntelOpenCLLinux/silent.cfg

    sudo bash install.sh -s silent.cfg
    cd ..