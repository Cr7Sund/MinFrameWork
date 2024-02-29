
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;

namespace Cr7Sund.Logger
{
    /// <summary>
    /// Send log events as UDP packages over the network.
    /// </summary>
    internal class UDPSink : IBatchedLogEventSink, IDisposable
    {
        private readonly IUdpClient _client;
        private readonly RemoteEndPoint remoteEndPoint;
        private readonly Encoding _encoding;
        private readonly ITextFormatter _formatter;

        public UDPSink(
            IUdpClient client,
            string remoteAddress,
            int remotePort,
            ITextFormatter formatter,
            Encoding encoding)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            remoteEndPoint = new RemoteEndPoint(remoteAddress, remotePort);
            _encoding = encoding;
            _formatter = formatter;
        }

        #region IBatchedLogEventSink Members

        public async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (LogEvent logEvent in events)
            {
                try
                {
                    using var stringWriter = new StringWriter();
                    
                    _formatter.Format(logEvent, stringWriter);
                    var buffer = _encoding.GetBytes(stringWriter.ToString());

                    if (remoteEndPoint.IPEndPoint != null)
                    {
                        await _client
                            .SendAsync(buffer, buffer.Length, remoteEndPoint.IPEndPoint)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        await _client
                            .SendAsync(buffer, buffer.Length, remoteEndPoint.Address, remoteEndPoint.Port)
                            .ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Failed to send UDP package. ");
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        public Task OnEmptyBatchAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
#if NET4
            // IUdpClient does not implement IDisposable, but calling Close disables the
            // underlying socket and releases all managed and unmanaged resources associated
            // with the instance.
            client?.Close();
#else
                _client?.Dispose();
#endif
            }
        }
    }
}