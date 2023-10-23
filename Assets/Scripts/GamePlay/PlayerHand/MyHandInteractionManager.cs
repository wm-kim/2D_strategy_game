using System;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.GamePlay.PlayerHand
{
    /// <summary>
    /// 플레이어의 손패와의 상호작용에 대한 로직을 담당하는 클래스
    /// </summary>
    public class MyHandInteractionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ClientMyHandManager m_clientMyHand;
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private ClientMap m_map;
        
        [Header("Game Logics")] 
        [SerializeField] private CardPlayingLogic m_cardPlayingLogic;
        
        [Header("Animation Settings")]
        [SerializeField] [Range(0, 1)] private float m_cardFadeDuration = 0.2f;
        [SerializeField] [Range(0, 1)] private float m_cardFadeAlpha = 0.20f;
        
        public Canvas Canvas => m_canvas;
        
        private int m_hoveringIndex = -1;
        private int m_selectedIndex = -1;

        private void OnEnable()
        {
            m_map.OnTouchOverMap += OnTouchOverMap;
            m_map.OnTouchOutsideOfMap += OnTouchOutsideOfMap;
        }
        
        private void OnDisable()
        {
            if (m_map == null) return;
            
            m_map.OnTouchOverMap -= OnTouchOverMap;
            m_map.OnTouchOutsideOfMap -= OnTouchOutsideOfMap;
        }
        
        public bool IsHovering => m_hoveringIndex != -1;
        public bool IsSelecting => m_selectedIndex != -1;
        public void HoverCard(int index) => m_hoveringIndex = index;
        public void UnHoverCard() => m_hoveringIndex = -1;
        public void HoverOffHoveringCard()
        {
            if (!IsHovering) return;
            m_clientMyHand[m_hoveringIndex].HoverOff();
        }
        public void SelectCard(int index) => m_selectedIndex = index;
        
        public void DeSelectCard()
        {
            if (!IsSelecting) return;
            m_selectedIndex = -1;
        }
        
        public void ReleaseSelectingCard()
        {
            if (!IsSelecting) return;
            
            DebugWrapper.Log($"Release Selecting Card {m_selectedIndex}");
            m_clientMyHand[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
            m_selectedIndex = -1;
        }
        
        // I think it is inefficient to add/remove this listener function
        // to Map's OnTouchOverMap event whenever player select/deselect a card.
        // instead, I can just check if the player is selecting a card or not, inside the listeners
        private void OnTouchOverMap(ClientCell clientCell)
        {
            if (!IsSelecting) return;
            m_clientMyHand[m_selectedIndex].HandCardView.FadeView(m_cardFadeAlpha, m_cardFadeDuration);
        }

        private void OnTouchOutsideOfMap()
        {
            if (!IsSelecting) return;
            m_clientMyHand[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
        }
        
        /// <summary>
        /// Wrapper method for ClientMap.TryGetCellFromTouchPos, additionally check if cell is placeable
        /// </summary>
        public bool TryGetCellOfPlayingCard(Vector2 touchPosition, out ClientCell cell)
        {
            if (!m_map.TryGetCellFromTouchPos(touchPosition, out cell)) return false;
            return true;
        }
        
        public void RequestPlaySelectingCard(ClientCell clientCell)
        {
            var cardUID = m_clientMyHand.GetCardUID(m_selectedIndex);
            DebugWrapper.Log($"Play Card UID {cardUID} on Cell {clientCell.Coord}");
            m_cardPlayingLogic.CommandPlayACardFromHandServerRpc(cardUID, clientCell.Coord);
        }
        
        public void HandlePlayCardFromHand(int cardUID)
        {
            DeSelectCard();
            m_clientMyHand.RemoveCardAndTween(cardUID);
        }
    }
}