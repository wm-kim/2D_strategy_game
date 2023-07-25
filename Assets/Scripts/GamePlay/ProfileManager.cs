using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.Events;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    public class ProfileManager : NetworkBehaviour
    {
        [SerializeField] private PlayerProfileController m_myProfileController;
        [SerializeField] private PlayerProfileController m_opponentProfileController;
        
        [Header("Listening To")]
        [SerializeField] private VoidEventSO OnPlayerDataNetworkListChanged = default;

        public override void OnNetworkSpawn()
        {
            OnPlayerDataNetworkListChanged.OnEventRaised.AddListener(SetPlayerProfileClientRpc);
        }

        public override void OnNetworkDespawn()
        {
            OnPlayerDataNetworkListChanged.OnEventRaised.RemoveListener(SetPlayerProfileClientRpc);
        }
        
        [ClientRpc]
        private void SetPlayerProfileClientRpc()
        {
            if (ConnectionManager.Instance.IsConnected())
            {
                foreach (var playerData in ConnectionManager.Instance.GetPlayerDataNetworkList())
                {
                    if (playerData.ClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        m_myProfileController.SetPlayerName(playerData.PlayerName.ToString());
                    }
                    else
                    {
                        m_opponentProfileController.SetPlayerName(playerData.PlayerName.ToString());
                    }
                }
            }
        }
    }
}
