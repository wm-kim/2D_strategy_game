using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LeTai.Asset.TranslucentImage;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    /// <summary>
    /// This class is a view component for the client player's deck.
    /// </summary>
    public class ClientPlayerDeckManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_myDeckButton;
        [SerializeField] private TranslucentImageSource m_playerPanelBlurSource;
        [SerializeField] private UIFader m_deckViewFader;
        [SerializeField] private Button m_closeMyDeckViewButton;
        
        [Header("Settings")]
        [SerializeField, Range(0, 1)] private float m_deckViewFadeDuration = 0.2f;
        
        private List<int> m_cardIds = new List<int>();
        private Tween m_deckViewFadeTween;

        private void Awake()
        {
            m_myDeckButton.onClick.AddListener(ShowMyDeckView);
            m_closeMyDeckViewButton.onClick.AddListener(HideMyDeckView);
            
            m_deckViewFader.OnFadeInComplete += OnDeckViewFadeInComplete;
        }
        
        /// <summary>
        /// This method is called by the server to setup the client player's deck.
        /// </summary>
        [ClientRpc]
        public void SetupPlayerDeckClientRpc(int[] cardIds, ClientRpcParams clientRpcParams = default)
        {
            DebugWrapper.Log($"SetupPlayerDeckClientRpc: {JsonConvert.SerializeObject(cardIds)}");
            m_cardIds = new List<int>(cardIds);
        }
        
        // [TODO] Need CardDataVisualizer to visualize the card
        
        private void ShowMyDeckView()
        {
            m_playerPanelBlurSource.enabled = false;
            m_deckViewFader.gameObject.SetActive(true);
            m_deckViewFader.StartShow(m_deckViewFadeDuration);
        }
        
        private void HideMyDeckView() => m_deckViewFader.StartHide(m_deckViewFadeDuration);
        
        private void OnDeckViewFadeInComplete()
        {
            m_playerPanelBlurSource.enabled = true;
        }
    }
}
