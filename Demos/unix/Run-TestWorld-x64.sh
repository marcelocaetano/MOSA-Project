#!/bin/bash

cd $(dirname $0)

./Run-Demo.x64.sh "$@" --assembly ../../bin/Mosa.TestWorld.x64.exe
