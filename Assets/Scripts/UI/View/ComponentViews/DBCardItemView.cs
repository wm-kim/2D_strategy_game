using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Minimax
{
    public class DBCardItemView : MonoBehaviour
    {
        private CardBaseData m_cardData;
        
        [SerializeField] private Button m_button;
        [SerializeField] private TextMeshProUGUI m_cardDataText;
        
        [Header("Listening To")]
        [SerializeField] private DBCardItemMenuEventSO m_dbCardItemMenuEvent;
        public CardBaseData CardData => m_cardData;
        public Button Button => m_button;
        
        public void SetData(CardBaseData cardData)
        {
            m_cardData = cardData;
            SetView(cardData.CardId);
        }
        
        private void SetView(int cardId)
        {
            m_cardDataText.text = $"Card ID : {cardId.ToString()}";
        }
        
        private void Awake()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnButtonClicked()
        {
            m_dbCardItemMenuEvent.RaiseEvent(this, true);
        }
    }
}
