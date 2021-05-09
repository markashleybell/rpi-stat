Import-Module IISAdministration
$sm = Get-IISServerManager
$pool = $sm.ApplicationPools['.NET Core']
$pool.Stop()
Start-Sleep -Second 3
dotnet publish -c Debug /p:EnvironmentName=Development
$pool.Start()
