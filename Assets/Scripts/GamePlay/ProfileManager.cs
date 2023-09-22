using Minimax.UnityGamingService.Multiplayer;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay
{
    public class ProfileManager : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_myPlayerNameText;
        [SerializeField] private TextMeshProUGUI m_opponentPlayerNameText;
        
        private NetworkManager m_networkManager = NetworkManager.Singleton;
        
        public void SetPlayersName()
        {
            if (!IsServer) return;
            
            foreach (var clientId in m_networkManager.ConnectedClientsIds)
            {
                var playerData = SessionPlayerManager.Instance.GetPlayerData(clientId);
                if (!playerData.HasValue) return;
                var playerName = playerData.Value.PlayerName;
                SetPlayersNameClientRpc(clientId, playerName);
            }
        }
        
        [ClientRpc]
        private void SetPlayersNameClientRpc(ulong clientId, string playerName)
        {
            if (clientId == m_networkManager.LocalClientId) m_myPlayerNameText.text = playerName;
            else m_opponentPlayerNameText.text = playerName;
        }
    }
}
