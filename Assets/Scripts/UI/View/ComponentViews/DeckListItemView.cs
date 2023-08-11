using System;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.UI.View.ComponentViews;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class DeckListItemView : MonoBehaviour
    {
        [Header("Inner References")]
        [SerializeField] private Button m_button;
        [SerializeField] private TextMeshProUGUI m_cardDataText;
        
        private DeckBuildingViewManager m_deckBuildingViewManager;
        private CardBaseData m_cardData;
        
        public CardBaseData CardData => m_cardData;

        private void Start()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }

        public void Init(CardBaseData cardData, DeckBuildingViewManager deckBuildingViewManager)
        {
            m_deckBuildingViewManager = deckBuildingViewManager;
            m_cardData = cardData;
            SetView(cardData.CardId);
        }
        
        private void SetView(int cardId)
        {
            m_cardDataText.text = $"Card ID : {cardId.ToString()}";
        }

        private void OnButtonClicked()
        {
            m_deckBuildingViewManager.SelectedDeckListItemView = this;
            m_deckBuildingViewManager.DeckListItemMenuView.StartShow();
        }
    }
}
