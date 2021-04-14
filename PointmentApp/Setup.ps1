
# Set-ExecutionPolicy -Scope CurrentUser
# ExecutionPolicy: RemoteSigned

param([string]$pluginIndex, [string]$projectName )

[string]$pluginCode = "P00"
[string]$appId = "DEpLhtjc"

[string]$rootIndex = [string]::Format("{0}\\index.js", $pluginIndex, $pluginCode) 
[string]$pluginFile = [string]::Format("{0}\\{1}\\index.js", $pluginIndex, $pluginCode) 

# Plugin Code güncelle
[string]$pluginSearh = "import Plugin from '\./\w+'"
[string]$pluginReplace = [string]::Format("import Plugin from './{0}'", $pluginCode) 
(Get-Content $rootIndex -Encoding "UTF8") | Foreach-Object {$_ -replace $pluginSearh, $pluginReplace} | Out-File $rootIndex "UTF8"

[string]$findAppIdLine = 'AppId\: [\"]\w*[\"]'
[string]$newAppIdLine = [string]::Format('AppId: "{0}"', $appId)
(Get-Content $pluginFile -Encoding "UTF8") | Foreach-Object {$_ -replace $findAppIdLine, $newAppIdLine} | Out-File $pluginFile "UTF8"
