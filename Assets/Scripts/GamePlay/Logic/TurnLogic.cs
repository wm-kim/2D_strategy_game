using Minimax.GamePlay.GridSystem;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay.Logic
{
    public class TurnLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField] private TurnManager m_turnManager;
        
        [Header("Client References")]
        [SerializeField] private ClientMap m_clientMap;
        
        [Header("Other Logic References")]
        [SerializeField] private CardDrawingLogic m_cardDrawingLogic;
        
        public override void OnNetworkSpawn()
        {
            m_turnManager.OnServerTurnStart += OnServerTurnStart;
            m_turnManager.OnClientTurnStart += OnClientTurnStart;
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            m_turnManager.OnServerTurnStart -= OnServerTurnStart;
            m_turnManager.OnClientTurnStart -= OnClientTurnStart;
            base.OnNetworkDespawn();
        }
        
        private void OnServerTurnStart(int playerNumber)
        {
            m_cardDrawingLogic.CommandDrawACardFromDeck(playerNumber);
        }
        
        private void OnClientTurnStart(int playerNumber)
        {
            if (m_turnManager.MyPlayerNumber != playerNumber)
            {
                m_clientMap.DisableHighlightCells();
            }
        }
    }
}