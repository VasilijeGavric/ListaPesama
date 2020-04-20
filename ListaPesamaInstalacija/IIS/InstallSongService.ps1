Add-Type -AssemblyName System.IO.Compression.FileSystem

$SiteName = "SongService"
$HostName = ""
$SiteFolder = 'C:\SongService'
$ZipFile = 'SongService.zip'

function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Remove-Item $SiteFolder -Recurse -ErrorAction Ignore
New-Item -ItemType Directory -Force -Path $SiteFolder
Unzip "$PSScriptRoot\$ZipFile" $SiteFolder -Force
New-WebSite -Name $SiteName -PhysicalPath $SiteFolder -Force
$IISSite = "IIS:\Sites\$SiteName"
Set-ItemProperty $IISSite -name  Bindings -value @{protocol="http";bindingInformation="*:555:$HostName"}
