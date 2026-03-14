using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct UIEquipmentSocketsData
    {
        public List<int> sockets;
        public SocketEnhancerType[] availableSocketEnhancerTypes;
        public UIEquipmentSocketsData(List<int> sockets, SocketEnhancerType[] availableSocketEnhancerTypes)
        {
            this.sockets = sockets;
            this.availableSocketEnhancerTypes = availableSocketEnhancerTypes;
        }
    }
}
