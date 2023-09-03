using System.Collections;
using System.Collections.Generic;
using Minimax.ScriptableObjects.CardDatas;
using TMPro;
using UnityEngine;

namespace Minimax
{
    public class CardVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_costText;
        [SerializeField] private TextMeshProUGUI m_cardTypeText;
        
        public void Init(CardBaseData cardData)
        {
            m_costText.text = cardData.Cost.ToString();
            m_cardTypeText.text = cardData.GetCardType().ToString();
        }
    }
}
