using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.ScriptableObjects.Events;
using Minimax.UI.View.ComponentViews;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Minimax
{
    public class DBCardItemView : MonoBehaviour
    {
        [Header("Inner References")]
        [Space(10f)]
        [SerializeField] private Button m_button;
        [SerializeField] private TextMeshProUGUI m_cardDataText;

        private DeckBuildingViewManager m_deckBuildingViewManager;
        private CardBaseData m_cardData;
        
        public CardBaseData CardData => m_cardData;
        
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
        
        public void SetButtonInteractable(bool interactable) => m_button.interactable = interactable;
      
        private void Start()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnButtonClicked()
        {
            m_deckBuildingViewManager.SelectedDBCardItemView = this;
            m_deckBuildingViewManager.DBCardItemMenuView.StartShow();
        }
    }
}
