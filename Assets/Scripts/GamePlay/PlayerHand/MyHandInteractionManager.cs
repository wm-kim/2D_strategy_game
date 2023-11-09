using System;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Debug = Utilities.Debug;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Minimax.GamePlay.PlayerHand
{
    /// <summary>
    /// 플레이어의 손패와의 상호작용에 대한 로직을 담당하는 클래스
    /// </summary>
    public class MyHandInteractionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private ClientMyHandManager m_clientMyHand;

        [SerializeField]
        private Canvas m_canvas;

        [SerializeField]
        private ClientMap m_clientMap;

        [Header("Game Logics")]
        [SerializeField]
        private CardPlayingLogic m_cardPlayingLogic;

        [Header("Animation Settings")]
        [SerializeField]
        [Range(0, 1)]
        private float m_cardFadeDuration = 0.2f;

        [SerializeField]
        [Range(0, 1)]
        private float m_cardFadeAlpha = 0.20f;

        public Canvas Canvas => m_canvas;

        private int  m_hoveringIndex              = -1;
        private int  m_selectedIndex              = -1;
        private bool m_isTouchingOverMapSelecting = false;

        private void OnEnable()
        {
            m_clientMap.OnTouchOverMap      += OnTouchOverMapSelecting;
            m_clientMap.OnTouchOutsideOfMap += OnTouchOutsideOfMapSelecting;
        }

        private void OnDisable()
        {
            if (m_clientMap == null) return;

            m_clientMap.OnTouchOverMap      -= OnTouchOverMapSelecting;
            m_clientMap.OnTouchOutsideOfMap -= OnTouchOutsideOfMapSelecting;
        }

        public int  SelectedIndex => m_selectedIndex;
        public bool IsHovering    => m_hoveringIndex != -1;
        public bool IsSelecting   => m_selectedIndex != -1;

        public void HoverCard(int index)
        {
            m_hoveringIndex = index;
        }

        public void UnHoverCard()
        {
            m_hoveringIndex = -1;
        }

        public void HoverOffHoveringCard()
        {
            if (!IsHovering) return;
            m_clientMyHand[m_hoveringIndex].HoverOff();
        }

        public void SelectCard(int index)
        {
            m_selectedIndex = index;
        }

        public void DeSelectCard()
        {
            if (!IsSelecting) return;
            m_selectedIndex = -1;
        }

        public void ReleaseSelectingCard()
        {
            if (!IsSelecting) return;

            Debug.Log($"Release Selecting Card {m_selectedIndex}");
            m_clientMyHand[m_selectedIndex].HandCardView.StartFadeTween(1f, m_cardFadeDuration);
            m_selectedIndex = -1;
        }

        // I think it is inefficient to add/remove this listener function
        // to Map's OnTouchOverMap event whenever player select/deselect a card.
        // instead, I can just check if the player is selecting a card or not, inside the listeners
        private void OnTouchOverMapSelecting(ClientCell clientCell)
        {
            if (!IsSelecting) return;
            if (!m_isTouchingOverMapSelecting)
            {
                m_isTouchingOverMapSelecting = true;
                m_clientMyHand[m_selectedIndex].HandCardView.StartFadeTween(m_cardFadeAlpha, m_cardFadeDuration);
            }
        }

        private void OnTouchOutsideOfMapSelecting()
        {
            if (!IsSelecting) return;
            if (m_isTouchingOverMapSelecting)
            {
                m_isTouchingOverMapSelecting = false;
                m_clientMyHand[m_selectedIndex].HandCardView.StartFadeTween(1f, m_cardFadeDuration);
            }
        }

        public void RequestPlayCard(ClientCell clientCell)
        {
            var cardUID = m_clientMyHand.GetCardUID(m_selectedIndex);
            Debug.Log($"Play Card UID {cardUID} on Cell {clientCell.Coord}");
            m_cardPlayingLogic.CommandPlayACardFromHandServerRpc(cardUID, clientCell.Coord);
        }

        public void HandlePlayCardFromHand(int cardUID)
        {
            DeSelectCard();
            m_clientMyHand.RemoveCardAndTween(cardUID);
        }

        public bool TryGetPlayableCellForCard(EnhancedTouch.Touch touch, out ClientCell playableCell)
        {
            var cardUID = m_clientMyHand.GetCardUID(m_selectedIndex);
            return m_cardPlayingLogic.TryGetPlayableCellForCard(cardUID, touch, out playableCell);
        }
    }
}