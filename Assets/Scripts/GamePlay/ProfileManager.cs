using System;
using Minimax.UnityGamingService.Multiplayer;
using Nova;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class ProfileManager : NetworkBehaviour
    {
        [SerializeField] private TextBlock m_myPlayerNameText;
        [SerializeField] private TextBlock m_opponentPlayerNameText;
        
        private NetworkManager m_networkManager = NetworkManager.Singleton;
        
        [ClientRpc]
        public void SetMyPlayerNameClientRpc(ulong clientId, string playerName)
        {
            if (clientId == m_networkManager.LocalClientId) m_myPlayerNameText.Text = playerName;
            else m_opponentPlayerNameText.Text = playerName;
        }
    }
}
