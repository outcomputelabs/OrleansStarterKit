using Orleans;

namespace Grains
{
    /// <summary>
    /// Represents a user account in the system.
    /// </summary>
    public interface IUser : IGrainWithStringKey
    {
    }
}
