using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Minimax.Definitions;
using Minimax.UI.View.Pages;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using TMPro;
using Unity.Services.CloudCode;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.ComponentViews.DeckBuilding
{
    [RequireComponent(typeof(Button))]
    public class DBDeckItemView : ButtonView
    {
        [Header("Inner References")] [SerializeField]
        private TextMeshProUGUI m_deckNameText;

        [SerializeField] private CanvasGroup m_menuCanvasGroup;
        [SerializeField] private Button      m_selectButton;
        [SerializeField] private Button      m_deleteButton;

        // Needs for setting selected button interactable into true in DeckPageView
        public Button SelectButton => m_selectButton;

        [Header("Animations")] [SerializeField]
        private AnimationSequencerController m_showAnimationSequencer;

        [SerializeField] private AnimationSequencerController m_hideAnimationSequencer;

        // Caching Deck Page View Reference for setting current deck name
        private DeckPageView m_deckPageView;

        // Caching deck id for deleting deck
        private int m_deckId = -1;

        public void Init(string deckName, int deckId, DeckPageView deckPageView)
        {
            m_deckNameText.text = deckName;
            m_deckId            = deckId;
            m_deckPageView      = deckPageView;
            m_selectButton.onClick.AddListener(OnSelectButtonClicked);
            m_deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

        public override Button Button => GetComponent<Button>();

        public override void SetVisualActive(bool active, bool isImmediate = false)
        {
            if (active)
            {
                m_hideAnimationSequencer.Kill();
                if (m_showAnimationSequencer.IsPlaying) return;
                m_showAnimationSequencer.Play(() => m_menuCanvasGroup.blocksRaycasts = true);
            }
            else
            {
                m_showAnimationSequencer.Kill();
                if (m_hideAnimationSequencer.IsPlaying) return;
                m_hideAnimationSequencer.Play(() => m_menuCanvasGroup.blocksRaycasts = false);
            }
        }

        private async void OnSelectButtonClicked()
        {
            try
            {
                // First reset the deck page view, deselecting all deck item views
                m_deckPageView.ResetDBDeckItem();

                using (new LoadingPopupContext(Define.SelectDeckPopup, "Selecting Deck..."))
                {
                    await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "SelectPlayerDeck",
                        new Dictionary<string, object> { { "value", m_deckId.ToString() } });
                }

                DebugWrapper.Log($"Deck Id : {m_deckId} is selected");

                m_deckPageView.SetCurrentDeckName(m_deckNameText.text);

                // save deck name and Id to player prefs
                PlayerPrefs.SetInt(Define.CurrentDeckIdCache, m_deckId);
                PlayerPrefs.SetString(Define.CurrentDeckNameCache, m_deckNameText.text);

                m_deckPageView.ResetAllSelectedButton();
                // Set select button interactable to false
                m_selectButton.interactable = false;
            }
            catch (CloudCodeException exception)
            {
                DebugWrapper.LogError(exception.Message);
            }
        }

        private async void OnDeleteButtonClicked()
        {
            try
            {
                // First reset the deck page view, deselecting all deck item views
                m_deckPageView.ResetDBDeckItem();

                using (new LoadingPopupContext(Define.DeleteDeckPopup, "Deleting Deck..."))
                {
                    await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "DeletePlayerDeck",
                        new Dictionary<string, object> { { "value", m_deckId.ToString() } });
                }

                DebugWrapper.Log($"Deck Id : {m_deckId} is deleted");

                m_deckPageView.RemoveDeck(m_deckId);

                // if the deleting deck is current selected deck
                if (m_deckId == PlayerPrefs.GetInt(Define.CurrentDeckIdCache))
                {
                    m_deckPageView.SetCurrentDeckName("None");

                    // save deck name and Id to player prefs    
                    PlayerPrefs.SetInt(Define.CurrentDeckIdCache, -1);
                    PlayerPrefs.SetString(Define.CurrentDeckNameCache, "None");
                }
            }
            catch (CloudCodeException exception)
            {
                DebugWrapper.LogError(exception.Message);
            }
        }
    }
}