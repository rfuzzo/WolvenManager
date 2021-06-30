@echo off

call create-installer.bat

echo "create manifest exe"
dotnet publish .\WolvenManager.Installer\WolvenManager.Installer.csproj -o .\ -p:PublishSingleFile=true --no-self-contained -r win-x64 -c Release -f net5.0

echo "create manifest"
@RD /S /Q .\_out
.\WolvenManager.Installer.exe manifest -a .\publish\full\WolvenManager.UI.exe -i .\publish -o .\publish

echo "create manifest completed"

echo "fullpublish completed"