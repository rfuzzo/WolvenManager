@echo off

echo "Inno Setup ..."
.\WolvenManager.Installer\InnoSetup\ISCC\ISCC.exe /dMyAppBaseDir="..\..\publish\full\" .\WolvenManager.Installer\InnoSetup\installer.iss

echo "The program has completed"