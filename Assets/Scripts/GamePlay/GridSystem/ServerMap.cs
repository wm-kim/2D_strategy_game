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
        [SerializeField] private ClientMap   m_clientMap;
        [SerializeField] private PathFinding m_pathFinding;

        private ServerGrid m_serverGrid;

        public ServerCell this[Vector2Int coord] => m_serverGrid.Cells[coord.x, coord.y];

        public void GenerateMap(int mapSize)
        {
            if (!IsServer) return;
            m_serverGrid = new ServerGrid(mapSize, mapSize, m_pathFinding);
            DebugWrapper.Log($"Map Generated with size {mapSize}x{mapSize}");

            var sessionPlayers = SessionPlayerManager.Instance;
            foreach (var playerNumber in sessionPlayers.GetAllPlayerNumbers())
            {
                var clientRpcParam = sessionPlayers.ClientRpcParams[playerNumber];
                m_clientMap.GenerateMapClientRpc(mapSize, m_pathFinding, playerNumber, clientRpcParam);
            }
        }

        public void SetPlayersInitialPlaceableArea()
        {
            if (!IsServer) return;

            var sessionPlayers = SessionPlayerManager.Instance;
            var mapWidth       = m_serverGrid.Width;
            var mapHeight      = m_serverGrid.Height;
            foreach (var playerNumber in sessionPlayers.GetAllPlayerNumbers())
                if (playerNumber == 0)
                    for (var y = 0; y < mapHeight; y++)
                        m_serverGrid.Cells[0, y].IsPlaceable[playerNumber] = true;
                else if (playerNumber == 1)
                    for (var y = 0; y < mapHeight; y++)
                        m_serverGrid.Cells[mapWidth - 1, y].IsPlaceable[playerNumber] = true;
            m_clientMap.SetPlayersInitialPlaceableAreaClientRpc();
        }

        public void PlaceUnitOnMap(int unitId, Vector2Int coord)
        {
            if (!IsServer) return;
            m_serverGrid.Cells[coord.x, coord.y].PlaceUnit(unitId);
        }

        /// <summary>
        /// Wrapper for <see cref="ServerGrid.GetPath"/>
        /// </summary>
        public List<ServerCell> GetPath(Vector2Int start, Vector2Int target)
        {
            return m_serverGrid.GetPath(start, target);
        }
    }
}