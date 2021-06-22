
# Set-ExecutionPolicy -Scope CurrentUser
## ExecutionPolicy: RemoteSigned

param([string]$clientApp, [string]$projectName)

[string]$pluginCode = "P00"
[string]$host = "http://192.168.1.200:8800"

Write-Output "**************** Starting "
Write-Output $pluginCode 
Write-Output $host

[string]$rootIndex = [string]::Format("{0}\\src\\plugins\\index.js", $clientApp) 
[string]$axios = [string]::Format("{0}\\src\\store\\axios.js", $clientApp) 

[string]$pluginSearh = "import Plugin from '\./\w+'"
[string]$pluginReplace = [string]::Format("import Plugin from './{0}'", $pluginCode) 
(Get-Content $rootIndex -Raw) | Foreach-Object {$_ -replace $pluginSearh, $pluginReplace} | Out-File $rootIndex ASCII

[string]$findHostLine = "const Host = [\'].*[\']" 
[string]$newHostLine = [string]::Format("const Host = '{0}'", $host) 

(Get-Content $axios -Raw) | Foreach-Object {$_ -replace $findHostLine, $newHostLine} | Out-File $axios ASCII 


Write-Output "**************** Completed."