@echo off

call publish.bat

echo "Inno Setup ..."
.\WolvenManager.Installer\InnoSetup\ISCC\ISCC.exe /O"publish" /dMyAppBaseDir="..\..\publish\full\" .\WolvenManager.Installer\InnoSetup\installer.iss

echo "setup completed"