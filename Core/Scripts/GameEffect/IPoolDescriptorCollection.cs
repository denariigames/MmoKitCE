using System.Collections.Generic;

namespace MultiplayerARPG
{
    public interface IPoolDescriptorCollection
    {
        IEnumerable<IPoolDescriptor> PoolDescriptors { get; }
    }
}
