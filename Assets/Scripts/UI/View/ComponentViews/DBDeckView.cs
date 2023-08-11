using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.UI.Controller;
using Minimax.UnityGamingService.CloudSave;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax
{
    public class DBDeckView : MonoBehaviour
    {
        [SerializeField] private DBDeckItemView m_dbDeckItemViewPrefab;
        [SerializeField] private Transform m_contentTransform;
        [SerializeField] private ButtonGroupController m_deckButtonGroupController;
        
        // Caching 
        private List<DeckDTO> m_decks;

        private async void Start()
        {
            m_decks = await CloudService.Load<List<DeckDTO>>(CloudSaveManager.DeckSaveKey);
            if (m_decks == null)
            {
                DebugWrapper.LogWarning("No deck data found.");
            }
            else
            {
                foreach (var deck in m_decks)
                {
                    DBDeckItemView deckItemView = Instantiate(m_dbDeckItemViewPrefab, m_contentTransform);
                    deckItemView.Init(deck.Name);
                    m_deckButtonGroupController.AddButton(deckItemView);
                }
            }
        }
        
    }
}
