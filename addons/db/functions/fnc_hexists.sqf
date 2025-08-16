#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Check if a field exists in a hash.
 *
 * Arguments:
 * 0: Key <STRING> - Key to check
 * 1: Field <STRING> - Field to check
 *
 * Return Value:
 * Response from the database <STRING>
 *
 * Example:
 * ["myKey", "myField"] call dragonfly_db_fnc_hexists; (Server or Singleplayer only)
 * ["myKey", "myField"] remoteExec ["dragonfly_db_fnc_hexists", 2, false]; (Multiplayer only)
 *
 * Public: Yes
 */

params [
    ["_key", "", [""]],
    ["_field", "", [""]]
];

if (_field == "") exitWith {
    diag_log text format ["ArmaDragonflyClient: 'dragonfly_db_fnc_hexists' Invalid Input for Field '%1'", _field];
};

waitUntil { !GVAR(inuse) };
GVAR(inuse) = true;

private _result = "ArmaDragonflyClient" callExtension ["hexists", [_key, _field]];

GVAR(inuse) = false;

_result
