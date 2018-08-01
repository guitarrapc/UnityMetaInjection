#!/bin/bash

dotnet publish -c Release
find UnityMetaInjection/bin/release/* -type d -name publish | xargs -t -I{} tar zcfv {}/../UnityMetaInjection.tar.gz {}