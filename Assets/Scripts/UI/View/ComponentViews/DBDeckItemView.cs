using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Minimax.UI.View;
using Minimax.UI.View.ComponentViews;
using Minimax.UI.View.Pages;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using UnityEngine;
using TMPro;
using Unity.Services.CloudCode;
using UnityEngine.UI;

namespace Minimax
{
    [RequireComponent(typeof(Button))]
    public class DBDeckItemView : ButtonView
    {
        [Header("Inner References")]
        [SerializeField] private TextMeshProUGUI m_deckNameText;
        [SerializeField] private CanvasGroup m_menuCanvasGroup;
        [SerializeField] private Button m_selectButton;
        [SerializeField] private Button m_deleteButton;
        
        // Needs for setting selected button interactable into true in DeckPageView
        public Button SelectButton => m_selectButton;
        
        [Header("Animations")]
        [SerializeField] private AnimationSequencerController m_showAnimationSequencer;
        [SerializeField] private AnimationSequencerController m_hideAnimationSequencer;
        
        // Caching Deck Page View Reference for setting current deck name
        private DeckPageView m_deckPageView;
        private int m_deckId = -1;
        
        public void Init(string deckName, int deckId, DeckPageView deckPageView)
        {
            m_deckNameText.text = deckName;
            m_deckId = deckId;
            m_deckPageView = deckPageView;
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
                
                using (new LoadingPopupContext("Selecting Deck..."))
                {
                    await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "SelectPlayerDeck",
                        new Dictionary<string, object> { { "key", Define.CurrentDeckSaveKey }, { "value", m_deckId.ToString() } });
                }
                
                DebugWrapper.Log($"Deck Id : {m_deckId} is selected");
                
                m_deckPageView.CurrentDeckId = m_deckId;
                m_deckPageView.SetCurrentDeckName(m_deckNameText.text);
                
                // save deck name to player prefs
                PlayerPrefs.SetString(Define.CurrentDeckSaveKey, m_deckNameText.text);
                
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
                
                using (new LoadingPopupContext("Deleting Deck..."))
                {
                    await CloudCodeService.Instance.CallModuleEndpointAsync("Deck", "DeletePlayerDeck",
                        new Dictionary<string, object> { { "key", Define.DeckSaveKey }, { "value", m_deckId.ToString() } });
                }
                
                DebugWrapper.Log($"Deck Id : {m_deckId} is deleted");
                
                m_deckPageView.RemoveDeck(m_deckId);
                
                if (m_deckPageView.CurrentDeckId == m_deckId)
                {
                    m_deckPageView.CurrentDeckId = -1;
                    m_deckPageView.SetCurrentDeckName("None");
                    PlayerPrefs.SetString(Define.CurrentDeckSaveKey, "None");
                }
            }
            catch (CloudCodeException exception)
            {
                DebugWrapper.LogError(exception.Message);
            }
        }
    }
}
