using System.Linq;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Unit;
using Minimax.UnityGamingService.Multiplayer;
using Unity.Netcode;
using UnityEngine;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.Logic
{
    /// <summary>
    /// Responsible for generating unit related commands and sending them to the clients.
    /// </summary>
    public class UnitLogic : NetworkBehaviour
    {
        [Header("Server References")]
        [SerializeField]
        private ServerMap m_serverMap;

        [SerializeField]
        private TurnManager m_turnManager;

        [Header("Client References")]
        [SerializeField]
        private ClientUnitManager m_clientUnit;

        [SerializeField]
        private ClientMap m_clientMap;

        [ServerRpc(RequireOwnership = false)]
        public void CommandMoveUnitServerRpc(int unitUID, Vector2Int destCoord,
            ServerRpcParams serverRpcParams = default)
        {
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            m_turnManager.CheckIfPlayerTurn(senderClientId);

            var serverUnit = ServerUnit.UnitsCreatedThisGame[unitUID];
            if (!CheckIsUnitOwner(serverUnit, senderClientId)) return;

            if (!serverUnit.CheckIfMovable()) return;

            // check if the destination is reachable
            var path = m_serverMap.GetPath(serverUnit.Coord, destCoord);
            if (path.Count == 0) return;

            // remove the unit from the current cell and place it on the destination cell
            var serverCell = m_serverMap[serverUnit.Coord];
            serverCell.RemoveUnit();

            // TODO : add additional logic if there is event on the cell (in the future)
            serverUnit.Coord     =  destCoord;
            serverUnit.MoveRange -= path.Count;
            m_serverMap[destCoord].PlaceUnit(unitUID);

            var pathCoords = path.Select(t => t.Coord).ToList();
            MoveUnitClientRpc(unitUID, pathCoords.ToArray());
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommandAttackUnitServerRpc(int attackerUID, int targetUntiUID,
            ServerRpcParams serverRpcParams = default)
        {
            Debug.Log("Server received CommandAttackUnitServerRpc");
            var senderClientId = serverRpcParams.Receive.SenderClientId;
            m_turnManager.CheckIfPlayerTurn(senderClientId);

            var attackerUnit = ServerUnit.UnitsCreatedThisGame[attackerUID];
            if (!CheckIsUnitOwner(attackerUnit, senderClientId)) return;

            // TODO : add checking if unit has ability to attack

            var targetUnit = ServerUnit.UnitsCreatedThisGame[targetUntiUID];
            targetUnit.Health -= attackerUnit.Attack;
            AttackUnitClientRpc(attackerUID, targetUntiUID);
        }

        private bool CheckIsUnitOwner(ServerUnit serverUnit, ulong senderClientId)
        {
            var sessionPlayers = SessionPlayerManager.Instance;
            var playerNumber   = sessionPlayers.GetPlayerNumber(senderClientId);
            var isUnitOwner    = serverUnit.Owner == playerNumber;
            if (!isUnitOwner) Debug.Log($"Player {playerNumber} is not the owner of unit {serverUnit.UID}");
            return isUnitOwner;
        }

        [ClientRpc]
        private void MoveUnitClientRpc(int unitUID, Vector2Int[] path)
        {
            new MoveUnitCmd(unitUID, path, m_clientUnit, m_clientMap).AddToQueue();
        }

        [ClientRpc]
        private void AttackUnitClientRpc(int attackerUID, int targetUnitUID)
        {
            new AttackUnitCmd(attackerUID, targetUnitUID, m_clientUnit).AddToQueue();
        }
    }
}