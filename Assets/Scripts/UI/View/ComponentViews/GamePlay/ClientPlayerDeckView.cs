using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Minimax
{
    /// <summary>
    /// This class is a view component for the client player's deck.
    /// </summary>
    public class ClientPlayerDeckView : NetworkBehaviour
    {
        private List<int> m_cardIds = new List<int>();
        
        [ClientRpc]
        private void SetupDeckListViewClientRpc(int[] cardIds, ClientRpcParams clientRpcParams = default)
        {
            m_cardIds = new List<int>(cardIds);
        }
    }
}
