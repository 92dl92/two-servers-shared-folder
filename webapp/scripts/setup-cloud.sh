#!/bin/bash
# Installs the .NET 10 SDK in a Claude Code cloud session (Ubuntu 24.04).
# Safe to run repeatedly: does nothing if a .NET 10 SDK is already installed.
# Uses Ubuntu's own packages because Microsoft's download host is blocked by the
# cloud environment's default network policy.
set -euo pipefail

if command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks | grep -q '^10\.'; then
  echo ".NET 10 SDK already installed."
  exit 0
fi

apt-get update -qq
apt-get install -y -qq dotnet-sdk-10.0
dotnet --list-sdks
