using System;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.GamePlay.Logic;
using Minimax.Utilities;
using QFSW.QC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Minimax.GamePlay.PlayerHand
{
    /// <summary>
    /// Only responsible for visualizing the player's hand.
    /// </summary>
    public class ClientMyHandDataManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HandAnimationManager m_handAnimationManager;
        [SerializeField] private HandCardSlot m_handCardSlotPrefab;
        [SerializeField] private Transform m_cardParent;
        public Transform CardParent => m_cardParent;
        
        [SerializeField] private Canvas m_canvas;
        public Canvas Canvas => m_canvas;
        
        [SerializeField] private ClientMap m_map;
        
        [Header("Game Logics")] 
        [SerializeField] private CardPlayingLogic m_cardPlayingLogic;
        
        [BoxGroup("Animation Settings")] [SerializeField, Tooltip("선택한 카드 Fade In/Out 애니메이션의 시간")] [Range(0, 1)]
        private float m_cardFadeDuration = 0.2f;
        
        [BoxGroup("Animation Settings")] [SerializeField, Tooltip("선택한 카드 Fade Alpha")] [Range(0, 1)]
        private float m_cardFadeAlpha = 0.20f;
        
        // Object Pooling HandCardSlot
        private IObjectPool<HandCardSlot> m_cardSlotPool;
        private List<HandCardSlot> m_slotList = new List<HandCardSlot>();
        /// <summary>
        /// for keeping track of the index of card uids in my player's hand.
        /// </summary>
        private List<int> m_cardUIDs = new List<int>();

        public int CardCount => m_slotList.Count;
        
        private int m_hoveringIndex= -1;
        public void HoverCard(int index) => m_hoveringIndex = index;
        public void UnHoverCard() => m_hoveringIndex = -1;
        public bool IsHovering => m_hoveringIndex != -1;
        
        // SelectedIndex is set when the player drags a card
        private int m_selectedIndex = -1;
        public bool IsSelecting => m_selectedIndex != -1;
        
        private void Awake()
        {
            // Object Pooling
            m_cardSlotPool = new ObjectPool<HandCardSlot>(() =>
                {
                    var handCardSlot = Instantiate(m_handCardSlotPrefab, m_cardParent);
                    handCardSlot.gameObject.SetActive(false);
                    return handCardSlot;
                },
                (handCardSlot) => { handCardSlot.gameObject.SetActive(true); },
                (handCardSlot) => { handCardSlot.gameObject.SetActive(false); },
                (handCardSlot) => { Destroy(handCardSlot.gameObject); },
                maxSize: Define.MaxHandCardCount);
        }

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

        public void AddInitialCardsAndTween(int[] cardUIDs)
        {
            foreach (var cardUID in cardUIDs)
            {
                AddCard(cardUID);
            }
            m_handAnimationManager.UpdateAndTweenHand(m_slotList);
        }

        public void AddCardAndTween(int cardUID)
        {
            AddCard(cardUID);
            m_handAnimationManager.UpdateAndTweenHand(m_slotList);
        }
        
        /// <summary>
        /// Add Card To Rightmost side of the hand
        /// </summary>
        private void AddCard(int cardUID)
        {
            if (CardCount >= Define.MaxHandCardCount)
            {
                DebugWrapper.LogWarning("손패가 가득 찼습니다.");
                return;
            }
            
            var cardSlot = m_cardSlotPool.Get();
            
            // passing on cardUID to the cardSlot for visualizing the card
            cardSlot.Init(this, CardCount, cardUID);
            m_slotList.Add(cardSlot);
            m_cardUIDs.Add(cardUID);
        }
        
        public void PlayCardAndTween(int cardUID)
        {
            try
            {
                RemoveCard(cardUID);
                m_handAnimationManager.UpdateAndTweenHand(m_slotList);
            }
            catch (Exception e)
            {
                DebugWrapper.LogError(e.Message);
            }
        }

        private void RemoveCard(int cardUID)
        {
            int index = FindIndexOfCardUID(cardUID);
            m_cardSlotPool.Release(m_slotList[index]);
            m_slotList.RemoveAt(index);
            m_cardUIDs.RemoveAt(index);
            
            if (IsSelecting && m_selectedIndex == index) m_selectedIndex = -1;

            // Update Indexes
            for (int i = 0; i < m_slotList.Count; i++) m_slotList[i].Index = i;
        }
        
        private int FindIndexOfCardUID(int cardUID)
        {
            for (int i = 0; i < CardCount; i++)
            {
                if (m_cardUIDs[i] == cardUID) return i;
            }
            
            throw new Exception($"CardUID {cardUID} not found in Hand");
        }

        private bool IsValidIndex(int index)
        {
            bool isValid = index >= 0 && index < CardCount;
            if (!isValid) throw new IndexOutOfRangeException($"Index {index} is out of range. CardCount: {CardCount}");
            return true;
        }

        public void HoverOffHoveringCard()
        {
            if (!IsHovering) return;
            m_slotList[m_hoveringIndex].HoverOff();
        }
        
        public void SelectCard(int index) => m_selectedIndex = index;

        // I think it is inefficient to add/remove this listener function
        // to Map's OnTouchOverMap event whenever player select/deselect a card.
        // instead, I can just check if the player is selecting a card or not, inside the listeners
        private void OnTouchOverMap(ClientCell clientCell)
        {
            if (!IsSelecting) return;
            m_slotList[m_selectedIndex].HandCardView.FadeView(m_cardFadeAlpha, m_cardFadeDuration);
        }

        private void OnTouchOutsideOfMap()
        {
            if (!IsSelecting) return;
            m_slotList[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
        }
        
        public void ReleaseSelectingCard()
        {
            if (!IsSelecting) return;
            
            DebugWrapper.Log($"Release Selecting Card {m_selectedIndex}");
            m_slotList[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
            m_selectedIndex = -1;
        }
        
        public void PlaySelectingCard(ClientCell clientCell)
        {
            var cardUID = m_cardUIDs[m_selectedIndex];
            DebugWrapper.Log($"Play Card UID {cardUID} on Cell {clientCell.Coord}");
            m_cardPlayingLogic.CommandPlayACardFromHandServerRpc(cardUID, clientCell.Coord);
        }
        
        /// <summary>
        /// Wrapper method for ClientMap.TryGetCellFromTouchPos, additionally check if cell is placeable
        /// </summary>
        public bool TryGetCellOfPlayingCard(Vector2 touchPosition, out ClientCell cell)
        {
            if (!m_map.TryGetCellFromTouchPos(touchPosition, out cell)) return false;
            return true;
        }
        
#if UNITY_EDITOR
        [Command("Client.Hand.PrintAll", MonoTargetType.All)]
        public void PrintAllPlayerHands()
        {
            foreach (var cardUID in m_cardUIDs)
            {
                DebugWrapper.Log($"Card UID: {cardUID}, Card ID {ClientCard.CardsCreatedThisGame[cardUID].Data.CardId}");
            }
        }
#endif
    }
}
