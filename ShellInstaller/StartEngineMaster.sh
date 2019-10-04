#!/bin/bash

if [ ! -d "Minor" ]; then
  /bin/bash Install_Master.sh
fi

cd Minor
/bin/bash start_engine.sh