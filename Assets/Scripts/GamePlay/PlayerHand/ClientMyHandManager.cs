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

namespace Minimax.GamePlay.PlayerHand
{
    /// <summary>
    /// Only responsible for visualizing the player's hand.
    /// </summary>
    public class ClientMyHandManager : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        private HandCardSlot m_cardPrefab;

        [BoxGroup("References")] [SerializeField]
        private Transform m_cardParent;
        public Transform CardParent => m_cardParent;
        
        [BoxGroup("References")] [SerializeField]
        private Canvas m_canvas;
        public Canvas Canvas => m_canvas;
        
        [BoxGroup("References")] [SerializeField]
        private ClientMap m_map;
        
        [BoxGroup("Game Logics")] [SerializeField]
        private CardPlayingLogic m_cardPlayingLogic;
        
        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 반지름")] [Range(0, 10000)]
        private float m_curvRadius = 2000f;

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 각도")] [Range(0, 360)]
        private float m_curvAngle = 30f;

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 중심, m_cardParent를 기준으로 한다.")]
        Vector2 m_curvCenter = new Vector2(0, -200);

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드가 놓일 곡선의 각도")] [Range(0, 360)]
        private float m_baseRotation = 0f;

        [BoxGroup("Card Settings")] [SerializeField, Tooltip("카드 사이의 최대 각도")] [Range(0, 30)]
        private float m_maxBetweenAngle = 3f;

        [BoxGroup("Animation Settings")] [SerializeField, Tooltip("슬롯 정렬 애니메이션의 시간")]
        private float m_tweenDuration = 0.5f;
        
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
        
        private List<Vector3> m_slotPositionList = new List<Vector3>();
        private List<Quaternion> m_slotRotationList = new List<Quaternion>();

        public int CardCount => m_slotList.Count;
        
        private int m_hoveringIndex= -1;
        public void HoverCard(int index) => m_hoveringIndex = index;
        public void UnHoverCard() => m_hoveringIndex = -1;
        public bool IsHovering => m_hoveringIndex != -1;
        
        // SelectedIndex is set when the player drags a card
        private int m_selectedIndex = -1;
        public bool IsSelecting => m_selectedIndex != -1;
        
        /// <summary>
        /// Is the player targeting a cell to play a card
        /// </summary>
        public bool IsTargeting { get; private set; } = false;

        private void Awake()
        {
            // Object Pooling
            m_cardSlotPool = new ObjectPool<HandCardSlot>(() =>
                {
                    var card = Instantiate(m_cardPrefab, m_cardParent);
                    card.gameObject.SetActive(false);
                    return card;
                },
                (card) => { card.gameObject.SetActive(true); },
                (card) => { card.gameObject.SetActive(false); },
                (card) => { Destroy(card.gameObject); },
                maxSize: Define.MaxHandCardCount);

            // Memory Allocation
            for (int i = 0; i < Define.MaxHandCardCount; i++)
            {
                m_slotPositionList.Add(Vector3.zero);
                m_slotRotationList.Add(Quaternion.identity);
            }
        }

        private void OnEnable()
        {
            m_map.OnTouchOverMap += OnTouchOverMap;
            m_map.OnTouchOutsideOfMap += OnUnHoverMap;
            m_map.OnTouchEndOverMap += OnTouchEndOverMap;
        }
        
        private void OnDisable()
        {
            if (m_map == null) return;
            
            m_map.OnTouchOverMap -= OnTouchOverMap;
            m_map.OnTouchOutsideOfMap -= OnUnHoverMap;
            m_map.OnTouchEndOverMap -= OnTouchEndOverMap;
        }
        
        public void AddInitialCardsAndTween(int[] cardUIDs)
        {
            foreach (var cardUID in cardUIDs)
            {
                AddCard(cardUID);
            }
            UpdateSlotTransforms();
            TweenHandSlots();
        }

        public void AddCardAndTween(int cardUID)
        {
            AddCard(cardUID);
            UpdateSlotTransforms();
            TweenHandSlots();
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
                UpdateSlotTransforms();
                TweenHandSlots();
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

        /// <summary>
        /// 카드 슬롯의 위치를 업데이트한다. 카드 손패에 추가되거나 제거되었을 때 마다 호출해야 한다.
        /// </summary>
        private void UpdateSlotTransforms()
        {
            float cardAngle = CardCount <= 1 ? 0 : m_curvAngle / (CardCount - 1);  // Adjust for a single card.
            cardAngle = Mathf.Min(cardAngle, m_maxBetweenAngle);

            float cardAngleOffset = (CardCount - 1) * cardAngle / 2;

            for (int i = 0; i < CardCount; i++)
            {
                float angle = cardAngle * i - cardAngleOffset;
                float radian = angle * Mathf.Deg2Rad;
                float baseRadian = m_baseRotation * Mathf.Deg2Rad;
                float x = m_curvCenter.x + m_curvRadius * Mathf.Sin(baseRadian + radian);
                float y = m_curvCenter.y + m_curvRadius * Mathf.Cos(baseRadian + radian);

                m_slotPositionList[i] = new Vector3(x, y, 0);

                // Rotate the card such that its end points to the curve's center point
                Vector2 directionToCenter = m_curvCenter - new Vector2(x, y);
                // Subtracting 90 degrees to align with the vertical
                float rotationAngle = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg + 90f;

                m_slotRotationList[i] = Quaternion.Euler(0, 0, rotationAngle);
            }
        }

        /// <summary>
        /// 업데이트된 카드 슬롯의 위치에 따라 카드 슬롯을 이동시킨다.
        /// </summary>
        private void TweenHandSlots()
        {
            // sequence auto play when it get out of scope just like IDisposable
            Sequence sequence = DOTween.Sequence();
            
            for (int i = 0; i < CardCount; i++)
            {
                m_slotList[i].KillTweens();
                m_slotList[i].PosTween = m_slotList[i].transform.DOLocalMove(m_slotPositionList[i], m_tweenDuration);
                m_slotList[i].RotTween = m_slotList[i].transform
                .DOLocalRotateQuaternion(m_slotRotationList[i], m_tweenDuration);
                
                sequence.Join(m_slotList[i].PosTween);
                sequence.Join(m_slotList[i].RotTween);
            }

            sequence.Play();
            sequence.OnComplete(Command.ExecutionComplete);
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

        public void ReleaseSelectingCard()
        {
            if (!IsSelecting || IsTargeting) return;
            
            m_slotList[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
            m_selectedIndex = -1;
        }
        
        // I think it is inefficient to add/remove this listener function
        // to Map's OnTouchOverMap event whenever player select/deselect a card.
        // instead, I can just check if the player is selecting a card or not, inside the listeners
        private void OnTouchOverMap(ClientCell clientCell)
        {
            if (!IsSelecting) return;
            IsTargeting = true;
            m_slotList[m_selectedIndex].HandCardView.FadeView(m_cardFadeAlpha, m_cardFadeDuration);
        }

        private void OnUnHoverMap()
        {
            if (!IsSelecting) return;
            IsTargeting = false;
            m_slotList[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
        }
        
        private void OnTouchEndOverMap(ClientCell clientCell)
        {
            if (!IsSelecting) return;
            
            var cardUID = m_cardUIDs[m_selectedIndex];
            DebugWrapper.Log($"Play Card UID {cardUID} on Cell {clientCell.Coord}");
            IsTargeting = false;
            m_cardPlayingLogic.CommandPlayACardFromHandServerRpc(cardUID, clientCell.Coord);
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
