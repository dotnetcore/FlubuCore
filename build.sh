#!/usr/bin/env bash

#exit if any command fails
set -e

dotnet restore buildscript.csproj

dotnet flubu rebuild.linux