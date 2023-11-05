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
        [SerializeField] private TextMeshProUGUI m_cardIDText;
        [SerializeField] private TextMeshProUGUI m_cardTypeText;

        public void Init(CardBaseData cardData)
        {
            if (cardData == null)
            {
                m_costText.text     = "";
                m_cardIDText.text   = "";
                m_cardTypeText.text = "";
                return;
            }

            m_costText.text     = cardData.Cost.ToString();
            m_cardIDText.text   = cardData.CardId.ToString();
            m_cardTypeText.text = cardData.GetCardType().ToString();
        }
    }
}