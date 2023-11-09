using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.PlayerDeck;
using Minimax.GamePlay.Unit;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay.Logic
{
    public class TurnLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField]
        private TurnManager m_turnManager;

        [SerializeField]
        private ServerPlayersDeckManager m_serverPlayersDeck;

        [SerializeField]
        private ServerManaManager m_serverManaManager;

        [Header("Client References")]
        [SerializeField]
        private ClientUnitManager m_clientUnitManager;

        [SerializeField]
        private UnitControlPanelController m_unitControlPanelController;

        [SerializeField]
        private TurnNotifyView m_turnNotifyView;

        [SerializeField]
        private ClientMap m_clientMap;

        [Header("Other Logic References")]
        [SerializeField]
        private CardDrawingLogic m_cardDrawingLogic;

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
            m_serverManaManager.IncrementManaCapacity(playerNumber);
            m_serverManaManager.RefillMana(playerNumber);

            ServerResetUnitOnTurnStart(playerNumber);

            if (m_serverPlayersDeck.IsCardLeftInDeck(playerNumber))
                m_cardDrawingLogic.CommandDrawACardFromDeck(playerNumber);
        }

        private void OnClientTurnStart(int playerNumber)
        {
            m_clientMap.DisableHighlightCells();

            if (playerNumber == m_turnManager.MyPlayerNumber)
            {
                if (m_clientUnitManager.IsUnitSelected())
                {
                    var unitUID = m_clientUnitManager.CurrentUnitUID;
                    m_unitControlPanelController.ResetAndShowIfMyUnit(unitUID);
                }
            }
            else
            {
                m_unitControlPanelController.ResetAndHide();
            }
            
            m_clientUnitManager.ResetAllUnitsOnTurnStart();
            m_turnNotifyView.Notify(TurnManager.Instance.IsMyTurn ? "Your Turn" : "Opponent's Turn");
        }

        private void ServerResetUnitOnTurnStart(int playerNumber)
        {
            var units = ServerUnit.GetAllUnitsByOwner(playerNumber);
            foreach (var unit in units) unit.ResetOnTurnStart();
        }
    }
}