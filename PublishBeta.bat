@echo off
echo "Publish Windows X64"
dotnet publish -c Release -r win-x64
echo "Publish Linux X64"
dotnet publish -c Release -r linux-x64