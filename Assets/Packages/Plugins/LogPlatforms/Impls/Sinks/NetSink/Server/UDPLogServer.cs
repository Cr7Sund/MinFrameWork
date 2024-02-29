using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cr7Sund.Logger.Server
{
    public class UDPLogServer : IDisposable
    {
        private UdpClient _udpClient;
        private Encoding _encoding = Encoding.UTF8;
        private Task _receiveTask;
        private CancellationTokenSource _cts;

        public async void Dispose()
        {
            await _receiveTask;
            _udpClient.Close();
            _cts?.Cancel();

        }

        public void InitUDPClient()
        {
            int port = 7071;

            AddressFamily addressFamily = AddressFamily.InterNetwork; // IPv4
            _udpClient = new UdpClient(addressFamily);
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

            // 创建一个 CancellationTokenSource
            _cts = new CancellationTokenSource();

        }

        public void RunReceiveTask()
        {
            _receiveTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    await ReceiveMessage().ConfigureAwait(false);
                }
            });
        }

        private async Task ReceiveMessage()
        {
            try
            {
                UdpReceiveResult result = await _udpClient.ReceiveAsync();

                byte[] receivedData = result.Buffer;
                IPEndPoint remoteEP = result.RemoteEndPoint;

                var logEntry = JsonUtility.FromJson<LogEntryData>(Encoding.UTF8.GetString(receivedData));

                string receivedMessage = Encoding.UTF8.GetString(receivedData);

                DateTime Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(logEntry.TimeStamp).ToLocalTime();
                string LogChannel = logEntry.LogChannel;
                string Level = LogLevelUtil.GetLogLevel(logEntry.Level);
                string Message = logEntry.Message;
                string Ex = logEntry.ExceptionMsg;

                string message = $"[{remoteEP}]: {Timestamp:HH:mm:ss} [{LogChannel}] [{Level:u3}] {Message:lj}{Ex}";

                message = string.Format("<color=#{0}>{1}</color>", logEntry.Color, message);

                if (logEntry.Level < 3)
                {
                    UnityEngine.Debug.Log(message);
                }
                else if (logEntry.Level == 3)
                {
                    UnityEngine.Debug.LogWarning(message);
                }
                else
                {
                    UnityEngine.Debug.LogError(message);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Error when receive message：{e}");
            }
        }

    }

}
