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
        [SerializeField] private Button m_button;
        [SerializeField] private TextMeshProUGUI m_cardDataText;

        private CardBaseData m_cardData;
        private DBCardItemMenuView m_dbCardItemMenuView;
        
        public CardBaseData CardData => m_cardData;
        
        public void Init(CardBaseData cardData, DBCardItemMenuView dbCardItemMenuView)
        
        {
            m_cardData = cardData;
            SetView(cardData.CardId);
            SetMenuReference(dbCardItemMenuView);
        }
        
        private void SetView(int cardId)
        {
            m_cardDataText.text = $"Card ID : {cardId.ToString()}";
        }
        
        // store a reference to the menu view so we can show it when the button is clicked
        private void SetMenuReference(DBCardItemMenuView dbCardItemMenuView)
        {
            m_dbCardItemMenuView = dbCardItemMenuView;
        }
        
        public void SetButtonInteractable(bool interactable) => m_button.interactable = interactable;
      
        private void Start()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnButtonClicked()
        {
            m_dbCardItemMenuView.Show(this);
        }
    }
}
