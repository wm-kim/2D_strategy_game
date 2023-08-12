using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.UI.Controller;
using Minimax.UnityGamingService.CloudSave;
using Minimax.Utilities;
using UnityEngine;
using TMPro;

namespace Minimax.UI.View.Pages
{
    public class DeckPageView : PageView
    {
        [Header("References")]
        [Space(10f)]
        [SerializeField] private DBDeckItemView m_dbDeckItemViewPrefab;
        [SerializeField] private Transform m_contentTransform;
        [SerializeField] private ButtonGroupController m_deckButtonGroupController;
        [SerializeField] private TextMeshProUGUI m_currentDeckNameText;
        
        public int CurrentDeckId { get; set; } = -1;
        
        // Caching 
        private Dictionary<int, DeckDTO> m_decks;
        private Dictionary<int, DBDeckItemView> m_deckItemViews = new Dictionary<int, DBDeckItemView>();

        private async void Start()
        {
            // Set current deck name
            if (PlayerPrefs.HasKey(Define.CurrentDeckSaveKey))
                m_currentDeckNameText.text = PlayerPrefs.GetString(Define.CurrentDeckSaveKey);
            else m_currentDeckNameText.text = "None";
            
            var loadDecks = await FetchDecksFromCloud();
            if (loadDecks) InstantiateDeckItemView();
        }
        
        private async UniTask<bool> FetchDecksFromCloud()
        {
            m_decks = await CloudService.Load<Dictionary<int, DeckDTO>>(Define.DeckSaveKey);
            if (m_decks == null)
            {
                DebugWrapper.LogWarning("No deck data found.");
                return false;
            }

            return true;
        }
        
        private void InstantiateDeckItemView()
        {
            foreach (var deck in m_decks)
            {
                var deckItemView = Instantiate(m_dbDeckItemViewPrefab, m_contentTransform);
                deckItemView.Init(deck.Value.Name, deck.Value.Id, this);
                m_deckButtonGroupController.AddButtonView(deckItemView);
                m_deckItemViews.Add(deck.Value.Id, deckItemView);
            }
        }
        
        protected override void SetPageType()
        {
            m_pageType = PageType.DeckPage;
        }

        public void SetCurrentDeckName(string deckName) => m_currentDeckNameText.text = deckName;
        
        public void RemoveDeck(int deckId)
        {
            m_deckButtonGroupController.RemoveButtonView(m_deckItemViews[deckId]);
            Destroy(m_deckItemViews[deckId].gameObject);
            m_deckItemViews.Remove(deckId);
            m_decks.Remove(deckId);
        }
        
        public void ResetDBDeckItem() => m_deckButtonGroupController.Reset();
        
        /// <summary>
        /// Set all selected button interactable into true
        /// </summary>
        public void ResetAllSelectedButton()
        {
            foreach (var deckItemView in m_deckItemViews)
                deckItemView.Value.SelectButton.interactable = true;
        }
    }
}
