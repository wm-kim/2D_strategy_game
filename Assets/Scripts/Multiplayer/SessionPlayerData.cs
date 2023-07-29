using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public struct SessionPlayerData : IEquatable<SessionPlayerData>, INetworkSerializable
    {
        public ulong ClientId;
        public FixedString64Bytes PlayerName;
        public FixedString64Bytes PlayerId;

        public bool Equals(SessionPlayerData other) {
            return 
                ClientId == other.ClientId && 
                PlayerName == other.PlayerName &&
                PlayerId == other.PlayerId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerId);
        }
    }
}
