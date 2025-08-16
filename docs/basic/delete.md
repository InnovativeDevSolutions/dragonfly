---
title: ArmaDragonflyClient - Delete Key
icon: mdi:file-text-outline
excerpt: Remove the specified key from RAMDb.
---

# dragonfly_db_fnc_del

## Description
Removes the specified key from the database.

## Syntax
```sqf
[_key] call dragonfly_db_fnc_del
```

## Parameters
| Parameter | Type   | Description                               |
|-----------|--------|-------------------------------------------|
| `_key`    | String | Name of the key to delete from the database |

## Return Value
None. The operation runs asynchronously.

## Examples
### Delete a key in singleplayer or on the server:
```sqf
["myKey"] call dragonfly_db_fnc_del;
```

### Delete a key on the server from a client:
```sqf
["myKey"] remoteExecCall ["dragonfly_db_fnc_del", 2, false];
```

## Notes
- This operation is permanent and cannot be undone
- If the key doesn't exist, the operation will have no effect

## Related Functions
- `dragonfly_db_fnc_get`: Retrieves a value by key
- `dragonfly_db_fnc_set`: Stores a value by key

## Links

[Delete Key](delete.md) |
[Get Key](get.md) |
[Set Key](set.md)