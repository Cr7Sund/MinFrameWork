using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Cr7Sund.Logger
{
    internal class UDPClientProxy : IUdpClient
    {
        private readonly UdpClient client;

        public UDPClientProxy(int localPort, AddressFamily family, bool enableBroadcast)
        {
            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort) throw new ArgumentOutOfRangeException(nameof(localPort));


            client = localPort == 0
                ? new UdpClient(family)
                : new UdpClient(localPort, family);

            // Allow for IPv4 mapped addresses over IPv6
            if (family == AddressFamily.InterNetworkV6)
            {
                client.Client.DualMode = true;
            }

            // Enable broadcasting
            client.EnableBroadcast = enableBroadcast;
        }

        public Socket Client => client.Client;

        public Task<int> SendAsync(byte[] datagram, int bytes, IPEndPoint endPoint)
        {
            return client.SendAsync(datagram, bytes, endPoint);
        }

        public Task<int> SendAsync(byte[] datagram, int bytes, string hostname, int port)
        {
            return client.SendAsync(datagram, bytes, hostname, port);
        }

#if NET4
        public void Close()
        {
            client.Close();
        }
#else
        public void Dispose()
        {
            client.Dispose();
        }
#endif
    }
}