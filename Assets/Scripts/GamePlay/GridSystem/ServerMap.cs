using Minimax.CoreSystems;
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
            
            var connection = GlobalManagers.Instance.Connection;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var playerNumber = connection.GetPlayerNumber(clientId);
                var clientRpcParam = connection.ClientRpcParams[clientId];

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