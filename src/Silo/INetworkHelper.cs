namespace Silo
{
    /// <summary>
    /// Provides helper utilities for managing the network.
    /// </summary>
    public interface INetworkHelper
    {
        /// <summary>
        /// Discovers the given number of available ports.
        /// </summary>
        /// <param name="count">The number of available ports to discover.</param>
        /// <returns>The given number of available ports.</returns>
        int[] GetAvailablePorts(int count);
    }
}
