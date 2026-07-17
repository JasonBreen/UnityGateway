[CmdletBinding()]
param(
    [switch]$SkipDotNet,
    [string]$UnityPath,
    [switch]$RequireUnity
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Step {
    param([string]$Message)
    Write-Host "[validate] $Message"
}

function Stop-Validation {
    param([string]$Message)
    throw "UnityGateway validation failed: $Message"
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..\..'))
Push-Location $repoRoot

try {
    Write-Step "Checking repository structure at $repoRoot"
    $requiredPaths = @(
        'Assets',
        'Packages\manifest.json',
        'ProjectSettings\ProjectVersion.txt',
        'ProjectSettings\EditorBuildSettings.asset',
        'Gateway.sln'
    )

    foreach ($path in $requiredPaths) {
        if (-not (Test-Path -LiteralPath $path)) {
            Stop-Validation "Missing required path: $path"
        }
    }

    $manifest = Get-Content -Raw -LiteralPath 'Packages\manifest.json' | ConvertFrom-Json
    if ($null -eq $manifest.dependencies) {
        Stop-Validation 'Packages/manifest.json has no dependencies object.'
    }

    $dependencyNames = @($manifest.dependencies.PSObject.Properties.Name)
    if ($dependencyNames.Count -eq 0) {
        Stop-Validation 'Packages/manifest.json has an empty dependencies object.'
    }

    $localInferencePackages = @('com.unity.ai.inference', 'com.unity.sentis')
    if (-not ($localInferencePackages | Where-Object { $dependencyNames -contains $_ })) {
        Stop-Validation 'No supported local Unity inference package is declared.'
    }

    $projectVersion = Get-Content -Raw -LiteralPath 'ProjectSettings\ProjectVersion.txt'
    if ($projectVersion -notmatch '(?m)^m_EditorVersion:\s*\S+') {
        Stop-Validation 'ProjectSettings/ProjectVersion.txt does not declare m_EditorVersion.'
    }

    Write-Step 'Checking that every scene is present in build settings'
    $buildSettings = Get-Content -Raw -LiteralPath 'ProjectSettings\EditorBuildSettings.asset'
    $scenes = @(Get-ChildItem -LiteralPath 'Assets\Scenes' -Filter '*.unity' -File -Recurse -ErrorAction SilentlyContinue)
    if ($scenes.Count -eq 0) {
        Stop-Validation 'Assets/Scenes contains no Unity scenes.'
    }

    foreach ($scene in $scenes) {
        $relativeScenePath = $scene.FullName.Substring($repoRoot.Length).TrimStart('\', '/') -replace '\\', '/'
        if ($buildSettings -notmatch [regex]::Escape($relativeScenePath)) {
            Stop-Validation "Scene is missing from build settings: $relativeScenePath"
        }
    }

    Write-Step 'Checking runtime scripts for networking APIs'
    $runtimeScripts = @(Get-ChildItem -LiteralPath 'Assets\Scripts' -Filter '*.cs' -File -Recurse -ErrorAction SilentlyContinue)
    $networkPattern = 'using\s+System\.Net\.Http|UnityEngine\.Networking|\bUnityWebRequest\b|\bHttpClient\b|\bClientWebSocket\b|\bHttpWebRequest\b'
    $networkMatches = @($runtimeScripts | Select-String -Pattern $networkPattern)
    if ($networkMatches.Count -gt 0) {
        $locations = ($networkMatches | ForEach-Object { "$($_.Path):$($_.LineNumber)" }) -join ', '
        Stop-Validation "Runtime networking API detected at $locations"
    }

    if (-not $SkipDotNet) {
        if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
            Stop-Validation 'dotnet is unavailable. Install .NET 8 or rerun with -SkipDotNet.'
        }

        Write-Step 'Running .NET tests'
        & dotnet test 'Gateway.sln' --nologo
        if ($LASTEXITCODE -ne 0) {
            Stop-Validation "dotnet test exited with code $LASTEXITCODE"
        }
    }
    else {
        Write-Step 'Skipping .NET tests by request'
    }

    $resolvedUnityPath = $null
    if (-not [string]::IsNullOrWhiteSpace($UnityPath)) {
        if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) {
            Stop-Validation "Unity executable not found: $UnityPath"
        }

        $resolvedUnityPath = (Resolve-Path -LiteralPath $UnityPath).Path
    }
    else {
        $unityCommand = Get-Command Unity -ErrorAction SilentlyContinue
        if ($null -ne $unityCommand) {
            $resolvedUnityPath = $unityCommand.Source
        }
    }

    if ($null -ne $resolvedUnityPath) {
        Write-Step "Running Unity batch validation with $resolvedUnityPath"
        & $resolvedUnityPath -projectPath $repoRoot -batchmode -quit -nographics -logFile -
        if ($LASTEXITCODE -ne 0) {
            Stop-Validation "Unity batch mode exited with code $LASTEXITCODE"
        }
    }
    elseif ($RequireUnity) {
        Stop-Validation 'Unity validation was required, but no Unity executable was found.'
    }
    else {
        Write-Step 'Unity executable not found; Editor compilation was not verified'
    }

    Write-Host '[validate] PASS'
}
finally {
    Pop-Location
}
