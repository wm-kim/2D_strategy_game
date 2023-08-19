using System;
using Minimax.UnityGamingService.Multiplayer;
using TMPro;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class ProfileManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_myPlayerNameText;
        [SerializeField] private TextMeshProUGUI m_opponentPlayerNameText;
        
        private NetworkManager m_networkManager = NetworkManager.Singleton;
        
        [ClientRpc]
        public void SetMyPlayerNameClientRpc(ulong clientId, string playerName)
        {
            if (clientId == m_networkManager.LocalClientId) m_myPlayerNameText.text = playerName;
            else m_opponentPlayerNameText.text = playerName;
        }
    }
}
