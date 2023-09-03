using System;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Minimax.GamePlay.PlayerHand
{
    /// <summary>
    /// Only responsible for visualizing the player's hand.
    /// </summary>
    public class ClientPlayerHandManager : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        private HandCardSlot m_cardPrefab;

        [BoxGroup("References")] [SerializeField]
        private Transform m_cardParent;
        public Transform CardParent => m_cardParent;
        
        [BoxGroup("References")] [SerializeField]
        private Camera m_uiCamera;
        public Camera UICamera => m_uiCamera;
        
        [BoxGroup("References")] [SerializeField]
        private Map m_map;
        
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

        private void Start()
        {
            m_map.OnTouchOverMap += OnTouchOverMapMap;
            m_map.OnTouchOutsideOfMap += OnUnHoverMap;
        }

        // 아직 덱이 구현이 안되어 있어서 임시로 만들어 놓은 함수
        public void AddCard(int cardUID)
        {
            if (CardCount >= Define.MaxHandCardCount)
            {
                DebugWrapper.LogWarning("손패가 가득 찼습니다.");
                return;
            }
            
            var cardSlot = m_cardSlotPool.Get();
            cardSlot.Init(this, CardCount, cardUID);
            m_slotList.Add(cardSlot);

            UpdateSlotTransforms();
            TweenHandSlots();
        }

        public void RemoveCard(int index)
        {
            if (!IsValidIndex(index)) return;

            m_cardSlotPool.Release(m_slotList[index]);
            m_slotList.RemoveAt(index);
            
            // Update Indexes
            for (int i = 0; i < m_slotList.Count; i++) m_slotList[i].Index = i;

            UpdateSlotTransforms();
            TweenHandSlots();
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
            for (int i = 0; i < CardCount; i++)
            {
                m_slotList[i].KillTweens();
                m_slotList[i].PosTween = m_slotList[i].transform.DOLocalMove(m_slotPositionList[i], m_tweenDuration)
                    .OnComplete(Command.ExecutionComplete);
                
                m_slotList[i].RotTween = m_slotList[i].transform
                .DOLocalRotateQuaternion(m_slotRotationList[i], m_tweenDuration);
            }
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

        public void DeselectCard()
        {
            if (!IsSelecting) return;
            
            m_slotList[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
            m_selectedIndex = -1;
        }
        
        // I think it is inefficient to add/remove this listener function
        // to Map's OnTouchOverMap event whenever player select/deselect a card.
        // instead, I can just check if the player is selecting a card or not, inside the listeners
        private void OnTouchOverMapMap(Cell cell)
        {
            if (!IsSelecting) return;
            m_slotList[m_selectedIndex].HandCardView.FadeView(m_cardFadeAlpha, m_cardFadeDuration);
        }

        private void OnUnHoverMap()
        {
            if (!IsSelecting) return;
            m_slotList[m_selectedIndex].HandCardView.FadeView(1f, m_cardFadeDuration);
        }
    }
}
