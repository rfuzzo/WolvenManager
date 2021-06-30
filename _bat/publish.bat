@echo off

cd ..\

@RD /S /Q .\publish\

echo "dotnet publish"
dotnet publish .\WolvenManager.UI\WolvenManager.UI.csproj -o publish\full --no-self-contained -r win-x64 -c Release -f net5.0-windows

echo "publish completed"