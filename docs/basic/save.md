---
title: ArmaDragonflyClient - Save DB
icon: mdi:file-text-outline
excerpt: Save DB to disc.
---

# dragonfly_db_fnc_save

## Description
Saves the entire database to disk storage. This function persists all data (key-value pairs, hash tables, and lists), allowing it to be retrieved later even after server restart.

## Syntax
```sqf
[] call dragonfly_db_fnc_save
```

## Parameters
N/A

## Return Value
None. The operation runs synchronously and saves the database immediately.

## Examples
### Save the database:
```sqf
[] call dragonfly_db_fnc_save;
```

### Call the save function remotely from a client:
```sqf
[] remoteExecCall ["dragonfly_db_fnc_save", 2, false];
```

## Notes
- This function should be called periodically to ensure data persistence
- The backup feature creates a timestamped copy of the database
- Saving is a resource-intensive operation, so it shouldn't be called too frequently
- Consider saving before mission end or during low-activity periods

## Related Functions
N/A

## Links

[Save DB](save.md)