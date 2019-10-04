#!/bin/bash

if [ ! -d "Minor" ]; then
  /bin/bash Install_Develop.sh
fi

cd Minor
/bin/bash start_engine.sh