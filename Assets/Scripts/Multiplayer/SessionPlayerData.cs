using System;
using Unity.Collections;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;

namespace Minimax.Multiplayer
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        
        public SessionPlayerData(ulong clientId, string name, bool isConnected = false)
        {
            ClientID = clientId;
            PlayerName = name;
            IsConnected = isConnected;
        }
        
        public void Reinitialize() { }

        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }
    }
}
