# ArmaDragonflyClient Documentation

This documentation provides details on all functions available in `ArmaDragonflyClient`. These functions allow you to interact with the in-memory database system for Arma 3.

## Important Payload Limit
Keep payloads under 20,480 bytes. This is the maximum size the engine can send or receive at one time. If you expect a response larger than 20,480 bytes, provide a callback function so the mod can use its internal fetch/chunking flow and deliver the reconstructed data to your callback.

## Installation

1. Subscribe to the mod on Steam Workshop or download from releases
2. Create a folder named `@dragonfly` in your Arma 3 root directory (same location as `arma3_x64.exe` and/or `arma3server_x64.exe`)
3. Copy `config.xml` from the mod folder into `@dragonfly\config.xml`
4. Configure the settings in `config.xml` as needed:
   - Host IP
   - Host Port
   - Host Password (optional)
   - Context logging (true/false)
   - Debug mode (true/false)
5. Launch Arma 3 with the mod enabled

The `@dragonfly` folder will store:
- `logs\` - Debug and error logs (if debug mode is enabled)

## Function Categories

The functions are categorized by their purpose:

### [Core Functions](core/README.md)
- [init](core/init.md) - Initialize the database system
- [test](core/test.md) - Test the database connection

### [Basic Data Operations](basic/README.md)
- [get](basic/get.md) - Get a value from the database
- [set](basic/set.md) - Set a value in the database
- [del](basic/delete.md) - Delete a key from the database
- [save](basic/save.md) - Save the database to disk
- [fetch](basic/fetch.md) - Internal function to process data chunks

### [Hash Operations](hash/README.md)
- [hget](hash/hashGet.md) - Get a field from a hash
- [hgetall](hash/hashGetAll.md) - Get all fields from a hash
- [hrem](hash/hashRemove.md) - Remove a field from a hash
- [hset](hash/hashSet.md) - Set a field in a hash
- [hmset](hash/hashSetBulk.md) - Set multiple fields in a hash in one operation

### [List Operations](list/README.md)
- [lpush | rpush](list/listAdd.md) - Add an item to a list
- [lindex](list/listGet.md) - Get items from a list
- [lrange](list/listLoad.md) - Load a list from the database
- [lrem](list/listRemove.md) - Remove an item from a list
- [lset](list/listSet.md) - Set an item in a list

## Usage Examples

### Basic Usage
```sqf
// Initialize the database
[] call dragonfly_db_fnc_init;

// Set a value
["myKey", myValue] call dragonfly_db_fnc_set; (Server or Singleplayer only)
["myKey", myValue] remoteExec ["dragonfly_db_fnc_set", 2, false]; (Multiplayer only)

// Get a value
["myKey"] call dragonfly_db_fnc_get; (Server or Singleplayer only)
["myKey", netId player, "myFunction"] remoteExec ["dragonfly_db_fnc_get", 2, false]; (Multiplayer only)

// Delete a key
["myKey"] call dragonfly_db_fnc_del; (Server or Singleplayer only)
["myKey"] remoteExec ["dragonfly_db_fnc_del", 2, false]; (Multiplayer only)
```

### Hash Operations
```sqf 
// Set a hash field (context mode)
["", "myField", myValue] call dragonfly_db_fnc_hset; (Server or Singleplayer only)
["", "myField", myValue] remoteExec ["dragonfly_db_fnc_hset", 2, false]; (Multiplayer only)

// Get a hash field (context mode)
["", "myField"] call dragonfly_db_fnc_hget; (Server or Singleplayer only)
["", "myField", player, "myFunction"] remoteExec ["dragonfly_db_fnc_hget", 2, false]; (Multiplayer only)

// Get all hash fields (context mode)
[] call dragonfly_db_fnc_hgetall;
["", player, "myFunction"] remoteExec ["dragonfly_db_fnc_hgetall", 2, false]; (Multiplayer only)

// Set multiple hash fields (context mode)
["", ["loadout", getUnitLoadout player, "position", getPosASL player, "direction", getDir player, "stance", stance player]] call dragonfly_db_fnc_hmset; (Server or Singleplayer only)
["", ["loadout", getUnitLoadout player, "position", getPosASL player, "direction", getDir player, "stance", stance player]] remoteExec ["dragonfly_db_fnc_hmset", 2, false]; (Multiplayer only)

// Remove a hash field (context mode)
["", "myField"] call dragonfly_db_fnc_hrem; (Server or Singleplayer only)
["", "myField"] remoteExec ["dragonfly_db_fnc_hrem", 2, false]; (Multiplayer only)
```
```sqf
// Set a hash field for specific ID
["myKey", "myField", myValue] call dragonfly_db_fnc_hset; (Server or Singleplayer only)
["myKey", "myField", myValue] remoteExec ["dragonfly_db_fnc_hset", 2, false]; (Multiplayer only)

// Get a hash field for specific ID
["myKey", "myField"] call dragonfly_db_fnc_hget; (Server or Singleplayer only)
["myKey", "myField", player, "myFunction"] remoteExec ["dragonfly_db_fnc_hget", 2, false]; (Multiplayer only)

// Get all hash fields for specific ID
["myKey"] call dragonfly_db_fnc_hgetall; (Server or Singleplayer only)
["myKey" player, "myFunction"] remoteExec ["dragonfly_db_fnc_hgetall", 2, false]; (Multiplayer only)

// Set multiple hash fields for specific ID
[getPlayerUID player, ["loadout", getUnitLoadout player, "position", getPosASL player, "direction", getDir player, "stance", stance player]] call dragonfly_db_fnc_hmset; (Server or Singleplayer only)
[getPlayerUID player, ["loadout", getUnitLoadout player, "position", getPosASL player, "direction", getDir player, "stance", stance player]] remoteExec ["dragonfly_db_fnc_hmset", 2, false]; (Multiplayer only)

// Remove a hash field for specific ID
["myKey", "myField"] call dragonfly_db_fnc_hrem; (Server or Singleplayer only)
["myKey", "myField"] remoteExec ["dragonfly_db_fnc_hrem", 2, false]; (Multiplayer only)
```

### List Operations
```sqf
// Add an item to a list
["myKey", [myValue]] call dragonfly_db_fnc_lpush; (Server or Singleplayer only)
["myKey", [myValue]] call dragonfly_db_fnc_rpush; (Server or Singleplayer only)
["myKey", [myValue]] remoteExec ["dragonfly_db_fnc_lpush", 2, false]; (Multiplayer only)
["myKey", [myValue]] remoteExec ["dragonfly_db_fnc_rpush", 2, false]; (Multiplayer only)

// Set an item from a list
["myKey", 0, [myNewValue]] call dragonfly_db_fnc_lset; (Server or Singleplayer only)
["myKey", 0, [myNewValue]] remoteExec ["dragonfly_db_fnc_lset", 2, false]; (Multiplayer only)

// Get an item from a list
["myKey", 0] call dragonfly_db_fnc_lindex; (Server or Singleplayer only)
["myKey", 0, player, "myFunction"] remoteExec ["dragonfly_db_fnc_lindex", 2, false]; (Multiplayer only)

// Get items from a list
["myKey"] call dragonfly_db_fnc_lrange; (Server or Singleplayer only)
["myKey", player, "myFunction"] remoteExec ["dragonfly_db_fnc_lrange", 2, false]; (Multiplayer only)

// Remove an item from a list
["myKey", 0] call dragonfly_db_fnc_lrem; (Server or Singleplayer only)
["myKey", 0] remoteExec ["dragonfly_db_fnc_lrem", 2, false]; (Multiplayer only)
```

## Function Documentation Structure

Each function documentation includes:
- Function name and purpose
- Parameters
- Return value
- Examples
- Notes and warnings

## License
This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
To view a copy of this license, visit https://creativecommons.org/licenses/by-nc-sa/4.0/ or send a letter to Creative Commons, <br>PO Box 1866, Mountain View, CA 94042