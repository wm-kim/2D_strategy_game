using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.UnityGamingService.Multiplayer;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.GridSystem
{
    public class ServerMap : NetworkBehaviour
    {
        [SerializeField] private ClientMap m_clientMap;
        [SerializeField] private PathFinding m_pathFinding;
        
        private ServerGrid m_serverGrid;
        
        public ServerCell this[Vector2Int coord] => m_serverGrid.Cells[coord.x, coord.y];

        public void GenerateMap(int mapSize)
        {
            if (!IsServer) return;
            m_serverGrid = new ServerGrid(mapSize, mapSize, m_pathFinding);
            DebugWrapper.Log($"Map Generated with size {mapSize}x{mapSize}");
            
            var sessionPlayers = SessionPlayerManager.Instance;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerNumber = sessionPlayers.GetPlayerNumber(clientId);
                var clientRpcParam = sessionPlayers.ClientRpcParams[clientId];

                if (playerNumber == 0) m_clientMap.GenerateMapClientRpc(mapSize, m_pathFinding, GridRotation.Default, clientRpcParam);
                else if (playerNumber == 1) m_clientMap.GenerateMapClientRpc(mapSize, m_pathFinding, GridRotation.Rotate180, clientRpcParam);
            }
        }

        public void PlaceUnitOnMap(int unitId, Vector2Int coord)
        {
            if (!IsServer) return;
            m_serverGrid.Cells[coord.x, coord.y].PlaceUnit(unitId);
        }
        
        /// <summary>
        /// Wrapper for <see cref="ServerGrid.GetPath"/>
        /// </summary>
        public List<ServerCell> GetPath(Vector2Int start, Vector2Int target) => m_serverGrid.GetPath(start, target);
    }
}