#!/usr/bin/env pwsh

# build
dotnet publish -c Release

# Compress
Get-ChildItem -Recurse -Directory | 
    Where-Object name -eq publish | 
    Where-Object {$_.FullName.Split("\").Split("/") -contains "release"} |
    Select-Object -First 1 |
    ForEach-Object {Compress-Archive -Path "$($_.FullName)\*" -DestinationPath "$($_.Parent.FullName)\UnityMetaInjection" -Force}
