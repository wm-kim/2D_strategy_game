using System;
using Minimax.UnityGamingService.Multiplayer;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkPlayerProfile))]
    public class ServerProfileManager : NetworkBehaviour
    {
        private NetworkPlayerProfile m_networkPlayerProfile;
        private NetcodeHooks m_netcodeHooks;

        private void Awake()
        {
            m_networkPlayerProfile = GetComponent<NetworkPlayerProfile>();
            m_netcodeHooks = GetComponent<NetcodeHooks>();
            
            m_netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_netcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (m_netcodeHooks)
            {
                m_netcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                m_netcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
        }

        private void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
            }
            else // if Server
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }

        private void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            // We need to filter out the event that are not a client has finished loading the scene
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            AddNewPlayerProfile(sceneEvent.ClientId);
        }
        
        private void AddNewPlayerProfile(ulong clientId)
        {
            SessionPlayerData? sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
            if (sessionPlayerData.HasValue)
            {
                var playerData = sessionPlayerData.Value;
                m_networkPlayerProfile.PlayerProfiles.Add(new NetworkPlayerProfile.PlayerProfileData(clientId, playerData.PlayerName));
                DebugWrapper.Log($"Added new player profile for {clientId} with name {playerData.PlayerName}");
            }
        }
        
        private void OnClientDisconnectCallback(ulong clientId)
        {
            for (int i = 0; i < m_networkPlayerProfile.PlayerProfiles.Count; ++i)
            {
                var playerData = m_networkPlayerProfile.PlayerProfiles[i];
                if (playerData.ClientId == clientId)
                {
                    m_networkPlayerProfile.PlayerProfiles.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
