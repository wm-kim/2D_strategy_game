using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Unit;
using UnityEngine;

namespace Minimax
{
    /// <summary>
    /// Responsible for spawning units and visualizing them
    /// </summary>
    public class ClientUnitManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitVisual unitVisualPrefab;
        [SerializeField] private ClientMap m_clientMap;
        [SerializeField] private Transform m_unitContainer;
        
        /// <summary>
        /// Key is unit UID
        /// </summary>
        private Dictionary<int, UnitVisual> m_unitVisuals = new Dictionary<int, UnitVisual>();
        
        public event Action<ClientCell> OnUnitSpawned;
        public event Action<ClientCell> OnUnitSelected;

        private void OnEnable()
        {
            m_clientMap.OnTapMap += OnTapMap;
        }
        
        private void OnDisable()
        {
            m_clientMap.OnTapMap -= OnTapMap;
        }
        
        private void OnTapMap(ClientCell clientCell)
        {
            OnUnitSelected?.Invoke(clientCell);
        }

        public void SpawnUnit(int unitUID, int cardUID, Vector2Int coord)
        {
            new ClientUnit(unitUID, cardUID);
            m_clientMap.PlaceUnitOnMap(unitUID, coord);
            var clientCell = m_clientMap[coord];
            // Instantiate unit visual
            var unitVisual = Instantiate(unitVisualPrefab, clientCell.transform.position, 
                Quaternion.identity, m_unitContainer);
            m_unitVisuals.Add(unitUID, unitVisual);
            
            OnUnitSpawned?.Invoke(clientCell);
        }
    }
}
