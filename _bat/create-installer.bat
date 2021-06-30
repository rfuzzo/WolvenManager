@echo off

call publish.bat

echo "Inno Setup ..."
.\InnoSetup\ISCC\ISCC.exe /O"publish" /dMyAppBaseDir="..\publish\full\" .\InnoSetup\installer.iss

echo "setup completed"