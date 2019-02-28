using System;
using System.Net;
using System.Net.Sockets;

namespace Silo
{
    /// <summary>
    /// Default implementation of <see cref="INetworkPortFinder"/>.
    /// </summary>
    public class NetworkPortFinder : INetworkPortFinder
    {
        public int GetAvailablePortFrom(int start, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                var port = start + i;
                if (TryPort(port))
                {
                    return port;
                }
            }
            return -1;
        }

        private bool TryPort(int port)
        {
            var listener = TcpListener.Create(port);
            try
            {
                listener.Start();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
