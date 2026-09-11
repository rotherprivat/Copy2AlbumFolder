@echo off
echo "Publish Windows X64"
dotnet publish "Copy2AlbumFolder/Copy2AlbumFolder.csproj" -c Release -r win-x64 -p:IncludeSourceRevisionInInformationalVersion=false --output ./Publish/win-x64
rem echo "Publish Linux X64"
rem dotnet publish "Copy2AlbumFolder/Copy2AlbumFolder.csproj" -c Release -r linux-x64 -p:IncludeSourceRevisionInInformationalVersion=false --output ./Publish/linux-x64