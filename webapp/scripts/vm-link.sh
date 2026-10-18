#!/usr/bin/env bash
# Cuts a libvirt VM's network for a while, then restores it (phase 6b's failure tests):
#   vm-link.sh <vm> <seconds>
# Run as a stress tool fault: --fault "run:webapp/scripts/vm-link.sh ws2 10@40".
# Needs virsh and access to qemu:///system (the libvirt group). Restores the link even if interrupted.
set -euo pipefail
vm=$1 seconds=$2
uri=qemu:///system
iface=$(virsh -c $uri domiflist "$vm" | awk 'NR > 2 && $1 != "" { print $1; exit }')
[ -n "$iface" ] || { echo "no network interface found for $vm" >&2; exit 1; }
trap 'virsh -c $uri domif-setlink "$vm" "$iface" up >/dev/null' EXIT
virsh -c $uri domif-setlink "$vm" "$iface" down >/dev/null
echo "$vm $iface down for $seconds s"
sleep "$seconds"
virsh -c $uri domif-setlink "$vm" "$iface" up >/dev/null
trap - EXIT
echo "$vm $iface up again: $(virsh -c $uri domif-getlink "$vm" "$iface" | awk '{print $NF}')"
