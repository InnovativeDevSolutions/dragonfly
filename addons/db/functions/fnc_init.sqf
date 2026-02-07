#include "..\script_component.hpp"

/*
 * Author: Creedcoder, J.Schmidt
 * Initial Extension settings.
 *
 * Arguments:
 * N/A
 *
 * Return Value:
 * N/A
 *
 * Examples:
 * N/A
 *
 * Public: Yes
 */

dragonfly_db_fetch_array = [];

private _dll = "ArmaDragonflyClient" callExtension ["version", []];

diag_log text format ["ArmaDragonflyClient: DLL Version %1 found", _dll];
diag_log text "ArmaDragonflyClient: Functions loaded and Initialization completed!";
diag_log text format ["ArmaDragonflyClient: Buffer size set to %1 Bytes", GVAR(buffer)];
diag_log text "ArmaDragonflyClient: Ready for use!";
