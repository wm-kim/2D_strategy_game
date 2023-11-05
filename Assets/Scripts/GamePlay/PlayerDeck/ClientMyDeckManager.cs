using System;
using System.Collections.Generic;
using DG.Tweening;
using LeTai.Asset.TranslucentImage;
using Minimax.DeckBuliding;
using Minimax.GamePlay.Card;
using Minimax.UI.View.ComponentViews.GamePlay;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.GamePlay.PlayerDeck
{
    /// <summary>
    /// This class is responsible for visualizing the client player's deck.
    /// </summary>
    public class ClientMyDeckManager : NetworkBehaviour
    {
        [Header("References")] [SerializeField]
        private CardDBManager m_cardDBManager;

        [SerializeField] private Button                 m_myDeckButton;
        [SerializeField] private TranslucentImageSource m_playerPanelBlurSource;
        [SerializeField] private CardListView           m_deckViewFader;
        [SerializeField] private Button                 m_closeMyDeckViewButton;

        [Header("Settings")] [SerializeField] [Range(0, 1)]
        private float m_deckViewFadeDuration = 0.2f;

        private Tween m_deckViewFadeTween;

        private HashSet<int> m_cardsInDeck = new();

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
        public void SetupMyDeckClientRpc(int[] cardUIds, int[] cardIds, ClientRpcParams clientRpcParams = default)
        {
            for (var i = 0; i < cardUIds.Length; i++)
            {
                // create copy of the scriptable object
                var cardData       = Instantiate(m_cardDBManager.GetCardData(cardIds[i]));
                var myPlayerNumber = TurnManager.Instance.MyPlayerNumber;
                new ClientCard(cardUIds[i], myPlayerNumber, cardData);
                m_cardsInDeck.Add(cardUIds[i]);
            }

            m_deckViewFader.Init(cardUIds);
        }

        public void RemoveCard(int cardUID)
        {
            if (m_cardsInDeck.Contains(cardUID))
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

        private void HideMyDeckView()
        {
            m_deckViewFader.StartHide(m_deckViewFadeDuration);
        }

        private void OnDeckViewFadeInComplete()
        {
            m_playerPanelBlurSource.enabled = true;
        }
    }
}