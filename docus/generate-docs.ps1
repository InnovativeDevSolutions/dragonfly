# Script to convert existing docs/ to docus/content/
# Run this from the dragonfly root directory

$ErrorActionPreference = "Stop"

# Create directory structure
Write-Host "Creating directory structure..." -ForegroundColor Cyan
New-Item -ItemType Directory -Force -Path "docus\content\2.api\1.core" | Out-Null
New-Item -ItemType Directory -Force -Path "docus\content\2.api\2.basic" | Out-Null
New-Item -ItemType Directory -Force -Path "docus\content\2.api\3.hash" | Out-Null
New-Item -ItemType Directory -Force -Path "docus\content\2.api\4.list" | Out-Null

# Navigation files
@"
title: Core Functions
"@ | Out-File -FilePath "docus\content\2.api\1.core\.navigation.yml" -Encoding utf8

@"
title: Basic Operations
"@ | Out-File -FilePath "docus\content\2.api\2.basic\.navigation.yml" -Encoding utf8

@"
title: Hash Operations
"@ | Out-File -FilePath "docus\content\2.api\3.hash\.navigation.yml" -Encoding utf8

@"
title: List Operations
"@ | Out-File -FilePath "docus\content\2.api\4.list\.navigation.yml" -Encoding utf8

# Function to convert markdown for Docus
function Convert-DocToDocusFormat {
    param(
        [string]$Content,
        [string]$Title,
        [string]$Description
    )
    
    # Remove frontmatter if exists
    $Content = $Content -replace '(?s)^---.*?---\s*', ''
    
    # Create new frontmatter
    $newFrontmatter = @"
---
title: $Title
description: $Description
---

"@
    
    return $newFrontmatter + $Content
}

# Core Functions
Write-Host "Creating Core Functions..." -ForegroundColor Green

$initContent = Get-Content "docs\core\init.md" -Raw
$initConverted = Convert-DocToDocusFormat -Content $initContent -Title "init" -Description "Initialize the ArmaDragonflyClient extension"
$initConverted | Out-File -FilePath "docus\content\2.api\1.core\1.init.md" -Encoding utf8

$testContent = Get-Content "docs\core\test.md" -Raw
$testConverted = Convert-DocToDocusFormat -Content $testContent -Title "test" -Description "Test the database connection"
$testConverted | Out-File -FilePath "docus\content\2.api\1.core\2.test.md" -Encoding utf8

# Basic Operations
Write-Host "Creating Basic Operations..." -ForegroundColor Green

$setContent = Get-Content "docs\basic\set.md" -Raw
$setConverted = Convert-DocToDocusFormat -Content $setContent -Title "set" -Description "Set a key-value pair in the database"
$setConverted | Out-File -FilePath "docus\content\2.api\2.basic\1.set.md" -Encoding utf8

$getContent = Get-Content "docs\basic\get.md" -Raw
$getConverted = Convert-DocToDocusFormat -Content $getContent -Title "get" -Description "Get a value from the database"
$getConverted | Out-File -FilePath "docus\content\2.api\2.basic\2.get.md" -Encoding utf8

$delContent = Get-Content "docs\basic\delete.md" -Raw
$delConverted = Convert-DocToDocusFormat -Content $delContent -Title "del" -Description "Delete a key from the database"
$delConverted | Out-File -FilePath "docus\content\2.api\2.basic\3.del.md" -Encoding utf8

$existsContent = @"
---
title: exists
description: Check if keys exist in the database
---

# exists

Check if one or more keys exist in the database.

## Syntax

``````sqf
[_key1, _key2, ...] call dragonfly_db_fnc_exists
``````

## Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| _key | String | Key(s) to check | Yes |

## Return Value

Returns the number of keys that exist (integer).

## Examples

### Check single key

``````sqf
private _exists = ["playerData"] call dragonfly_db_fnc_exists;
// Returns: 1 if exists, 0 if not
``````

### Check multiple keys

``````sqf
private _count = ["player1", "player2", "player3"] call dragonfly_db_fnc_exists;
// Returns: number of keys that exist (0-3)
``````

## Notes

- Returns the count of existing keys
- Can check multiple keys at once
- Works across all store types (KV, Hash, List)

## Related Functions

- [set](/api/basic/set) - Set a value
- [get](/api/basic/get) - Get a value
- [del](/api/basic/del) - Delete a key
"@
$existsContent | Out-File -FilePath "docus\content\2.api\2.basic\4.exists.md" -Encoding utf8

$saveContent = Get-Content "docs\basic\save.md" -Raw
$saveConverted = Convert-DocToDocusFormat -Content $saveContent -Title "save" -Description "Save the database to disk"
$saveConverted | Out-File -FilePath "docus\content\2.api\2.basic\5.save.md" -Encoding utf8

$fetchContent = Get-Content "docs\basic\fetch.md" -Raw
$fetchConverted = Convert-DocToDocusFormat -Content $fetchContent -Title "fetch" -Description "Internal function to process data chunks"
$fetchConverted | Out-File -FilePath "docus\content\2.api\2.basic\6.fetch.md" -Encoding utf8

# Hash Operations
Write-Host "Creating Hash Operations..." -ForegroundColor Green

$hsetContent = Get-Content "docs\hash\hashSet.md" -Raw
$hsetConverted = Convert-DocToDocusFormat -Content $hsetContent -Title "hset" -Description "Set a field in a hash"
$hsetConverted | Out-File -FilePath "docus\content\2.api\3.hash\1.hset.md" -Encoding utf8

$hmsetContent = Get-Content "docs\hash\hashSetBulk.md" -Raw
$hmsetConverted = Convert-DocToDocusFormat -Content $hmsetContent -Title "hmset" -Description "Set multiple fields in a hash"
$hmsetConverted | Out-File -FilePath "docus\content\2.api\3.hash\2.hmset.md" -Encoding utf8

$hgetContent = Get-Content "docs\hash\hashGet.md" -Raw
$hgetConverted = Convert-DocToDocusFormat -Content $hgetContent -Title "hget" -Description "Get a field from a hash"
$hgetConverted | Out-File -FilePath "docus\content\2.api\3.hash\3.hget.md" -Encoding utf8

$hgetallContent = Get-Content "docs\hash\hashGetAll.md" -Raw
$hgetallConverted = Convert-DocToDocusFormat -Content $hgetallContent -Title "hgetall" -Description "Get all fields from a hash"
$hgetallConverted | Out-File -FilePath "docus\content\2.api\3.hash\4.hgetall.md" -Encoding utf8

$hdelContent = Get-Content "docs\hash\hashRemove.md" -Raw
$hdelConverted = Convert-DocToDocusFormat -Content $hdelContent -Title "hdel" -Description "Delete fields from a hash"
$hdelConverted | Out-File -FilePath "docus\content\2.api\3.hash\5.hdel.md" -Encoding utf8

# Add remaining hash functions
$hashFunctions = @(
    @{Name="hexists"; Title="hexists"; Desc="Check if a field exists in a hash"; File="6.hexists.md"},
    @{Name="hlen"; Title="hlen"; Desc="Get number of fields in a hash"; File="7.hlen.md"},
    @{Name="hkeys"; Title="hkeys"; Desc="Get all field names from a hash"; File="8.hkeys.md"},
    @{Name="hvals"; Title="hvals"; Desc="Get all values from a hash"; File="9.hvals.md"},
    @{Name="hincrby"; Title="hincrby"; Desc="Increment a hash field by an integer"; File="10.hincrby.md"},
    @{Name="hincrbyfloat"; Title="hincrbyfloat"; Desc="Increment a hash field by a float"; File="11.hincrbyfloat.md"}
)

foreach ($func in $hashFunctions) {
    $content = @"
---
title: $($func.Title)
description: $($func.Desc)
---

# $($func.Title)

$($func.Desc).

## Syntax

See the [Hash Operations documentation](/api/hash/hset) for detailed syntax and examples.

## Related Functions

- [hset](/api/hash/hset) - Set a hash field
- [hget](/api/hash/hget) - Get a hash field
- [hgetall](/api/hash/hgetall) - Get all hash fields
"@
    $content | Out-File -FilePath "docus\content\2.api\3.hash\$($func.File)" -Encoding utf8
}

# List Operations
Write-Host "Creating List Operations..." -ForegroundColor Green

$listFunctions = @(
    @{Name="lpush"; Title="lpush"; Desc="Push value to the left of a list"; File="1.lpush.md"; Source="docs\list\listAdd.md"},
    @{Name="rpush"; Title="rpush"; Desc="Push value to the right of a list"; File="2.rpush.md"; Source="docs\list\listAdd.md"},
    @{Name="lidx"; Title="lindex"; Desc="Get value at index from a list"; File="3.lindex.md"; Source="docs\list\listGet.md"},
    @{Name="lrange"; Title="lrange"; Desc="Get range of values from a list"; File="4.lrange.md"; Source="docs\list\listLoad.md"},
    @{Name="lset"; Title="lset"; Desc="Set value at index in a list"; File="5.lset.md"; Source="docs\list\listSet.md"},
    @{Name="lrem"; Title="lrem"; Desc="Remove values from a list"; File="6.lrem.md"; Source="docs\list\listRemove.md"},
    @{Name="llen"; Title="llen"; Desc="Get length of a list"; File="7.llen.md"; Source="$null"}
)

foreach ($func in $listFunctions) {
    if ($func.Source -and (Test-Path $func.Source)) {
        $content = Get-Content $func.Source -Raw
        $converted = Convert-DocToDocusFormat -Content $content -Title $func.Title -Description $func.Desc
        $converted | Out-File -FilePath "docus\content\2.api\4.list\$($func.File)" -Encoding utf8
    } else {
        $content = @"
---
title: $($func.Title)
description: $($func.Desc)
---

# $($func.Title)

$($func.Desc).

## Syntax

See the [List Operations documentation](/api/list/lpush) for detailed syntax and examples.

## Related Functions

- [lpush](/api/list/lpush) - Push to left of list
- [rpush](/api/list/rpush) - Push to right of list
- [lrange](/api/list/lrange) - Get range from list
"@
        $content | Out-File -FilePath "docus\content\2.api\4.list\$($func.File)" -Encoding utf8
    }
}

# Create homepage
Write-Host "Creating homepage..." -ForegroundColor Green
$homepage = @"
---
seo:
  title: ArmaDragonflyClient - Ultra-Fast In-Memory Database for Arma 3
  description: Experience next-level persistence in Arma 3 with ArmaDragonflyClient - an ultra-fast in-memory data store powered by C# .NET 8 and DragonflyDB. Redis-like API with unparalleled performance.
---

::u-page-hero
#title
ArmaDragonflyClient

#description
Experience next-level persistence in Arma 3 with our mod powered by C# .NET 8 and DragonflyDB.

This ultra-fast in-memory data store offers unparalleled performance and scalability for your Arma 3 gameplay data management needs.

#links
  :::u-button
  ---
  color: primary
  size: xl
  to: /getting-started/installation
  trailing-icon: i-lucide-arrow-right
  ---
  Get started
  :::

  :::u-button
  ---
  color: neutral
  icon: simple-icons-github
  size: xl
  to: https://github.com/innovativedevsolutions/dragonfly
  variant: outline
  ---
  Star on GitHub
  :::
::

::u-page-section
#title
Powerful features for Arma 3 persistence

#features
  :::u-page-feature
  ---
  icon: i-lucide-zap
  ---
  #title
  [Blazing Fast]{.text-primary} Performance
  
  #description
  Powered by DragonflyDB, an ultra-fast Redis alternative. Experience minimal latency and maximum throughput for all database operations in your Arma 3 missions.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-database
  ---
  #title
  [Redis-Like]{.text-primary} API
  
  #description
  Familiar Redis-style commands for key-value, hash tables, and lists. Easy to learn, powerful to use with operations like SET, GET, HSET, LPUSH, and more.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-save
  ---
  #title
  [Persistent Storage]{.text-primary}
  
  #description
  Save your data to disk when needed with the SAVE command. DragonflyDB handles persistence efficiently with snapshots and AOF logs.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-shield-check
  ---
  #title
  [Production Ready]{.text-primary}
  
  #description
  Built with C# .NET 8 and DragonflyDB's proven architecture. Reliable, stable, and battle-tested for mission-critical Arma 3 deployments.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-split
  ---
  #title
  [Smart Chunking]{.text-primary}
  
  #description
  Automatic data chunking for large payloads exceeding the 20KB buffer. Seamlessly handles large arrays, loadouts, and complex game data.
  :::

  :::u-page-feature
  ---
  icon: i-lucide-users
  ---
  #title
  [Context Awareness]{.text-primary}
  
  #description
  Player Steam ID integration with special placeholders for single-player and multiplayer modes. Easy per-player data management without manual key tracking.
  :::
::
"@
$homepage | Out-File -FilePath "docus\content\index.md" -Encoding utf8

# Create getting started section
Write-Host "Creating getting started section..." -ForegroundColor Green
New-Item -ItemType Directory -Force -Path "docus\content\1.getting-started" | Out-Null

@"
title: Getting Started
"@ | Out-File -FilePath "docus\content\1.getting-started\.navigation.yml" -Encoding utf8

$installation = @"
---
title: Installation
description: How to install and configure ArmaDragonflyClient
---

# Installation

Get started with ArmaDragonflyClient in just a few steps.

## Prerequisites

- Arma 3 (client or dedicated server)
- DragonflyDB server (running and accessible)

## Step 1: Install the Mod

Subscribe to the mod on Steam Workshop or download from [GitHub Releases](https://github.com/innovativedevsolutions/dragonfly/releases).

## Step 2: Create Configuration Directory

Create a folder named **@dragonfly** in your Arma 3 root directory (same location as ``arma3_x64.exe`` and/or ``arma3server_x64.exe``).

## Step 3: Configure Settings

Copy ``config.xml`` from the mod folder into ``@dragonfly\config.xml`` and configure:

``````xml
<?xml version="1.0" encoding="UTF-8"?>
<config>
    <host>127.0.0.1</host>
    <port>6379</port>
    <password>your_password_here</password>
    <contextLog>false</contextLog>
    <debug>false</debug>
</config>
``````

### Configuration Options

| Option | Type | Description | Default |
|--------|------|-------------|---------|
| host | String | DragonflyDB server IP address | 127.0.0.1 |
| port | Integer | DragonflyDB server port | 6379 |
| password | String | Authentication password (optional) | xyz123 |
| contextLog | Boolean | Enable context logging | false |
| debug | Boolean | Enable debug mode | false |

## Step 4: Launch Arma 3

Start Arma 3 with the mod enabled. The extension will automatically connect to your DragonflyDB server.

## Verify Installation

In your mission, run:

``````sqf
[] call dragonfly_db_fnc_test;
``````

If successful, you're ready to use ArmaDragonflyClient!

## Next Steps

- Learn about [Basic Operations](/api/basic/set)
- Explore [Hash Operations](/api/hash/hset)
- Check out [List Operations](/api/list/lpush)
"@
$installation | Out-File -FilePath "docus\content\1.getting-started\1.installation.md" -Encoding utf8

$quickstart = @"
---
title: Quick Start
description: Get up and running quickly with ArmaDragonflyClient
---

# Quick Start

This guide will help you get started with ArmaDragonflyClient in minutes.

## Basic Usage

### Initialize the Database

``````sqf
[] call dragonfly_db_fnc_init;
``````

### Store Data

``````sqf
// Set a simple value
["playerName", "John Doe"] call dragonfly_db_fnc_set;

// Set player data using hash (context mode)
["", "loadout", getUnitLoadout player] call dragonfly_db_fnc_hset;
["", "position", getPosASL player] call dragonfly_db_fnc_hset;
``````

### Retrieve Data

``````sqf
// Get a simple value
private _name = ["playerName"] call dragonfly_db_fnc_get;

// Get player data using hash (context mode)
private _loadout = ["", "loadout"] call dragonfly_db_fnc_hget;
private _position = ["", "position"] call dragonfly_db_fnc_hget;
``````

### Delete Data

``````sqf
// Delete a key
["playerName"] call dragonfly_db_fnc_del;

// Delete hash fields (context mode)
["", "loadout"] call dragonfly_db_fnc_hdel;
``````

## Multiplayer Usage

For multiplayer missions, use ``remoteExec`` to run functions on the server:

``````sqf
// Set data on server
["myKey", myValue] remoteExec ["dragonfly_db_fnc_set", 2, false];

// Get data from server with callback
["myKey", netId player, "myCallbackFunction"] remoteExec ["dragonfly_db_fnc_get", 2, false];
``````

## Context Mode

ArmaDragonflyClient supports context mode, which automatically uses the player's Steam ID as the key:

``````sqf
// These use the player's Steam ID automatically
["", "fieldName", value] call dragonfly_db_fnc_hset;
["", "fieldName"] call dragonfly_db_fnc_hget;
``````

## Important Notes

::alert{type="warning"}
Keep payloads under **20,480 bytes**. This is the maximum size the engine can send or receive at one time.
::

For responses larger than 20,480 bytes, provide a callback function:

``````sqf
["myKey", player, "myCallbackFunction"] call dragonfly_db_fnc_get;
``````

## Next Steps

- Explore [Core Functions](/api/core/init)
- Learn about [Hash Operations](/api/hash/hset)
- Check out [List Operations](/api/list/lpush)
"@
$quickstart | Out-File -FilePath "docus\content\1.getting-started\2.quickstart.md" -Encoding utf8

Write-Host ""
Write-Host "Documentation generation complete!" -ForegroundColor Green
Write-Host "Files created in docus\content\" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run 'bun install' or 'npm install' in the docus directory"
Write-Host "2. Run 'bun run dev' or 'npm run dev' to preview"
Write-Host "3. Run 'bun run build' or 'npm run build' to build for production"
