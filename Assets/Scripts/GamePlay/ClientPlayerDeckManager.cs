using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LeTai.Asset.TranslucentImage;
using Minimax.GamePlay;
using Minimax.Utilities;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    /// <summary>
    /// This class is responsible for visualizing the client player's deck.
    /// </summary>
    public class ClientPlayerDeckManager : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CardDBManager m_cardDBManager;
        [SerializeField] private Button m_myDeckButton;
        [SerializeField] private TranslucentImageSource m_playerPanelBlurSource;
        [SerializeField] private CardListView m_deckViewFader;
        [SerializeField] private Button m_closeMyDeckViewButton;
        
        [Header("Settings")]
        [SerializeField, Range(0, 1)] private float m_deckViewFadeDuration = 0.2f;
        private Tween m_deckViewFadeTween;
        
        private Dictionary<int, ClientCard> m_cardsInDeck = new Dictionary<int, ClientCard>();

        public event Action<int> OnCardRemovedFromDeck;
        
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
        public void SetupPlayerDeckClientRpc(int[] cardUIds, int[] cardIds, ClientRpcParams clientRpcParams = default)
        {
            for (int i = 0; i < cardUIds.Length; i++)
            {
                var cardData = Instantiate(m_cardDBManager.GetCardData(cardIds[i]));
                var clientCardLogic = new ClientCard(cardUIds[i], cardData);
                m_cardsInDeck.Add(cardUIds[i], clientCardLogic);
            }
            
            m_deckViewFader.Init(cardUIds);
        }
        
        public void RemoveCard(int cardUID)
        {
            if (m_cardsInDeck.ContainsKey(cardUID))
            {
                m_cardsInDeck.Remove(cardUID);
                OnCardRemovedFromDeck?.Invoke(cardUID);
            }
        }
        
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
