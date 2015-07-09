if($args.Count -lt 1){
    throw "Need at least one argument!"
}

[System.Environment]::CurrentDirectory = $pwd

function use-var([IDisposable]$instance, [ScriptBlock]$script){
    try{
        &$script
    }finally{
        $instance.Dispose();
    }
}

$nuspec = [string]$args[0]
if([System.IO.Path]::GetExtension($nuspec).ToUpperInvariant() -ne ".NUSPEC"){
    $nuspec += ".nuspec"
}

if(!(Test-Path $nuspec)){
    throw "File """ + $nuspec + """ is not found!"
}

"Updating: " + $nuspec

if($args.Count -lt 2){
    $list = @($args[0])
}else{
    $list = [Array]$args[1..($args.Count-1)]
}

$parts = 0,0,0,0

$exts = ".DLL", ".EXE"
foreach($item in $list){
    $src = [string]$item
    $ext = [System.IO.Path]::GetExtension($nuspec).ToUpperInvariant()
    if(!$exts.Contains($ext)){
        $src += ".dll"
    }

    if(!(Test-Path $src)){
        "File """ + $src + """ is not found!"
        continue
    }

    $file = gi $src
    $vi = [System.Diagnostics.FileVersionInfo]$file.VersionInfo

    "- " + $src + " - " + $vi.FileVersion

    if($parts[0] -lt $vi.FileMajorPart){ 
        $parts = $vi.FileMajorPart, $vi.FileMinorPart, $vi.FileBuildPart, $vi.FilePrivatePart
        continue 
    }
    if($parts[0] -gt $vi.FileMajorPart){ continue }

    if($parts[1] -lt $vi.FileMinorPart){ 
        $parts = $vi.FileMajorPart, $vi.FileMinorPart, $vi.FileBuildPart, $vi.FilePrivatePart
        continue 
    }
    if($parts[1] -gt $vi.FileMinorPart){ continue }

    if($parts[2] -lt $vi.FileBuildPart){ 
        $parts = $vi.FileMajorPart, $vi.FileMinorPart, $vi.FileBuildPart, $vi.FilePrivatePart
        continue 
    }
    if($parts[2] -gt $vi.FileBuildPart){ continue }

    if($parts[3] -lt $vi.FilePrivatePart){ 
        $parts = $vi.FileMajorPart, $vi.FileMinorPart, $vi.FileBuildPart, $vi.FilePrivatePart
    }
}

$ver = [string]::Join(".", $parts)
"Final Version: " + $nuspec + " - " + $ver

$xs = New-Object System.Xml.XmlWriterSettings
$xs.Indent = $true
$xs.IndentChars = "    "
$xml = [xml](Get-Content $nuspec)
$meta = $xml.package.metadata
$nupkg = $meta.id + "." + $ver + ".nupkg"
$meta.version = $ver

use-var($xw = [System.Xml.XmlWriter]::Create($nuspec, $xs)){
    $xml.Save($xw)
}

"Updating Acomplished."

&nuget pack "$nuspec" -Verbosity detailed -OutputDirectory "nupkg"

if(Test-Path Env:LocalNupkg){
    if(-not (Test-Path $Env:LocalNupkg)){
        md $Env:LocalNupkg
    }
    copy ("nupkg\" + $nupkg) $Env:LocalNupkg
}