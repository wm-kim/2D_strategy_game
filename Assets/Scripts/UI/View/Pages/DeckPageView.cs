using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using Minimax.Definitions;
using Minimax.ScriptableObjects;
using Minimax.UI.Controller;
using Minimax.UI.Controller.ComponentControllers;
using Minimax.UI.View.ComponentViews.DeckBuilding;
using Minimax.UnityGamingService.CloudSave;
using UnityEngine;
using TMPro;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.UI.View.Pages
{
    public class DeckPageView : PageView
    {
        [Header("Scriptable Object")]
        [SerializeField]
        private DeckDtoCollectionSO m_deckCollectionSO;

        [Header("References")]
        [Space(10f)]
        [SerializeField]
        private DBDeckItemView m_dbDeckItemViewPrefab;

        [SerializeField]
        private Transform m_contentTransform;

        [SerializeField]
        private ButtonGroupController m_deckButtonGroupController;

        [SerializeField]
        private TextMeshProUGUI m_currentDeckNameText;

        private Dictionary<int, DBDeckItemView> m_deckItemViews = new();

        private void Start()
        {
            m_deckButtonGroupController.Init();

            // Set current deck name
            SetCurrentDeckName();

            var caches = GlobalManagers.Instance.Cache;
            caches.Register(Define.DeckDtoCollectionCache, FetchDecksFromCloud, InitializeDeckViewsOnLoad);
            caches.UpdateLoadCompletedAction(Define.DeckDtoCollectionCache, InitializeDeckViewsOnLoad);
            caches.RequestLoad(Define.DeckDtoCollectionCache);
        }

        private void InitializeDeckViewsOnLoad()
        {
            if (m_deckCollectionSO.Decks == null || m_deckCollectionSO.Decks.Count == 0) return;

            InstantiateDeckItemView();
            DisableSelectedButton();
        }

        // Set current deck name
        private void SetCurrentDeckName()
        {
            m_currentDeckNameText.text = PlayerPrefs.HasKey(Define.CurrentDeckNameCache)
                ? PlayerPrefs.GetString(Define.CurrentDeckNameCache)
                : "None";
        }

        private async UniTask FetchDecksFromCloud(bool isUpdate)
        {
            Debug.Log("Fetching decks from cloud...");
            var decks = await CloudService.Load<Dictionary<int, DeckDTO>>(Define.DeckCloud);
            if (decks == null || decks.Count == 0)
            {
                Debug.LogWarning("No deck data found.");
            }
            else
            {
                Debug.Log("Decks successfully fetched from cloud.");
                m_deckCollectionSO.Decks = decks;

                // setting recent deck id in PlayerPrefs
                if (isUpdate) PlayerPrefs.SetInt(Define.CurrentDeckIdCache, m_deckCollectionSO.GetRecentDeckId());
            }
        }

        private void InstantiateDeckItemView()
        {
            foreach (var deck in m_deckCollectionSO.Decks)
            {
                var deckItemView = Instantiate(m_dbDeckItemViewPrefab, m_contentTransform);
                deckItemView.Init(deck.Value.Name, deck.Value.Id, this);
                m_deckButtonGroupController.AddButtonView(deckItemView);
                m_deckItemViews.Add(deck.Value.Id, deckItemView);
            }
        }

        private void DisableSelectedButton()
        {
            if (!PlayerPrefs.HasKey(Define.CurrentDeckIdCache)) return;

            var selectedDeckId = PlayerPrefs.GetInt(Define.CurrentDeckIdCache);

            foreach (var deckItemView in m_deckItemViews)
                if (deckItemView.Key == selectedDeckId)
                    deckItemView.Value.SelectButton.interactable = false;
        }

        protected override void SetPageType()
        {
            m_pageType = PageType.DeckPage;
        }

        public void SetCurrentDeckName(string deckName)
        {
            m_currentDeckNameText.text = deckName;
        }

        public void RemoveDeck(int deckId)
        {
            m_deckButtonGroupController.RemoveButtonView(m_deckItemViews[deckId]);
            Destroy(m_deckItemViews[deckId].gameObject);
            m_deckItemViews.Remove(deckId);
            m_deckCollectionSO.Decks.Remove(deckId);
        }

        public void ResetDBDeckItem()
        {
            m_deckButtonGroupController.Reset();
        }

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