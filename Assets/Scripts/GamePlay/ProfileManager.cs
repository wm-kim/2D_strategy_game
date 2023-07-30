using Minimax.Multiplayer.ConnectionManagement;
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

        
    }
}
