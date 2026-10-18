# Runs a stress-test server by hand on a Windows server that the stress tool can't reach over
# ssh (phase 6c; docs/strategies/StressToolWindows.md). Windows PowerShell 5.1; no admin rights
# needed unless -Root is somewhere only admins can write. Changes no firewall rule and installs
# no service: every port is one the owner says is already open.
#
#   .\ClaudeTestServer.ps1 Install -Package .\claudetest-server-win-x64.zip
#   .\ClaudeTestServer.ps1 Start -Strategy PerRecordFile -Data \\fs\share\stress\run1\data `
#                                -Users \\fs\share\stress\run1-users.json -Port 8080 -Name run1
#   .\ClaudeTestServer.ps1 Status
#   .\ClaudeTestServer.ps1 Stop -Name run1
#   .\ClaudeTestServer.ps1 CollectLogs -Name run1 -To \\fs\share\stress\logs
#   .\ClaudeTestServer.ps1 Uninstall
#
# The server runs as the user who starts it, so it reaches the share with that user's access.
param(
    [Parameter(Mandatory, Position = 0)]
    [ValidateSet('Install', 'Start', 'Stop', 'Status', 'CollectLogs', 'Uninstall')]
    [string] $Action,
    [string] $Root = 'C:\ClaudeTest',
    [string] $Package,
    [string] $Strategy,
    [string] $Data,
    [string] $Users,
    [int] $Port,
    [string] $Name = 'run',
    [string] $To
)
$ErrorActionPreference = 'Stop'
# A full path, so the server's path compares equal to the process's (Get-RunningServer).
$Root = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Root)
$serverDir = Join-Path $Root 'server'
$serverExe = Join-Path $serverDir 'BlazorApp.Server.exe'
$runDir = Join-Path (Join-Path $Root 'runs') $Name
$pidFile = Join-Path $runDir 'server.json'

function Require([string] $value, [string] $option) {
    if (-not $value) { throw "$Action needs -$option." }
}

function Get-RunningServer([string] $file) {
    if (-not (Test-Path $file)) { return $null }
    $info = Get-Content $file -Raw | ConvertFrom-Json
    $process = Get-Process -Id $info.pid -ErrorAction SilentlyContinue
    # A process id can be reused: only a BlazorApp.Server started from this install counts.
    if ($process -and $process.Path -eq $serverExe) { return @{ Info = $info; Process = $process } }
    return $null
}

function Stop-Run([string] $file) {
    $running = Get-RunningServer $file
    if ($running) {
        Stop-Process -Id $running.Process.Id -Force
        Wait-Process -Id $running.Process.Id -Timeout 15 -ErrorAction SilentlyContinue
        Write-Output "Stopped the server on port $($running.Info.port) (process $($running.Process.Id))."
    }
    if (Test-Path $file) { Remove-Item $file }
}

switch ($Action) {
    'Install' {
        Require $Package 'Package'
        if (Get-ChildItem (Join-Path $Root 'runs') -Filter server.json -Recurse -ErrorAction SilentlyContinue |
                Where-Object { Get-RunningServer $_.FullName }) {
            throw 'A test server from this install is running: Stop it first.'
        }
        if (Test-Path $serverDir) { Remove-Item $serverDir -Recurse -Force }
        New-Item -ItemType Directory -Force $serverDir | Out-Null
        Expand-Archive -Path $Package -DestinationPath $serverDir
        if (-not (Test-Path $serverExe)) { throw "The package has no BlazorApp.Server.exe; is it the self-contained win-x64 build?" }
        Write-Output "Installed in $serverDir$(if (Test-Path (Join-Path $serverDir 'commit.txt')) { ', commit ' + (Get-Content (Join-Path $serverDir 'commit.txt')) })."
    }

    'Start' {
        Require $Strategy 'Strategy'; Require $Data 'Data'; Require $Users 'Users'
        if (-not $Port) { throw 'Start needs -Port: one the owner says is open.' }
        if (-not (Test-Path $serverExe)) { throw "Not installed: run Install first." }
        if (Get-RunningServer $pidFile) { throw "Run $Name already has a server: Stop -Name $Name first." }
        if (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue) {
            throw "Port $Port is already in use on this machine."
        }
        if (-not (Test-Path $Users)) { throw "No users file at ${Users}: run the stress tool with --prepare first." }

        $logs = Join-Path $runDir 'logs'
        New-Item -ItemType Directory -Force $logs | Out-Null
        $console = Join-Path $runDir "server-$Port.console.txt"
        # Quoted for the command line; a trailing backslash would escape the closing quote.
        $Data = $Data.TrimEnd('\'); $Users = $Users.TrimEnd('\')
        $arguments = @(
            '--urls', "http://0.0.0.0:$Port",
            '--Storage:Strategy', $Strategy,
            '--Storage:Path', "`"$Data`"",
            '--Users:Path', "`"$Users`"",
            '--Logging:File:Directory', "`"$logs`"")
        # As the stress tool starts its servers: Development, API only.
        $env:ASPNETCORE_ENVIRONMENT = 'Development'
        $process = Start-Process -FilePath $serverExe -ArgumentList $arguments -WorkingDirectory $serverDir `
            -RedirectStandardOutput $console -RedirectStandardError "$console.err" -WindowStyle Hidden -PassThru
        @{ pid = $process.Id; port = $Port; strategy = $Strategy; data = $Data; users = $Users; started = (Get-Date).ToString('o') } |
            ConvertTo-Json | Set-Content $pidFile

        # Ready when the API answers on this machine; a server that fails to start says why in its console.
        $deadline = (Get-Date).AddSeconds(60)
        while ((Get-Date) -lt $deadline) {
            if ($process.HasExited) { break }
            try {
                Invoke-WebRequest "http://localhost:$Port/api/users" -UseBasicParsing -TimeoutSec 5 | Out-Null
                Write-Output "Started: http://$($env:COMPUTERNAME):$Port (process $($process.Id), run $Name, logs in $logs)."
                Write-Output "From the PC, check the port is open: Test-NetConnection $($env:COMPUTERNAME) -Port $Port"
                return
            } catch {
                Start-Sleep -Seconds 1
            }
        }
        Stop-Run $pidFile
        $output = (Get-Content $console, "$console.err" -ErrorAction SilentlyContinue | Select-Object -Last 20) -join "`n"
        throw "The server didn't answer within 60 s. Its last output:`n$output"
    }

    'Stop' {
        if (-not (Get-RunningServer $pidFile)) { Write-Output "No server running for run $Name." }
        Stop-Run $pidFile
    }

    'Status' {
        $found = $false
        Get-ChildItem (Join-Path $Root 'runs') -Filter server.json -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
            $found = $true
            $info = Get-Content $_.FullName -Raw | ConvertFrom-Json
            $state = if (Get-RunningServer $_.FullName) { 'running' } else { 'not running' }
            Write-Output "$($_.Directory.Name): $state, port $($info.port), $($info.strategy), data $($info.data), started $($info.started)"
        }
        if (-not $found) { Write-Output 'No test servers started from this install.' }
        if (Test-Path (Join-Path $serverDir 'commit.txt')) { Write-Output "Package commit: $(Get-Content (Join-Path $serverDir 'commit.txt'))" }
    }

    'CollectLogs' {
        Require $To 'To'
        $logs = Join-Path $runDir 'logs'
        if (-not (Test-Path $logs)) { throw "No logs for run $Name." }
        New-Item -ItemType Directory -Force $To | Out-Null
        $zip = Join-Path $To "$Name-$($env:COMPUTERNAME)-logs.zip"
        Compress-Archive -Path (Join-Path $runDir '*') -DestinationPath $zip -Force
        Write-Output "Logs in $zip."
    }

    'Uninstall' {
        Get-ChildItem (Join-Path $Root 'runs') -Filter server.json -Recurse -ErrorAction SilentlyContinue |
            ForEach-Object { Stop-Run $_.FullName }
        if (Test-Path $Root) { Remove-Item $Root -Recurse -Force }
        Write-Output "Removed $Root."
    }
}
