# List Operations

This section contains documentation for the list operations of ArmaDragonflyClient that allow for working with ordered collections of items.

## Available Functions

- [lpush | rpush](listAdd.md) - Add an item to a list
- [lindex](listGet.md) - Get items from a list
- [lrange](listLoad.md) - Load a list from the database
- [lrem](listRemove.md) - Remove an item from a list
- [lset](listSet.md) - Set an item in a list

## Example Usage

```sqf
// Add an item to a list
["myList", ["myItem"]] call dragonfly_db_fnc_lpush;
["myList", ["myItem"]] call dragonfly_db_fnc_rpush;

// Set an item at a specific index
["myList", 0, [myNewValue]] call dragonfly_db_fnc_lset;

// Get an item at a specific index
["myList", 0, "myFunction"] call dragonfly_db_fnc_lindex;

// Load all items from a list
["myList", "myFunction"] call dragonfly_db_fnc_lrange;

// Remove an item at a specific index
["myList", 0] call dragonfly_db_fnc_lrem;
```

## Related Categories

- [Core Functions](../core/README.md)
- [Basic Data Operations](../basic/README.md)
- [Hash Operations](../hash/README.md)