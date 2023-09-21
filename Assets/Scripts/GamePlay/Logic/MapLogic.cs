using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay.Logic
{
    /// <summary>
    /// Responsible for generating commands related to the map and sending them to the clients.
    /// </summary>
    public class MapLogic : NetworkBehaviour
    {
        [Header("Client References")]
        [SerializeField] private ClientMap m_clientMap;
        
        [ClientRpc] 
        public void HighlightReachableCellsClientRpc(int unitUID, ClientRpcParams clientRpcParams = default)
        {
            new HighlightReachableCmd(unitUID, m_clientMap).AddToQueue();
        }
        
        [ClientRpc]
        public void DisableAllHighlightsClientRpc()
        {
            new DisableAllHighlightOverlayCmd(m_clientMap).AddToQueue();
        }
    }
}