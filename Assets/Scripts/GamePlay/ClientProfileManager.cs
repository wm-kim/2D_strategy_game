using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkPlayerProfile))]
    public class ClientProfileManager : MonoBehaviour
    {
        private NetworkPlayerProfile m_networkPlayerProfile;
        private NetcodeHooks m_netcodeHooks;
        
        [SerializeField] private TMP_Text m_myPlayerNameText;
        [SerializeField] private TMP_Text m_opponentPlayerNameText;
        
        private void Awake()
        {
            m_networkPlayerProfile = GetComponent<NetworkPlayerProfile>();
            m_netcodeHooks = GetComponent<NetcodeHooks>();
            
            m_netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_netcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        private void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
            else
            {
                m_networkPlayerProfile.PlayerProfiles.OnListChanged += OnPlayerProfilesChanged;
            }
        }
        
        private void OnNetworkDespawn()
        {
            if (m_networkPlayerProfile)
            {
                m_networkPlayerProfile.PlayerProfiles.OnListChanged -= OnPlayerProfilesChanged;
            }
        }
        
        private void OnPlayerProfilesChanged(NetworkListEvent<NetworkPlayerProfile.PlayerProfileData> changeEvent)
        {
            if (changeEvent.Type == NetworkListEvent<NetworkPlayerProfile.PlayerProfileData>.EventType.Add)
            {
                if (changeEvent.Value.ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    m_myPlayerNameText.text = changeEvent.Value.PlayerName;
                }
                else
                {
                    m_opponentPlayerNameText.text = changeEvent.Value.PlayerName;
                }
            }
        }
    }
}
