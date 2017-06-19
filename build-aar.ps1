$aarName = "repository-release.aar"
$javaProjectRoot = [IO.Path]::Combine($PSScriptRoot, "java")
$aarSource = [IO.Path]::Combine($javaProjectRoot, "repository", "build", "outputs", "aar", $aarName)
$javaInteropRoot = [IO.Path]::Combine($PSScriptRoot, "Repository.JavaInterop")
$aarDestination = [IO.Path]::Combine($javaInteropRoot, "Jars")

pushd $javaProjectRoot

.\gradlew assembleRelease
if (! $?)
{
    exit $LastExitCode
}

cp $aarSource $aarDestination -Force
echo "Successfully copied $aarName to $aarDestination."

popd
