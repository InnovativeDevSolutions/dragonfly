#!/bin/bash

# Run the dragonfly command
./dragonfly --logtostderr --requirepass=xyz123 --cache_mode=true --bind localhost --port 6379 --maxmemory=4gb --snapshot_cron "*/30 * * * *" --dir logs