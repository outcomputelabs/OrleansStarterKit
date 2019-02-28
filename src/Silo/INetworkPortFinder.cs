namespace Silo
{
    /// <summary>
    /// Interface for services that help find available ports.
    /// </summary>
    public interface INetworkPortFinder
    {
        /// <summary>
        /// Discovers the given number of available ports.
        /// </summary>
        /// <param name="start">The start of the port range to search for.</param>
        /// <param name="end">The end of the port range to search for.</param>
        /// <returns>The first available port in the given range or -1 if no port is available.</returns>
        int GetAvailablePortFrom(int start, int end);
    }
}
