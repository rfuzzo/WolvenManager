@echo off

@RD /S /Q .\publish\singlefile
dotnet publish .\WolvenManager.UI\WolvenManager.UI.csproj -o publish\singlefile -p:PublishSingleFile=true --no-self-contained -r win-x64 -c Release -f net5.0-windows

echo "publish singlefile completed"