using System.Net.Sockets;

namespace Silo
{
    /// <summary>
    /// Default implementation of <see cref="INetworkPortFinder"/>.
    /// </summary>
    public class NetworkPortFinder : INetworkPortFinder
    {
        public int GetAvailablePortFrom(int start, int end)
        {
            var length = end - start;
            for (var i = 0; i <= length; ++i)
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
                listener.ExclusiveAddressUse = true;
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
