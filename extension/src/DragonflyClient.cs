using System.Net.Sockets;
using System.Text;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ArmaDragonflyClient
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class DragonflyClient(string host, int port, string password) : IDisposable
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        public void Connect()
        {
            if (_client?.Connected == true) return;
            _client = new TcpClient();
            _client.Connect(host, port);
            _reader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _writer = new StreamWriter(_client.GetStream(), Encoding.ASCII) { AutoFlush = true };

            if (!string.IsNullOrEmpty(password))
            {
                if (SendCommand($"AUTH {password}") != "OK")
                    throw new InvalidOperationException("Invalid password provided.");
            }

            Main.Log("Connected to DragonflyDB.", "debug");
        }

        public string SendCommand(string command, bool convertFromBase64 = false)
        {
            if (_client?.Connected != true)
                Connect();

            _writer!.WriteLine(command);
            return ParseResponse(_reader!.ReadLine(), _reader, convertFromBase64);
        }

        public string ReceiveMessage()
        {
            var response = _reader.ReadLine();
            return ParseResponse(response, _reader);
        }

        private static string ParseResponse(string response, StreamReader reader, bool convertFromBase64 = false)
        {
            if (response == null)
                throw new InvalidOperationException("Null response received.");

            switch (response[0])
            {
                case '$':
                    var count = int.Parse(response[1..]);
                    if (count == -1)
                        return null!;

                    var buffer = new char[count];
                    reader.Read(buffer, 0, count);
                    reader.ReadLine();
                    return new string(buffer);

                case '*':
                    var arrayLength = int.Parse(response[1..]);
                    if (arrayLength == -1) return null!;

                    var elements = new List<string>();
                    for (var i = 0; i < arrayLength; i++)
                    {
                        var element = ParseResponse(reader.ReadLine(), reader, convertFromBase64);

                        if (convertFromBase64)
                        {
                            var data = Convert.FromBase64String(element!);
                            var originalString = Encoding.UTF8.GetString(data);
                            elements.Add(originalString);
                        }
                        else
                        {
                            elements.Add(element!);
                        }
                    }

                    var output = elements.Count > 0
                        ? $"[{string.Join(",", elements.Select(e => e.StartsWith('[') ? e : $"\"{e}\""))}]"
                        : "";
                    return output;

                case '+':
                case '-':
                    return response[1..];

                case ':':
                    return response[1..].Trim();

                default:
                    throw new NotSupportedException($"Unsupported response type: {response}");
            }
        }

        private void Disconnect()
        {
            _writer?.Close();
            _reader?.Close();
            _client?.Close();

            _writer?.Dispose();
            _reader?.Dispose();
            _client?.Dispose();

            _writer = null;
            _reader = null;
            _client = null;

            Main.Log("Disconnected from DragonflyDB.", "debug");
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}