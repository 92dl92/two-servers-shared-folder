#!/bin/bash
# Starts several servers sharing one data folder, for manual testing in browser tabs
# (plan section 4). Each server is its own process on its own port: 5081, 5082, ...
#
#   webapp/scripts/run-cluster.sh <servers> <strategy> [data folder]
#   webapp/scripts/run-cluster.sh 3 PerRecordFile
#
# Without a data folder, a new run folder is made under manual-run/ (gitignored) holding
# data/ and logs/; each server writes logs/server-<port>.jsonl and server-<port>.console.txt
# (the stress tool's names). Ctrl-C stops every server.
# Tip: browsers keep cookies per host, not per port, so use a private window (or
# localhost vs 127.0.0.1) to act as two different users at once.
set -euo pipefail

if [[ $# -lt 2 || ! "$1" =~ ^[1-9][0-9]*$ ]]; then
  echo "Usage: $0 <servers> <strategy> [data folder]" >&2
  echo "Strategies: InMemory, NaiveFile, LockedFile, PerRecordFile, AppendLog, Sqlite" >&2
  exit 2
fi

servers=$1
strategy=$2
repo=$(cd "$(dirname "$0")/../.." && pwd)
run="$repo/manual-run/$(date -u +%Y%m%d-%H%M%S)-$strategy"
data=$(realpath -m "${3:-$run/data}")
logs="$run/logs"
mkdir -p "$data" "$logs"

# A port already in use would let an old server answer for a new one that failed to start.
for ((i = 0; i < servers; i++)); do
  port=$((5081 + i))
  if curl -s -o /dev/null --max-time 2 "http://127.0.0.1:$port/"; then
    echo "Port $port is already in use (another run-cluster.sh or server?); stop it first." >&2
    exit 1
  fi
done

if ! build=$(dotnet build "$repo/webapp/src/BlazorApp.Server" -v q -nologo 2>&1); then
  echo "$build" >&2
  echo "Build failed." >&2
  exit 1
fi

pids=()
stop() {
  for pid in "${pids[@]}"; do kill "$pid" 2>/dev/null || true; done
  wait 2>/dev/null || true
  echo "Stopped. Logs: $logs"
}
# Clean up once, on any exit; a signal just exits (130/143), which runs the EXIT trap.
trap stop EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

# From the Server's project folder, so users.json and the static files are found.
cd "$repo/webapp/src/BlazorApp.Server"
for ((i = 0; i < servers; i++)); do
  port=$((5081 + i))
  ASPNETCORE_ENVIRONMENT=Development dotnet bin/Debug/net10.0/BlazorApp.Server.dll \
    --urls "http://127.0.0.1:$port" \
    --Storage:Strategy "$strategy" --Storage:Path "$data" \
    --Logging:File:Directory "$logs" \
    >"$logs/server-$port.console.txt" 2>&1 &
  pids+=($!)
done

# Wait until every server answers, at most 60 s each; a server that exits (e.g. an unknown
# strategy) stops the run. The process is checked first, so only our own server can answer.
for ((i = 0; i < servers; i++)); do
  port=$((5081 + i))
  deadline=$((SECONDS + 60))
  while true; do
    if ! kill -0 "${pids[$i]}" 2>/dev/null; then
      echo "Server on port $port exited; see $logs/server-$port.console.txt:" >&2
      tail -5 "$logs/server-$port.console.txt" >&2
      exit 1
    fi
    curl -sf --max-time 2 "http://127.0.0.1:$port/api/users" >/dev/null && break
    if ((SECONDS >= deadline)); then
      echo "Server on port $port did not answer within 60 s; see $logs/server-$port.console.txt." >&2
      exit 1
    fi
    sleep 0.5
  done
  echo "Server $((i + 1)): http://127.0.0.1:$port"
done

echo "Strategy $strategy, data $data, logs $logs. Ctrl-C to stop."
wait
