[CmdletBinding(PositionalBinding = $false)]
Param(
    [switch] $release
)

if ($release) {
    $aarSourceName = "repository-release.aar"
} else {
    $aarSourceName = "repository-debug.aar"
}
$javaProjectRoot = [IO.Path]::Combine($PSScriptRoot, "java")
$aarSource = [IO.Path]::Combine($javaProjectRoot, "repository", "build", "outputs", "aar", $aarSourceName)
$javaInteropRoot = [IO.Path]::Combine($PSScriptRoot, "Repository.JavaInterop")
$aarDestination = [IO.Path]::Combine($javaInteropRoot, "Jars", "repository.aar")

pushd $javaProjectRoot

if ($release) {
    .\gradlew assembleRelease
} else {
    .\gradlew assembleDebug
}
if (! $?) {
    exit $LastExitCode
}

New-Item -Force -ItemType Directory -Path $(Split-Path $aarDestination) | Out-Null
cp $aarSource $aarDestination -Force
$time = Get-Date -Format "hh:mm:ss tt"
echo "Successfully copied $aarSourceName to $aarDestination at $time."

popd
