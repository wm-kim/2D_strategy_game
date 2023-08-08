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
        
        private CardBaseData m_cardData;
        private DeckListItemMenuView m_deckListItemMenuView;
        
        public CardBaseData CardData => m_cardData;

        private void Start()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }

        public void Init(CardBaseData cardData, DeckListItemMenuView deckListItemMenuView)
        {
            m_cardData = cardData;
            SetView(cardData.CardId);
            SetMenuReference(deckListItemMenuView);
        }
        
        private void SetMenuReference(DeckListItemMenuView deckListItemMenuView)
        {
            m_deckListItemMenuView = deckListItemMenuView;
        }
        
        private void SetView(int cardId)
        {
            m_cardDataText.text = $"Card ID : {cardId.ToString()}";
        }

        private void OnButtonClicked()
        {
            m_deckListItemMenuView.Show(this);
        }
    }
}
