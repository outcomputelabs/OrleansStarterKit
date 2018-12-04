using System;
using System.Collections.ObjectModel;

namespace Interfaces.Models
{
    public class ChannelCollection : KeyedCollection<Guid, ChannelInfo>
    {
        protected override Guid GetKeyForItem(ChannelInfo item)
        {
            return item.Id;
        }
    }
}
