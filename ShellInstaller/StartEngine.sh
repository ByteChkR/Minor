#!/bin/bash

if [ ! -d "Minor" ]; then
  /bin/bash EngineInstaller.sh
fi

cd Minor
/bin/bash start_engine.sh