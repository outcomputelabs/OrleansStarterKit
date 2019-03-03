namespace Silo.Tests.Fakes
{
    public class FakeNetworkPortFinder : INetworkPortFinder
    {
        public int GetAvailablePortFrom(int start, int end)
        {
            return start;
        }
    }
}
