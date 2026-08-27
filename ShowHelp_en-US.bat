@echo off
echo "Build"
dotnet publish "Copy2AlbumFolder/Copy2AlbumFolder.csproj" -c Release -r win-x64 -p:Lang=enUS  --output ./publish/win-x64-enUS

.\publish\win-x64-enUS\Copy2AlbumFolder.exe -h

pause