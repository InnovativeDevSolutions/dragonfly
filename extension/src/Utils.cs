using System.Collections.Concurrent;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaDragonflyClient
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    ///     Provides utility methods for common operations such as Base64 encoding/decoding, 
    ///     unique ID generation, and string manipulation.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        ///     Decodes a Base64 encoded string into its original plain text representation.
        ///     This method converts the input Base64 encoded string into a byte array
        ///     and then decodes the byte array into a plain text string using UTF-8 encoding.
        /// </summary>
        /// <param name="base64EncodedData">The Base64 encoded string to be decoded.</param>
        /// <return>A string containing the original plain text representation of the input Base64 data.</return>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        ///     Encodes a plain text string into its Base64 representation.
        ///     This method converts the input plain text into a byte array using UTF-8 encoding
        ///     and then encodes the byte array into a Base64 string.
        /// </summary>
        /// <param name="plainText">The plain text string to be encoded.</param>
        /// <return>A string containing the Base64 encoded representation of the input text.</return>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        ///     Generates a unique identifier based on the current UTC timestamp.
        ///     This method creates a unique identifier by retrieving the number of
        ///     milliseconds since the Unix epoch (January 1, 1970, 00:00:00 UTC).
        /// </summary>
        /// <return>A long value representing the unique identifier generated from the current UTC timestamp.</return>
        public static long GenerateUniqueId()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        ///     Splits a given string into smaller chunks of specified size.
        ///     This method divides the input string into multiple substrings based
        ///     on the specified chunk size, ensuring none exceed the given limit.
        /// </summary>
        /// <param name="data">The string to be divided into smaller chunks.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <return>A list of substrings, each with a length up to the specified chunk size.</return>
        public static List<string> SplitIntoChunks(string data, int chunkSize)
        {
            var totalChunks = (int)Math.Ceiling(data.Length / (double)chunkSize);
            List<string> chunks = [];

            for (var i = 0; i < totalChunks; i++)
            {
                var start = i * chunkSize;
                var end = Math.Min(data.Length, start + chunkSize);
                chunks.Add(data[start..end]);
            }

            return chunks;
        }

        /// <summary>
        ///     Processes a given data string, checks its byte size, and splits it into chunks if it exceeds the specified buffer
        ///     size.
        ///     If chunking is necessary, it sends each chunk to a callback function and returns metadata about the operation.
        /// </summary>
        /// <param name="uniqueId">A unique identifier used to associate the processed data.</param>
        /// <param name="bufferSize">The maximum permissible byte size for the data before it is split into chunks.</param>
        /// <param name="data">The data string to be processed, analyzed for byte size, and optionally chunked.</param>
        /// <param name="function">An optional function name associated with the processing, or null if not applicable.</param>
        /// <param name="entity">An optional entity name linked to the processing operation, or null if it does not apply.</param>
        /// <param name="call">A boolean value indicating whether to invoke a callback function during processing.</param>
        /// <return>
        ///     A string representing either the processed data or metadata, including chunking status and the unique
        ///     identifier.
        /// </return>
        public static string CheckByteCount(string uniqueId, int bufferSize, string data, string function = "",
            string entity = "", bool call = false)
        {
            var byteCount = Encoding.UTF8.GetByteCount(data);

            if (!data.StartsWith('[') || !data.EndsWith(']')) data = Main.SerializeList([.. data.Split(',')]);
            if (byteCount > bufferSize)
            {
                var chunks = SplitIntoChunks(data, bufferSize);
                var totalChunks = chunks.Count;

                for (var i = 0; i < totalChunks; i++)
                {
                    var escapedChunk = chunks[i].Replace("\"", "\"\"");
                    var chunk = $"[\"{uniqueId}\", \"{function}\", {i + 1}, {totalChunks}, \"{escapedChunk}\", {call.ToString().ToLower()}, \"{entity}\"]";
                    Main.Log($"Chunk data: {chunk}", "debug");
                    Main.Callback("ArmaDragonflyClient", "dragonfly_db_fnc_fetch", chunk);
                }

                return "OK";
            }

            Main.Log($"Data: {data}", "debug");
            return data;
        }

        /// <summary>
        ///     Processes a given message string for pub/sub operations, checks its byte size, and splits it into chunks if it exceeds the specified buffer
        ///     size.
        ///     If chunking is necessary, it sends each chunk to a callback function and returns metadata about the operation.
        /// </summary>
        /// <param name="uniqueId">A unique identifier used to associate the processed data.</param>
        /// <param name="message">The message string to be processed, analyzed for byte size, and optionally chunked.</param>
        /// <param name="eventType">The type of event associated with the processing.</param>
        /// <param name="eventName">The name of the event associated with the processing.</param>
        /// <param name="target">An optional target name linked to the processing operation, or null if it does not apply.</param>
        /// <param name="bufferSize">The maximum permissible byte size for the data before it is split into chunks.</param>
        /// <return>
        ///     A string representing either the processed data or metadata, including chunking status and the unique
        ///     identifier.
        /// </return>
        public static string CheckByteCountPubSub(string uniqueId, string message, string eventType, string eventName, string target = null, int bufferSize = Main.AdcBufferSize)
        {
            var byteCount = Encoding.UTF8.GetByteCount(message);

            if (!message.StartsWith('[') || !message.EndsWith(']')) message = Main.SerializeList([.. message.Split(',')]);
            if (byteCount > bufferSize)
            {
                var chunks = SplitIntoChunks(message, bufferSize);
                var totalChunks = chunks.Count;

                for (var i = 0; i < totalChunks; i++)
                {
                    var escapedChunk = chunks[i].Replace("\"", "\"\"");
                    var chunk = $"[\"{uniqueId}\", \"{eventType}\", {i + 1}, {totalChunks}, \"{escapedChunk}\", true, \"{target ?? ""}\"]";
                    Main.Log($"PubSub Chunk data: {chunk}", "debug");
                    Main.Callback("ArmaDragonflyClient", eventName, chunk);
                }

                return "OK";
            }

            Main.Log($"PubSub Data: {message}", "debug");
            return message;
        }
    }
}
