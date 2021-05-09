Stop-WebAppPool '.NET Core'
Start-Sleep -s 3
dotnet publish -c Debug
Start-WebAppPool '.NET Core'
