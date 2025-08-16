#!/bin/bash

# Set your download URL here
DOWNLOAD_URL="https://dragonflydb.gateway.scarf.sh/latest/dragonfly-x86_64.tar.gz"

# Download the file using wget or curl. Here, we'll use wget.
wget $DOWNLOAD_URL -O latest.tar.gz

# Extract the downloaded tar.gz file
tar -zxf latest.tar.gz

# Rename the dragonfly-x86_64 file to dragonfly
mv dragonfly-x86_64 dragonfly

# Remove the tar.gz and dragonfly-x86_64 files
rm latest.tar.gz
rm dragonfly-x86_64

# Run the dragonfly command
./dragonfly --logtostderr --requirepass=xyz123 --cache_mode=true --bind localhost --port 6379 --maxmemory=4gb --snapshot_cron "*/30 * * * *" --dir logs