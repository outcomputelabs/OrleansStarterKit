using System;
using System.Net;
using System.Net.Sockets;

namespace Silo
{
    /// <summary>
    /// Default implementation of <see cref="INetworkHelper"/>.
    /// </summary>
    public class NetworkHelper : INetworkHelper
    {
        /// <inheritdoc />
        public int[] GetAvailablePorts(int count)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero.");

            var ports = new int[count];
            var listeners = new TcpListener[count];

            try
            {
                for (var i = 0; i < count; ++i)
                {
                    listeners[i] = TcpListener.Create(0);
                    listeners[i].Start();
                    ports[i] = ((IPEndPoint)listeners[i].LocalEndpoint).Port;
                }
            }
            finally
            {
                for (var i = 0; i < count; ++i)
                {
                    listeners[i]?.Stop();
                }
            }
            return ports;
        }
    }
}
