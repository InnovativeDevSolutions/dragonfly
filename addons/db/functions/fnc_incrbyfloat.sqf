#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Increment a value by a float.
 *
 * Arguments:
 * 0: Key <STRING> - Key to increment
 * 1: Increment <NUMBER> - Increment value
 *
 * Return Value:
 * New value of the key after increment <NUMBER>
 *
 * Example:
 * ["myKey", 1.0] call dragonfly_db_fnc_incrbyfloat; (Server or Singleplayer only)
 * ["myKey", 1.0] remoteExec ["dragonfly_db_fnc_incrbyfloat", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_increment", 0.0, [0]]
];

if (_key isEqualTo "") exitWith {
    diag_log text format ["ArmaDragonflyClient: 'dragonfly_db_fnc_incrbyfloat' Invalid Input for Key '%1'", _key];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaDragonflyClient" callExtension ["incrbyfloat", [_key, _increment]];

GVAR(inuse) = false;

_result
