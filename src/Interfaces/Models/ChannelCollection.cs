using System;
using System.Collections.ObjectModel;

namespace Interfaces.Models
{
    public class ChannelCollection : KeyedCollection<Guid, Channel>
    {
        protected override Guid GetKeyForItem(Channel item)
        {
            return item.Id;
        }
    }
}
