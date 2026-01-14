using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable PropertyCanBeMadeInitOnly.Global

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaDragonflyClient
#pragma warning restore IDE0130 // Namespace does not match folder structure
{

    /// <summary>
    /// Represents the context information for a remote procedure call.
    /// </summary>
    public class CallContext
    {
        /// <summary>
        /// Gets or sets the Steam ID of the user making the call.
        /// </summary>
        public ulong SteamId { get; set; }

        /// <summary>
        /// Gets or sets the source file path of the call.
        /// </summary>
        public string FileSource { get; set; }

        /// <summary>
        /// Gets or sets the name of the mission associated with the call.
        /// </summary>
        public string MissionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the server where the call originated.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the ID of the owner who remotely executed the call.
        /// </summary>
        public short RemoteExecutedOwner { get; set; }
    }

    /// <summary>
    /// Main class containing core functionality and configuration settings.
    /// </summary>
    public static class Main
    {
        /// <summary>
        /// The current version of DragonflyDB.
        /// </summary>
        private const string AdcVersion = "1.0.0";

        /// <summary>
        /// The buffer size for database operations.
        /// </summary>
        public const int AdcBufferSize = 20480;

        /// <summary>
        /// Gets the path for the config file.
        /// </summary>
        private static readonly string AdcConfigPath = $"{Path.DirectorySeparatorChar}@dragonfly{Path.DirectorySeparatorChar}config.xml";

        /// <summary>
        /// Gets the folder path for storing log files.
        /// </summary>
        private static readonly string AdcLogFolder = $"{Path.DirectorySeparatorChar}@dragonfly{Path.DirectorySeparatorChar}logs";

        /// <summary>
        /// Gets or sets whether debug mode is enabled.
        /// </summary>
        private static bool AdcDebug { get; set; } = false;

        /// <summary>
        /// Gets or sets whether an initialization check has been performed.
        /// </summary>
        private static bool AdcInitCheck { get; set; } = false;

        /// <summary>
        /// Gets or sets the host address.
        /// </summary>
        public static string AdcHost { get; private set; } = "127.0.0.1";

        /// <summary>
        /// Gets or sets the host port.
        /// </summary>
        public static int AdcPort { get; private set; } = 6379;

        /// <summary>
        /// Gets or sets the host password.
        /// </summary>
        public static string AdcPassword { get; private set; } = "xyz123";

        /// <summary>
        /// Gets or sets the Steam ID of the current user.
        /// </summary>
        private static string SteamId { get; set; } = "";

        /// <summary>
        /// Gets or sets whether context logging is enabled.
        /// </summary>
        private static bool AdcContextLog { get; set; } = false;

        /// <summary>
        /// Gets or sets the pointer to the current DragonflyDB context.
        /// </summary>
        public static IntPtr AdcContext { get; private set; }

        /// <summary>
        /// Gets or sets whether the database is currently loaded.
        /// </summary>
        public static bool AdcIsLoaded { get; private set; } = false;

        /// <summary>
        /// Delegate for extension callback functions that process extension operations.
        /// </summary>
        /// <param name="name">The name of the extension.</param>
        /// <param name="function">The function to be called.</param>
        /// <param name="data">The data to be processed.</param>
        /// <returns>Integer result code of the operation.</returns>
        public delegate int ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);

        /// <summary>
        /// Gets or sets the callback function for extension operations.
        /// </summary>
        public static ExtensionCallback Callback { get; private set; }

        
        /// <summary>
        /// Resolves a player key to a valid Steam ID format. This method handles several cases:
        /// 1. If the key is null, empty, or contains only quotes, returns the current Steam ID
        /// 2. If the key exactly matches "_SP_PLAYER_", returns the current Steam ID
        /// 3. If the key starts with "_SP_PLAYER_:", replaces that prefix with the current Steam ID
        /// 4. Otherwise, returns the key with any surrounding quotes removed
        /// </summary>
        /// <param name="key">The player key to resolve. Can be null, empty, or contain special placeholders.</param>
        /// <returns>A string representing the resolved player key, either the current Steam ID or a processed version of the input key.</returns>
        private static string ResolveKey(string key)
        {
            if (string.IsNullOrEmpty(key) || key == "" || key == "\"\"")
                return SteamId;

            var finalKey = key.Trim('"');

            if (finalKey == "_SP_PLAYER_")
                return SteamId;

            if (finalKey.StartsWith("_SP_PLAYER_:"))
                return finalKey.Replace("_SP_PLAYER_", SteamId);

            return finalKey;
        }

        /// <summary>
        ///     Initializes the application by reading the configuration from a specified
        ///     file or using default settings if the file is not found. It sets up the
        ///     main database, logging, debugging, and other runtime parameters.
        /// </summary>
        private static void Init()
        {
            char[] separator = ['='];
            var commandLineArgs = Environment.GetCommandLineArgs();

            foreach (var t in commandLineArgs)
            {
                var strArray = t.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (!strArray[0].Equals("-rdb", StringComparison.CurrentCultureIgnoreCase)) continue;
                var str = strArray[1];
                break;
            }

            if (!File.Exists(Environment.CurrentDirectory + AdcConfigPath))
                Log("Config file not found! Default settings will be used.", "action");

            try
            {
                var configXml = XElement.Load(Environment.CurrentDirectory + AdcConfigPath);
                List<string> settings = [.. configXml.Elements().Select(element => (string)element)];

                AdcHost = settings[0];
                if (int.TryParse(settings[1], out var res1))
                    AdcPort = res1;
                AdcPassword = settings[2];
                if (bool.TryParse(settings[3], out var res3))
                    AdcContextLog = res3;
                if (bool.TryParse(settings[4], out var res4))
                    AdcDebug = res4;

                Log($"Config file found! Context Mode: {AdcContextLog}! Debug Mode: {AdcDebug}! Host Address: {AdcHost}! Host Port: {AdcPort}! Host Password: {AdcPassword}!", "action");

                AdcIsLoaded = true;
            }
            catch (Exception e)
            {
                Log($"Error reading config file: {e.Message}", "error");
                Log("Default settings will be used", "action");
            }

            AdcInitCheck = true;
        }

        /// <summary>
        ///     Logs a message to a file of the specified type if debugging is enabled.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="type">The type of log (e.g., "error", "action").</param>
        public static void Log(string message, string type)
        {
            if (!AdcDebug)
                return;

            var fileName = $"{type}.log";
            var filePath = Environment.CurrentDirectory + AdcLogFolder;

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            using StreamWriter sw = new(Path.Combine(filePath, fileName), true);
            sw.WriteLine($"{DateTime.Now} >> {message}");
        }

        /// <summary>
        ///     Called only once when Arma 3 loads the extension.
        /// </summary>
        /// <param name="func">Pointer to Arma 3's callback function</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionRegisterCallback")]
        public static unsafe void RvExtensionRegisterCallback(nint func)
        {
            Callback = Marshal.GetDelegateForFunctionPointer<ExtensionCallback>(func);
        }

        /// <summary>
        ///     Called only once when Arma 3 loads the extension.
        ///     The output will be written in the RPT logs.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="outputSize">The maximum length of the buffer (always 32 for this particular method)</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionVersion")]
        public static unsafe void RvExtensionVersion(char* output, int outputSize)
        {
            WriteOutput(output, $"ArmaDragonflyClient {AdcVersion}");
        }

        /// <summary>
        ///     Called before every RVExtension/RVExtensionArgs execution, providing context about where the call came from.
        /// </summary>
        /// <param name="argv">
        ///     Array of pointers to null-terminated strings containing:
        ///     [0] - Steam 64bit UserID
        ///     [1] - FileSource
        ///     [2] - MissionName
        ///     [3] - ServerName
        ///     [4] - Machine Network ID
        /// </param>
        /// <param name="argc">Number of arguments in the argv array (always 5).</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionContext")]
        public static unsafe int RvExtensionContext(char** argv, int argc)
        {
            AdcContext = (IntPtr)argv;

            var context = new CallContext
            {
                SteamId = ulong.Parse(Marshal.PtrToStringAnsi((IntPtr)argv[0]) ?? "0"),
                FileSource = Marshal.PtrToStringAnsi((IntPtr)argv[1]) ?? "",
                MissionName = Marshal.PtrToStringAnsi((IntPtr)argv[2]) ?? "",
                ServerName = Marshal.PtrToStringAnsi((IntPtr)argv[3]) ?? "",
                RemoteExecutedOwner = short.Parse(Marshal.PtrToStringAnsi((IntPtr)argv[4]) ?? "0")
            };

            SteamId = context.SteamId.ToString();

            if (AdcContextLog)
            {
                Log(
                    $"Context: SteamID={context.SteamId}, FileSource={context.FileSource}, MissionName={context.MissionName}, ServerName={context.ServerName}, RemoteExecutedOwner={context.RemoteExecutedOwner}",
                    "context");
            }

            return 100;
        }

        /// <summary>
        ///     The entry point for the default "callExtension" command.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="outputSize">The maximum size of the buffer (20,480 bytes)</param>
        /// <param name="function">The function identifier passed in "callExtension"</param>
        [UnmanagedCallersOnly(EntryPoint = "RVExtension")]
        public static unsafe void RvExtension(char* output, int outputSize, char* function)
        {
            if (!AdcInitCheck)
                Init();

            var func = GetString(function);
            switch (func.ToLower())
            {
                case "time":
                    WriteOutput(output, DateTime.Now.ToString("dd:MM:yyyy HH:mm:ss"));
                    break;
                case "version":
                    WriteOutput(output, AdcVersion);
                    break;
                default:
                    WriteOutput(output, func);
                    break;
            }
        }

        /// <summary>
        ///     The entry point for the "callExtension" command with function arguments.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="outputSize">The maximum size of the buffer (20,480 bytes)</param>
        /// <param name="function">The function identifier passed in "callExtension"</param>
        /// <param name="argv">The args passed to "callExtension" as a string array</param>
        /// <param name="argc">Number of elements in "argv"</param>
        /// <returns>The return code</returns>
        [UnmanagedCallersOnly(EntryPoint = "RVExtensionArgs")]
        public static unsafe int RvExtensionArgs(char* output, int outputSize, char* function, char** argv, int argc)
        {
            if (!AdcInitCheck)
                Init();

            var id = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var func = GetString(function);

            List<string> args = [];
            for (var i = 0; i < argc; i++)
            {
                args.Add(GetString(argv[i]));
            }

            var count = 0;

            // Log($"Function: {func}, Args: {string.Join(", ", args)}", "debug");

            switch (func.ToLower())
            {
                case "set":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var set = DragonflyDB.Set(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, set);
                    return 100;
                case "get":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var get = DragonflyDB.Get(ResolveKey(args[0]));

                    WriteOutput(output, get ?? "");
                    return 200;
                case "del":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    foreach (var arg in args)
                    {
                        DragonflyDB.Del(arg.Trim('"'));
                        count++;
                    }

                    WriteOutput(output, $"{count}");
                    return 100;
                case "exists":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    foreach (var arg in args)
                    {
                        DragonflyDB.Exists(arg.Trim('"'));
                        count++;
                    }

                    WriteOutput(output, $"{count}");
                    return 200;
                case "hset":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hset = DragonflyDB.HSet(ResolveKey(args[0]), args[1].Trim('"'), args[2]);

                    WriteOutput(output, $"{hset}");
                    return 100;
                case "hmset":
                    if (argc < 2 || (argc - 1) % 2 != 0)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var fieldValues = new Dictionary<string, string>();

                    for (var i = 1; i < argc; i += 2) fieldValues.Add(args[i].Trim('"'), args[i + 1]);
                    var hmset = DragonflyDB.HMSet(ResolveKey(args[0]), fieldValues);

                    WriteOutput(output, hmset);
                    return 100;
                case "hget":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hget = DragonflyDB.HGet(ResolveKey(args[0]), args[1].Trim('"'));

                    if (string.IsNullOrEmpty(hget))
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hget", AdcBufferSize, hget,
                            args[2], args[3], Convert.ToBoolean(args[4])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hget", AdcBufferSize, hget));
                    }

                    return 200;
                case "hgetall":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hgetall = DragonflyDB.HGetAll(ResolveKey(args[0]));

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hgetall", AdcBufferSize, SerializeList(hgetall),
                            args[1], args[2], Convert.ToBoolean(args[3])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hgetall", AdcBufferSize, SerializeList(hgetall)));
                    }

                    return 200;
                case "hdel":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var fieldsToDelete = new List<string>();

                    for (var i = 1; i < argc; i++) fieldsToDelete.Add(args[i]);

                    foreach (var field in fieldsToDelete)
                    {
                        DragonflyDB.HDel(ResolveKey(args[0]), field.Trim('"'));
                        count++;
                    }

                    WriteOutput(output, $"{count}");
                    return 200;
                case "hlen":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hlen = DragonflyDB.HLen(ResolveKey(args[0]));

                    if (hlen == 0)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    WriteOutput(output, $"{hlen}");
                    return 200;
                case "hkeys":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hkeys = DragonflyDB.HKeys(ResolveKey(args[0]));

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hkeys", AdcBufferSize, SerializeList(hkeys),
                            args[1], args[2], Convert.ToBoolean(args[3])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hkeys", AdcBufferSize, SerializeList(hkeys)));
                    }

                    return 200;
                case "hvals":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hvals = DragonflyDB.HVals(ResolveKey(args[0]));

                    if (argc >= 4)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hvals", AdcBufferSize, SerializeList(hvals),
                            args[1], args[2], Convert.ToBoolean(args[3])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_hvals", AdcBufferSize, SerializeList(hvals)));
                    }

                    return 200;
                case "hexists":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hexists = DragonflyDB.HExists(ResolveKey(args[0]), args[1].Trim('"'));

                    WriteOutput(output, $"{hexists}");
                    return 200;
                case "hincrby":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hincrby = DragonflyDB.HIncrBy(ResolveKey(args[0]), args[1].Trim('"'), int.Parse(args[2]));

                    WriteOutput(output, $"{hincrby}");
                    return 200;
                case "hincrbyfloat":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var hincrbyfloat = DragonflyDB.HIncrByFloat(ResolveKey(args[0]), args[1].Trim('"'), int.Parse(args[2]));

                    WriteOutput(output, hincrbyfloat);
                    return 200;
                case "lindex":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var listIndex = DragonflyDB.LIndex(ResolveKey(args[0]), args[1].Trim('"'));

                    if (listIndex is null)
                    {
                        WriteOutput(output, "NotFound");
                        return -1;
                    }

                    if (argc >= 5)
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_lindex", AdcBufferSize, listIndex,
                            args[2], args[3], Convert.ToBoolean(args[4])));
                    }
                    else
                    {
                        WriteOutput(output, Utils.CheckByteCount($"{id}_lindex", AdcBufferSize, listIndex));
                    }

                    return 200;
                case "llen":
                    if (argc < 1)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var listLength = DragonflyDB.LLen(ResolveKey(args[0]));

                    WriteOutput(output, $"{listLength}");
                    return 200;
                case "lpush":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lpush = DragonflyDB.LPush(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, $"{lpush}");
                    return 200;
                case "lrange":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lrange = DragonflyDB.LRange(ResolveKey(args[0]), args[1].Trim('"'), args[2].Trim('"'));

                    WriteOutput(output, SerializeList(lrange));
                    return 200;
                case "lrem":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lrem = DragonflyDB.LRem(ResolveKey(args[0]), args[1].Trim('"'));

                    WriteOutput(output, $"{lrem}");
                    return 200;
                case "lset":
                    if (argc < 3)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var lset = DragonflyDB.LSet(ResolveKey(args[0]), args[1].Trim('"'), args[2]);

                    WriteOutput(output, lset);
                    return 200;
                case "rpush":
                    if (argc < 2)
                    {
                        WriteOutput(output, "Invalid number of arguments");
                        return -1;
                    }

                    var rpush = DragonflyDB.RPush(ResolveKey(args[0]), args[1]);

                    WriteOutput(output, $"{rpush}");
                    return 200;
                case "save":
                    DragonflyDB.Save();

                    WriteOutput(output, "Saved data to disc");
                    return 100;
                case "version":
                    WriteOutput(output, AdcVersion);
                    return 100;
                default:
                    WriteOutput(output, "Available functions: set, get, del, exists, hset, hmset, hget, hgetall, hdel, hlen, hkeys, hvals, hexists, hincrby, hincrbyfloat, lindex, llen, linsert, lpop, lpush, lrange, lrem, lset, ltrim, rpop, rpush, incrby, incrbyfloat, save, version");
                    return -1;
            }
        }

        /// <summary>
        ///     Reads a string from the given pointer.
        ///     If the pointer is null, a default value will be returned instead.
        /// </summary>
        /// <param name="pointer">The pointer to read</param>
        /// <param name="defaultValue">The string's default value</param>
        /// <returns>The marshaled string</returns>
        private static unsafe string GetString(char* pointer, string defaultValue = "")
        {
            return Marshal.PtrToStringAnsi((nint)pointer) ?? defaultValue;
        }

        /// <summary>
        ///     Serializes a list of strings into a string representing a valid Arma 3 array.
        /// </summary>
        /// <param name="list">The list of strings to serialize</param>
        /// <returns>A string representing an Arma 3 array</returns>
        public static string SerializeList(List<string> list)
        {
            var content = string.Join(",", [.. list]);
            return $"[{content}]";
        }

        /// <summary>
        ///     Writes the given payload to the output buffer using the provided pointer.
        ///     Ensures that the payload string is always null terminated.
        /// </summary>
        /// <param name="output">A pointer to the output buffer</param>
        /// <param name="payload">The payload to write</param>
        private static unsafe void WriteOutput(char* output, string payload)
        {
            Log($"Writing output: {payload}", "info");

            var bytes = Encoding.ASCII.GetBytes(payload + '\0');
            Marshal.Copy(bytes, 0, (nint)output, bytes.Length);
        }
    }
}