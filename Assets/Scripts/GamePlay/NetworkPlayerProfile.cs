using System;
using Minimax.CoreSystems;
using Unity.Netcode;

namespace Minimax.GamePlay
{
    public class NetworkPlayerProfile : NetworkBehaviour
    {
        /// <summary>
        /// Describes profile data for a player in the gameScene.
        /// </summary>
        public struct PlayerProfileData : INetworkSerializable, IEquatable<PlayerProfileData>
        {
            public ulong ClientId;

            private NetworkString m_playerName;

            public string PlayerName
            {
                get => m_playerName;
                private set => m_playerName = value;
            }

            public PlayerProfileData(ulong clientId, string name)
            {
                ClientId = clientId;

                m_playerName = new NetworkString();
                m_playerName = name;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref m_playerName);
            }

            public bool Equals(PlayerProfileData other)
            {
                return ClientId == other.ClientId &&
                       PlayerName.Equals(other.PlayerName);
            }
        }

        private NetworkList<PlayerProfileData> m_playerProfiles;

        /// <summary>
        /// Current state of all player profiles in the gameScene.
        /// </summary>
        public NetworkList<PlayerProfileData> PlayerProfiles => m_playerProfiles;

        private void Awake()
        {
            m_playerProfiles = new NetworkList<PlayerProfileData>();
        }
    }
}