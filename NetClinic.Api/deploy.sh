#!/usr/bin/sh

\rm -rf publish
rm -rf netclinic-api.zip
dotnet publish -c Release -o publish
cd publish && zip -r ../netclinic-api.zip . && cd ..
az webapp deploy --resource-group netclinic --name netclinic --src-path ./netclinic-api.zip --type zip --restart true
