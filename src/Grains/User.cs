using Interfaces;
using Orleans;

namespace Grains
{
    public class User : Grain, IUser
    {
    }
}
