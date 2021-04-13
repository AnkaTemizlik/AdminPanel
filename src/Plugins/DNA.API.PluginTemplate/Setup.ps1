
# Set-ExecutionPolicy -Scope CurrentUser
# ExecutionPolicy: RemoteSigned

param([string]$pluginIndex, [string]$projectName )

Set-ItemProperty $pluginIndex -name IsReadOnly -value $false


[string]$pluginCode = "P03"
[string]$appId = "NOUEsaFy"

# Plugin Code güncelle
[string]$pluginSearh = "import Plugin from '\./\w+'"
[string]$pluginReplace = [string]::Format("import Plugin from './{0}'", $pluginCode) 
(Get-Content $pluginIndex -Encoding "UTF8") | Foreach-Object {$_ -replace $pluginSearh, $pluginReplace} | Out-File $pluginIndex "UTF8"

[string]$findAppIdLine = 'const AppId = "\w+"'
[string]$newAppIdLine = [string]::Format('const AppId = "{0}"', $appId)
(Get-Content $pluginIndex -Encoding "UTF8") | Foreach-Object {$_ -replace $findAppIdLine, $newAppIdLine} | Out-File $pluginIndex "UTF8"
