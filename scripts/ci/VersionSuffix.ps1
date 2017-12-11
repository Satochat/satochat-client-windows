param($Stable = $false)

$versionSuffix = ""
$buildNumber = $env:APPVEYOR_BUILD_NUMBER
$branch = $env:APPVEYOR_REPO_BRANCH -replace "/", "-"
$gitCommit = git describe --long --dirty --always

if ($Stable -ne "stable") {
    $versionSuffix = "ci.$buildNumber.$branch.$gitCommit"
}

if ($versionSuffix) {
    return $versionSuffix
}
