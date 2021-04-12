
# Set-ExecutionPolicy -Scope CurrentUser
# ExecutionPolicy: RemoteSigned

param([string]$pluginIndex, [string]$appsettings, [string]$projectName )

Set-ItemProperty $pluginIndex -name IsReadOnly -value $false
Set-ItemProperty $appsettings -name IsReadOnly -value $false

[string]$pluginCode = "P03"
[string]$appId = "NOUEsaFy"

# Assembly Name güncelle
[string]$findLine =  '"Assembly": "\w+.dll"'
[string]$newLine = [string]::Format('"Assembly": "{0}.dll"', $projectName)
(Get-Content $appsettings -Encoding "UTF8") | Foreach-Object {$_ -replace $findLine, $newLine } | Out-File $appsettings "UTF8"

# App Id güncelle
[string]$findLine =  '"WEB": "\w+"'
[string]$newLine = [string]::Format('"WEB": "{0}"', $appId)
(Get-Content $appsettings -Encoding "UTF8") | Foreach-Object {$_ -replace $findLine, $newLine } | Out-File $appsettings "UTF8"

# Plugin Code güncelle
[string]$pluginSearh = "import Plugin from '\./\w+'"
[string]$pluginReplace = [string]::Format("import Plugin from './{0}'", $pluginCode) 
(Get-Content $pluginIndex -Encoding "UTF8") | Foreach-Object {$_ -replace $pluginSearh, $pluginReplace} | Out-File $pluginIndex "UTF8"

[string]$findAppIdLine = 'const AppId = "\w+"'
[string]$newAppIdLine = [string]::Format('const AppId = "{0}"', $appId)
(Get-Content $pluginIndex -Encoding "UTF8") | Foreach-Object {$_ -replace $findAppIdLine, $newAppIdLine} | Out-File $pluginIndex "UTF8"
