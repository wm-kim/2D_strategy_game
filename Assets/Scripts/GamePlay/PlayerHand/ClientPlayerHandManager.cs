using System;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Minimax.GamePlay.PlayerHand
{
    public class ClientPlayerHandManager : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField]
        private HandCardSlot m_cardPrefab;

        [BoxGroup("References")] [SerializeField]
        private Transform m_cardParent;

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

        // Object Pooling HandCardSlot
        private IObjectPool<HandCardSlot> m_cardPool;

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
        public void SelectCard(int index) => m_selectedIndex = index;
        public void DeselectCard() => m_selectedIndex = -1;
        public bool IsSelecting => m_selectedIndex != -1;

        private void Awake()
        {
            // Object Pooling
            m_cardPool = new ObjectPool<HandCardSlot>(() =>
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

#region Test
        // For testing
        public InputAction m_inputAction;

        private void OnEnable()
        {
            // For testing
            m_inputAction.Enable();
        }
        
        private void OnDisable()
        {
            // For testing
            m_inputAction.Disable();
        }

        private void Update()
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                AddCardFromDeck();
            }
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                RemoveCard(3);
            }
        }
#endregion

        // 아직 덱이 구현이 안되어 있어서 임시로 만들어 놓은 함수
        public void AddCardFromDeck()
        {
            if (CardCount >= Define.MaxHandCardCount)
            {
                DebugWrapper.Instance.LogWarning("손패가 가득 찼습니다.");
                return;
            }
            
            var card = m_cardPool.Get();
            card.Init(this, CardCount, $"HandCardSlot_{CardCount}");
            m_slotList.Add(card);

            UpdateSlotTransforms();
            TweenHandSlots();
        }

        public void RemoveCard(int index)
        {
            if (!IsValidIndex(index)) return;

            m_cardPool.Release(m_slotList[index]);
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
                m_slotList[i].PosTween = m_slotList[i].transform.DOLocalMove(m_slotPositionList[i], m_tweenDuration);
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
    }
}
