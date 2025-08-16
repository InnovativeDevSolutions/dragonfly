#pragma warning disable IDE0130 // Namespace does not match folder structure
using System.Text;

namespace ArmaDragonflyClient
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class DragonflyDB
    {
        private static readonly DragonflyClient _client = new(Main.AdcHost, Main.AdcPort, Main.AdcPassword);
        private static CancellationTokenSource _listenerCts;

        #region Generic
        public static string Raw(string cmd, params string[] args)
        {
            StringBuilder command = new(cmd);
            foreach (var arg in args)
            {
                command.Append($" {arg}");
            }

            _client.Connect();
            var response = _client.SendCommand(command.ToString());

            return response;
        }

        public static int Exists(params string[] keys)
        {
            StringBuilder command = new("EXISTS");
            foreach (var key in keys)
            {
                command.Append($" {key}");
            }

            _client.Connect();
            var response = _client.SendCommand(command.ToString());

            return int.Parse(response);
        }

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'function' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"GET {key}");

            return response;
        }

        public static string Set(string key, string keyValue)
        {
            _client.Connect();
            var response = _client.SendCommand($"SET {key} {keyValue}");

            return response;
        }

        public static int Del(params string[] keys)
        {
            StringBuilder command = new("DEL");
            foreach (var key in keys)
            {
                command.Append($" {key}");
            }

            _client.Connect();
            var response = _client.SendCommand(command.ToString());

            return int.Parse(response);
        }

        public static string Save()
        {
            _client.Connect();
            var response = _client.SendCommand("SAVE");

            Main.Log($"ArmaDragonflyClient 'SAVE', Response: '{response}'", "debug");
            return response;
        }
        #endregion


        #region List
        public static int LPush(string key, string keyValue)
        {
            _client.Connect();
            var response = _client.SendCommand($"LPUSH {key} {Utils.Base64Encode(keyValue)}");

            return int.Parse(response);
        }

        public static int RPush(string key, string keyValue)
        {
            _client.Connect();
            var response = _client.SendCommand($"RPUSH {key} {Utils.Base64Encode(keyValue)}");

            return int.Parse(response);
        }

        public static string LIndex(string key, string keyIndex)
        {

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(keyIndex))
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'keyIndex' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"LINDEX {key} {keyIndex}", true);
            var decodedResponse = Utils.Base64Decode(response);

            return decodedResponse;
        }

        public static int LLen(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Main.Log($"ArmaDragonflyClient 'key' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"LLEN {key}");

            return int.Parse(response);
        }

        public static List<string> LRange(string key, string keyStart, string keyStop)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(keyStart) || string.IsNullOrEmpty(keyStop))
            {
                Main.Log($"ArmaDragonflyClient 'key', 'keyStart' or 'keyStop' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"LRANGE {key} {keyStart} {keyStop}", true);

            return [.. response.Split(',')];
        }

        public static int LRem(string key, string keyIndex)
        {
            _client.Connect();
            _client.SendCommand($"LSET {key} {keyIndex} DRAGONFLYREMOVE");
            var response = _client.SendCommand($"LREM {key} 0 DRAGONFLYREMOVE");

            return int.Parse(response);
        }

        public static string LSet(string key, string keyIndex, string keyValue)
        {
            _client.Connect();
            var response = _client.SendCommand($"LSET {key} {keyIndex} {Utils.Base64Encode(keyValue)}");

            return response;
        }
        #endregion

        #region Hash
        public static int HExists(string key, string keyField)
        {
            _client.Connect();
            var response = _client.SendCommand($"HEXISTS {key} {keyField}");

            return int.Parse(response);
        }

        public static int HSet(string key, string keyField, string keyValue)
        {
            _client.Connect();
            var response = _client.SendCommand($"HSET {key} {keyField} {keyValue}");

            return int.Parse(response);
        }

        // ReSharper disable once InconsistentNaming
        public static string HMSet(string key, Dictionary<string, string> fieldValues)
        {
            if (string.IsNullOrEmpty(key) || fieldValues == null || fieldValues.Count == 0)
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'fieldValues' cannot be empty", "debug");
            }

            StringBuilder command = new($"HMSET {key}");
            if (fieldValues != null)
                foreach (var kvp in fieldValues)
                {
                    command.Append($" {kvp.Key} {kvp.Value}");
                }

            _client.Connect();
            var response = _client.SendCommand(command.ToString());

            return response;
        }

        public static string HGet(string key, string keyField)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(keyField))
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'keyField' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HGET {key} {keyField}");

            return response;
        }

        public static List<string> HGetAll(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Main.Log($"ArmaDragonflyClient 'key' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HGETALL {key}");

            return [.. response.Split(',')];
        }

        public static int HDel(string key, string keyField)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(keyField))
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'keyField' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HDEL {key} {keyField}");

            return int.Parse(response);
        }

        public static int HIncrBy(string key, string keyField, int value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(keyField))
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'keyField' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HINCRBY {key} {keyField} {value}");

            return int.Parse(response);
        }

        public static string HIncrByFloat(string key, string keyField, int value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(keyField))
            {
                Main.Log($"ArmaDragonflyClient 'key' or 'keyField' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HINCRBYFLOAT {key} {keyField} {value}");

            return response;
        }

        public static int HLen(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Main.Log($"ArmaDragonflyClient 'key' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HLEN {key}");

            return int.Parse(response);
        }

        public static List<string> HKeys(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Main.Log($"ArmaDragonflyClient 'key' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HKEYS {key}");

            return [.. response.Split(',')];
        }

        public static List<string> HVals(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Main.Log($"ArmaDragonflyClient 'key' cannot be empty", "debug");
            }

            _client.Connect();
            var response = _client.SendCommand($"HVALS {key}");

            return [.. response.Split(',')];
        }
        #endregion

        #region Pub/Sub
        public static void DragonflyPublish(string channel, string message, string uniqueId)
        {
            _client.Connect();
            _client.SendCommand($"PUBLISH {channel} {message}");

            Main.Log($"ArmaDragonflyClient 'PUBLISH', Channel: '{channel}', Message: '{message}'", "debug");
        }

        public static void DragonflySubscribe(string channel, string eventType, string eventName, string uniqueId, string target = null, int bufferSize = Main.AdcBufferSize)
        {
            _client.Connect();

            try
            {
                _client.SendCommand($"SUBSCRIBE {channel}");
                Main.Log($"ArmaDragonflyClient 'SUBSCRIBE', Channel: '{channel}'", "debug");

                _listenerCts = new CancellationTokenSource();

                Task.Run(() =>
                {
                    StartMessageListener(message =>
                    {
                        Utils.CheckByteCountPubSub(uniqueId, message, eventType, eventName, target, bufferSize);
                    });
                });
            }
            catch (Exception ex)
            {
                Main.Log($"ArmaDragonflyClient 'SUBSCRIBE', Exception: '{ex.Message}'", "error");
                throw;
            }
        }

        private static void StartMessageListener(Action<string> messageHandler)
        {
            try
            {
                while (!_listenerCts.Token.IsCancellationRequested)
                {
                    var response = _client.ReceiveMessage();

                    if (!string.IsNullOrEmpty(response))
                    {
                        var messageParts = response.Split(',');
                        if (messageParts.Length >= 3)
                        {
                            var messageType = messageParts[0];
                            var channel = messageParts[1];
                            var message = messageParts[2];

                            Main.Log($"ArmaDragonflyClient 'SUBSCRIBE', Channel: '{channel}', Message: '{message}'", "debug");
                            messageHandler(message);
                        }
                    }

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Main.Log($"ArmaDragonflyClient 'SUBSCRIBE', Exception: '{ex.Message}'", "error");
                throw;
            }
        }

        public static void StopMessageListener()
        {
            _listenerCts.Cancel();
            _listenerCts.Dispose();
            _listenerCts = null;
        }
        #endregion
    }
}