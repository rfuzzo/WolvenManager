@echo off

call publish.bat

echo "create manifest exe"
dotnet publish .\WolvenKit\WolvenManager.Installer\WolvenManager.Installer.csproj -o .\ -p:PublishSingleFile=true --no-self-contained -r win-x64 -c Release -f net5.0

echo "create assets"
.\WolvenManager.Installer.exe create -a .\publish\full\WolvenManager.UI.exe -o .\publish

echo "Inno Setup ..."
.\InnoSetup\ISCC\ISCC.exe /O"publish" /dMyAppBaseDir="..\publish\full\" .\InnoSetup\installer.iss

echo "create manifest"
.\WolvenManager.Installer.exe manifest -a .\publish\full\WolvenManager.UI.exe -i .\publish -o .\publish

echo "fullpublish completed"