# Prepares a Windows Server machine to run stress-test servers started by the stress tool over
# ssh (docs/handover-solution-c.md, "Recreating the test harness"). Run once, as an
# administrator, in Windows PowerShell 5.1:
#
#   powershell -ExecutionPolicy Bypass -File prepare-test-server.ps1 `
#     -AuthorizedKey 'ssh-ed25519 AAAA... you@driver' -DriverAddress 10.0.0.5
#
# It turns on the OpenSSH server with PowerShell as its shell, lets the given key log in as an
# administrator, installs the ASP.NET Core 10 runtime, and allows inbound connections to
# dotnet.exe from the driver machine only. Safe to run again.
# Adapted from the unattended setup of the phase 6b test VMs; this parameterised version has not
# itself been run yet.
param(
    [Parameter(Mandatory)] [string] $AuthorizedKey,
    [Parameter(Mandatory)] [string] $DriverAddress,
    [string] $DotnetDir = 'C:\dotnet'
)
$ErrorActionPreference = 'Stop'

# OpenSSH server (included but disabled on Server 2025; an optional feature on older versions)
if (-not (Get-Service sshd -ErrorAction SilentlyContinue)) {
    Add-WindowsCapability -Online -Name OpenSSH.Server~~~~0.0.1.0 | Out-Null
}
Set-Service sshd -StartupType Automatic
Start-Service sshd

# The stress tool sends every command as `powershell -EncodedCommand`, so PowerShell must be
# the ssh server's default shell.
New-ItemProperty HKLM:\SOFTWARE\OpenSSH -Name DefaultShell -PropertyType String -Force `
    -Value "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe" | Out-Null

# Key login for administrators uses this one file, readable only by Administrators and SYSTEM.
$keys = "$env:ProgramData\ssh\administrators_authorized_keys"
if (-not ((Test-Path $keys) -and (Select-String -Path $keys -SimpleMatch $AuthorizedKey -Quiet))) {
    Add-Content -Path $keys -Value $AuthorizedKey -Encoding ascii
}
icacls $keys /inheritance:r /grant 'Administrators:F' /grant 'SYSTEM:F' | Out-Null
Restart-Service sshd

# ASP.NET Core 10 runtime: the stress tool copies a framework-dependent build of the Server.
if (-not (Test-Path "$DotnetDir\shared\Microsoft.AspNetCore.App\10.*")) {
    [Net.ServicePointManager]::SecurityProtocol = 'Tls12'
    $installer = Join-Path $env:TEMP 'dotnet-install.ps1'
    Invoke-WebRequest https://dot.net/v1/dotnet-install.ps1 -OutFile $installer -UseBasicParsing
    & $installer -Channel 10.0 -Runtime aspnetcore -InstallDir $DotnetDir
}
$path = [Environment]::GetEnvironmentVariable('Path', 'Machine')
if (($path -split ';') -notcontains $DotnetDir) {
    [Environment]::SetEnvironmentVariable('Path', "$path;$DotnetDir", 'Machine')
}
[Environment]::SetEnvironmentVariable('DOTNET_ROOT', $DotnetDir, 'Machine')

# Each server listens on a port the stress tool picks, so allow the program rather than ports,
# and only from the driver machine.
$rule = 'ClaudeTest stress servers'
Get-NetFirewallRule -DisplayName $rule -ErrorAction SilentlyContinue | Remove-NetFirewallRule
New-NetFirewallRule -DisplayName $rule -Direction Inbound -Action Allow -Protocol TCP `
    -Program "$DotnetDir\dotnet.exe" -RemoteAddress $DriverAddress | Out-Null

# Restart-Service above doesn't pick up the new machine Path for later ssh sessions until sshd
# restarts once more after it is set.
Restart-Service sshd

New-Item -ItemType Directory -Force (Join-Path $HOME 'claudetest-stress') | Out-Null
& "$DotnetDir\dotnet.exe" --list-runtimes
Write-Output "Ready. Next: put the share's credentials in $HOME\claudetest-stress\connect.ps1 (see the handover)."
