Import-Module "sqlps" -DisableNameChecking
Import-Module WebAdministration
sleep 2

Function Create-Ssl-Site
{
	Param([string]$Name, [int]$Port, [string]$AppPool, [string]$Cert)

	$relativePath = ".\" + $Name
	$sitePath = Resolve-Path $relativePath
	$bindingPath = "IIS:\SslBindings\0.0.0.0!" + $Port

	Write-Host "Creating site $Name" -ForegroundColor Green
	New-Website -Name $Name -PhysicalPath $sitePath -ApplicationPool $appPool -Port $Port -Ssl

	Write-Host "Creating SSL binding for site $Name" -ForegroundColor Green
	Get-Item $Cert | New-Item -Path $bindingPath -Force
}

Write-Host "Importing certificate" -ForegroundColor Green
$certPath = Resolve-Path ".\localhost.pfx"
$pwd = ConvertTo-SecureString -String "Testing123" -Force –AsPlainText
Import-PfxCertificate -FilePath $certPath -Password $pwd -CertStoreLocation cert:\LocalMachine\Root
$pfx = Import-PfxCertificate -FilePath $certPath -Password $pwd -CertStoreLocation cert:\LocalMachine\My
$pfxStorePath = "cert:\LocalMachine\My\" + $pfx.Thumbprint

Write-Host "`nCreating app pool`n" -ForegroundColor Green
$appPoolName = "OrleansPOC"
$appPool = New-WebAppPool $appPoolName
$appPool.managedRuntimeVersion = "v4.0"
$appPool | Set-Item

Create-Ssl-Site -Name "Client" -Port 44300 -AppPool $appPoolName -Cert $pfxStorePath
Create-Ssl-Site -Name "Identity" -Port 44301 -AppPool $appPoolName -Cert $pfxStorePath
Create-Ssl-Site -Name "Api" -Port 44302 -AppPool $appPoolName -Cert $pfxStorePath

$orleansScriptPath = Resolve-Path ".\packages\Microsoft.Orleans.OrleansSqlUtils.1.2.1\lib\net451\SQLServer\CreateOrleansTables_SqlServer.sql"
$sqlInstanceName = $env:COMPUTERNAME + "\SQLEXPRESS"

Write-Host "Creating database" -ForegroundColor Green
Invoke-SqlCmd -Query "CREATE DATABASE Orleans;" -ServerInstance $sqlInstanceName

Write-Host "Creating database schema" -ForegroundColor Green
Invoke-SqlCmd -InputFile $orleansScriptPath -ServerInstance $sqlInstanceName -Database "Orleans"

Write-Host "Creating login and granting rights" -ForegroundColor Green
Invoke-SqlCmd -Query "CREATE LOGIN [IIS APPPOOL\OrleansPOC] FROM WINDOWS;" -ServerInstance $sqlInstanceName
Invoke-SqlCmd -Query "USE Orleans; CREATE USER OrleansPOC FOR LOGIN [IIS APPPOOL\OrleansPOC];" -ServerInstance $sqlInstanceName
Invoke-SqlCmd -Query "USE Orleans; EXEC sp_addrolemember 'db_datareader', 'OrleansPOC'" -ServerInstance $sqlInstanceName
Invoke-SqlCmd -Query "USE Orleans; EXEC sp_addrolemember 'db_datawriter', 'OrleansPOC'" -ServerInstance $sqlInstanceName