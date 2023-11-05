using Minimax.ScriptableObjects.CardDatas;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.ComponentViews.DeckBuilding
{
    public class DBCardItemView : MonoBehaviour
    {
        [Header("Inner References")] [Space(10f)] [SerializeField]
        private Button m_button;

        [SerializeField] private TextMeshProUGUI m_cardDataText;

        private DeckBuildingManager m_deckBuildingManager;
        private CardBaseData        m_cardData;

        public CardBaseData CardData => m_cardData;

        public void Init(CardBaseData cardData, DeckBuildingManager deckBuildingManager)
        {
            m_deckBuildingManager = deckBuildingManager;
            m_cardData            = cardData;
            SetView(cardData.CardId);
        }

        private void SetView(int cardId)
        {
            m_cardDataText.text = $"Card ID : {cardId.ToString()}";
        }

        public void SetButtonInteractable(bool interactable)
        {
            m_button.interactable = interactable;
        }

        private void Start()
        {
            m_button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            m_deckBuildingManager.SelectedDBCardItemView = this;
            m_deckBuildingManager.DBCardItemMenuView.StartShow();
        }
    }
}