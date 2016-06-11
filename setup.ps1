Import-Module WebAdministration
Sleep 2

Function Create-SslSite
{
	Param([string]$Name, [int]$Port, [string]$AppPool, [string]$Cert)

	$relativePath = ".\" + $Name
	$sitePath = Resolve-Path $relativePath
	$bindingPath = "IIS:\SslBindings\0.0.0.0!" + $Port

	Write-Host "Creating site $Name" -ForegroundColor Green
	New-Website -Name $Name -PhysicalPath $sitePath -ApplicationPool $appPool -Port $Port -Ssl

	if (Test-Path -LiteralPath $bindingPath)
	{
		Write-Host "Removing existing SSL binding" -ForegroundColor Green
		Remove-Item -Path $bindingPath
	}

	Write-Host "Creating SSL binding for site $Name" -ForegroundColor Green
	Get-Item $Cert | New-Item -Path $bindingPath -Force
}

Write-Host "Cleaning and building solution" -ForegroundColor Green
$msbuild = "& 'C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe'"
$buildOptions = "/noconsolelogger /p:Configuration=Debug"
$clean = $msbuild + " ""OrleansPOC.sln"" " + $options + " /t:Clean"
$build = $msbuild + " ""OrleansPOC.sln"" " + $options + " /t:Build"
Invoke-Expression $clean
Invoke-Expression $build

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

Create-SslSite -Name "Client" -Port 44300 -AppPool $appPoolName -Cert $pfxStorePath
Create-SslSite -Name "Identity" -Port 44301 -AppPool $appPoolName -Cert $pfxStorePath