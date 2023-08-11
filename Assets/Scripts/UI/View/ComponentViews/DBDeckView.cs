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

        private async void Start()
        {
            List<DeckDTO> decks = await CloudService.Load<List<DeckDTO>>(CloudSaveManager.DeckSaveKey);
            if (decks == null)
            {
                DebugWrapper.LogWarning("No deck data found.");
            }
            else
            {
                foreach (var deck in decks)
                {
                    var deckItemView = Instantiate(m_dbDeckItemViewPrefab, m_contentTransform);
                    deckItemView.Init(deck.Name);
                }
            }
        }
        
    }
}
