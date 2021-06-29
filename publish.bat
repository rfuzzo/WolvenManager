@echo off

@RD /S /Q .\publish\full
dotnet publish .\WolvenManager.UI\WolvenManager.UI.csproj -o publish\full --no-self-contained -r win-x64 -c Release -f net5.0-windows

echo "The program has completed"
@pause