using Minimax.CoreSystems;
using Minimax.UnityGamingService.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace Minimax.GamePlay.GridSystem
{
    public class ServerMap : NetworkBehaviour
    {
        [SerializeField] private ClientMap m_clientMap;
        
        private ServerCell[,] m_cells;

        public void GenerateMap(int mapSize)
        {
            if (!IsServer) return;
            
            m_cells = new ServerCell[mapSize, mapSize];
            
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    m_cells[x, y] = new ServerCell(x, y);
                }
            }
            
            var sessionPlayers = SessionPlayerManager.Instance;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerNumber = sessionPlayers.GetPlayerNumber(clientId);
                var clientRpcParam = sessionPlayers.ClientRpcParams[clientId];

                if (playerNumber == 0) m_clientMap.GenerateMapClientRpc(mapSize, GridRotation.Default, clientRpcParam);
                else if (playerNumber == 1) m_clientMap.GenerateMapClientRpc(mapSize, GridRotation.Rotate180, clientRpcParam);
            }
        }

        public void PlaceUnitOnMap(int unitId, Vector2Int coord)
        {
            if (!IsServer) return;
            m_cells[coord.x, coord.y].PlaceUnit(unitId);
        }
    }
}