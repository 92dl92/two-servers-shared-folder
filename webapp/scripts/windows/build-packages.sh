#!/bin/bash
# Builds the packages for testing on Windows servers without ssh (phase 6c;
# docs/strategies/StressToolWindows.md), self-contained so they need no .NET on the machine:
#
#   webapp/scripts/windows/build-packages.sh [output folder]     (default: dist/)
#
#   claudetest-server-win-x64.zip   the Server, for ClaudeTestServer.ps1 Install on each server
#   claudetest-stress-win-x64.zip   the stress tool, for the PC that runs the clients and checks
#   ClaudeTestServer.ps1            the script that installs, starts and stops the server
#
# Each zip holds commit.txt, the commit it was built from (with -dirty for uncommitted changes).
set -euo pipefail

repo=$(cd "$(dirname "$0")/../../.." && pwd)
out=$(realpath -m "${1:-$repo/dist}")
work=$(mktemp -d)
trap 'rm -rf "$work"' EXIT

commit=$(git -C "$repo" rev-parse HEAD)
[[ -z $(git -C "$repo" status --porcelain) ]] || commit="$commit-dirty"
mkdir -p "$out"

publish() { # <project> <zip name>
  local dir="$work/$2"
  dotnet publish "$repo/webapp/$1" -c Release -r win-x64 --self-contained -o "$dir" -nologo -v q \
    -p:SourceRevisionId="$commit"
  echo "$commit" > "$dir/commit.txt"
  rm -f "$out/$2.zip"
  (cd "$dir" && python3 -m zipfile -c "$out/$2.zip" .)
  echo "Built $out/$2.zip"
}

publish src/BlazorApp.Server claudetest-server-win-x64
publish tools/BlazorApp.Stress claudetest-stress-win-x64
cp "$repo/webapp/scripts/windows/ClaudeTestServer.ps1" "$out/"
echo "Commit $commit. Copy the server zip and ClaudeTestServer.ps1 to each server, the stress zip to the PC."
