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
        /// <param name="count">The number of available ports to discover.</param>
        /// <returns>The given number of available ports.</returns>
        int[] GetAvailablePorts(int count);
    }
}
