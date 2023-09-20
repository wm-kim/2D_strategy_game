using System.Collections.Generic;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Unit;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.Logic
{
    /// <summary>
    /// Responsible for generating unit related commands and sending them to the clients.
    /// </summary>
    public class UnitLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField] private ServerMap m_serverMap;
        
        [Header("Client References")]
        [SerializeField] private ClientUnitManager m_clientUnit;
        
        [Header("Other Logic References")]
        [SerializeField] private MapLogic m_mapLogic;
        
        [ServerRpc(RequireOwnership = false)]
        public void CommandMoveUnitServerRpc(int unitUID, Vector2Int destCoord, ServerRpcParams serverRpcParams = default)
        {
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            var serverUnit = ServerUnit.UnitsCreatedThisGame[unitUID];
            
            // check if the unit is movable
            if (!serverUnit.IsMovable)
            {
                DebugWrapper.Log("Unit is not movable");
                return;
            }
            
            var path = m_serverMap.GetPath(serverUnit.Coord, destCoord);
            if (path.Count == 0) return;
            
            var serverCell = m_serverMap[serverUnit.Coord];
            serverCell.RemoveUnit();
            
            // TODO : add additional logic if there is event on the cell (in the future)
            serverUnit.Coord = destCoord;
            serverUnit.MoveRange -= path.Count;
            m_serverMap[destCoord].PlaceUnit(unitUID);
            
            SetUnitMovableClientRpc(unitUID, false);
            var clientRpcParams = SessionPlayerManager.Instance.ClientRpcParams;
            m_mapLogic.HideOverlayClientRpc(clientRpcParams[senderClientId]);
            for (int i = 0; i < path.Count; i++)
                MoveUnitOneCellClientRpc(unitUID, path[i].Coord);
            m_mapLogic.HighlightReachableCellsClientRpc(unitUID, clientRpcParams[senderClientId]);
            SetUnitMovableClientRpc(unitUID, true);
        }
        
        [ClientRpc]
        private void SetUnitMovableClientRpc(int unitUID, bool isMovable)
        {
            new SetUnitMovableCmd(unitUID, isMovable).AddToQueue();
        }
        
        [ClientRpc]
        private void MoveUnitOneCellClientRpc(int unitUID, Vector2Int destCoord)
        {
            new MoveUnitOneCellCmd(unitUID, destCoord, m_clientUnit).AddToQueue();
        }
    }
}