using System;
using System.Collections.Generic;
using Minimax.Definitions;
using Minimax.GamePlay.Card;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Pool;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.GamePlay.PlayerHand
{
    /// <summary>
    /// 카드의 추가, 삭제 등 카드 자체의 상태를 관리하는 것을 목표로 합니다.
    /// </summary>
    public class ClientMyHandManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private HandAnimationManager m_handAnimation;

        [SerializeField] private MyHandInteractionManager m_myHandInteraction;
        [SerializeField] private HandCardSlot             m_handCardSlotPrefab;
        [SerializeField] private Transform                m_cardParent;

        // Object Pooling HandCardSlot
        private IObjectPool<HandCardSlot> m_cardSlotPool;
        private List<HandCardSlot>        m_slotList = new();
        private List<int>                 m_cardUIDs = new();
        public  int                       CardCount => m_slotList.Count;

        public HandCardSlot this[int index] => m_slotList[index];

        public int GetCardUID(int index)
        {
            return m_cardUIDs[index];
        }

        private void Awake()
        {
            ConfigureCardSlotPool();
        }

        private void ConfigureCardSlotPool()
        {
            m_cardSlotPool = new ObjectPool<HandCardSlot>(() =>
                {
                    var handCardSlot = Instantiate(m_handCardSlotPrefab, m_cardParent);
                    handCardSlot.gameObject.SetActive(false);
                    return handCardSlot;
                },
                OnGetCardFromPool,
                OnReleaseCardToPool,
                (handCardSlot) => { Destroy(handCardSlot.gameObject); },
                maxSize: Define.MaxHandCardCount);
        }

        private void OnGetCardFromPool(HandCardSlot handCardSlot)
        {
            handCardSlot.gameObject.SetActive(true);
            m_handAnimation.SetInitialTransform(handCardSlot);
        }

        private void OnReleaseCardToPool(HandCardSlot handCardSlot)
        {
            handCardSlot.gameObject.SetActive(false);
            handCardSlot.HandCardView.StartFadeTween(1);
        }

        public void AddInitialCardsAndTween(int[] cardUIDs)
        {
            foreach (var cardUID in cardUIDs) AddCard(cardUID);
            m_handAnimation.UpdateAndTweenHand(m_slotList);
        }

        public void AddCardAndTween(int cardUID)
        {
            AddCard(cardUID);
            m_handAnimation.UpdateAndTweenHand(m_slotList);
        }

        public void RemoveCardAndTween(int cardUID)
        {
            RemoveCard(cardUID);
            m_handAnimation.UpdateAndTweenHand(m_slotList);
        }

        /// <summary>
        /// Add Card To Rightmost side of the hand
        /// </summary>
        private void AddCard(int cardUID)
        {
            if (!CheckMaxHandCardCount()) return;

            var cardSlot = m_cardSlotPool.Get();
            cardSlot.Init(m_myHandInteraction, CardCount, cardUID);
            m_slotList.Add(cardSlot);
            m_cardUIDs.Add(cardUID);
        }

        private void RemoveCard(int cardUID)
        {
            try
            {
                var index = FindIndexOfCardUID(cardUID);
                m_cardSlotPool.Release(m_slotList[index]);
                m_slotList.RemoveAt(index);
                m_cardUIDs.RemoveAt(index);

                for (var i = 0; i < m_slotList.Count; i++) m_slotList[i].Index = i;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private int FindIndexOfCardUID(int cardUID)
        {
            for (var i = 0; i < CardCount; i++)
                if (m_cardUIDs[i] == cardUID)
                    return i;

            throw new Exception($"CardUID {cardUID} not found in Hand");
        }

        private bool CheckMaxHandCardCount()
        {
            if (CardCount >= Define.MaxHandCardCount)
            {
                Debug.LogWarning("손패가 가득 찼습니다.");
                return false;
            }

            return true;
        }

#if UNITY_EDITOR
        [Command("Client.Hand.PrintAll", MonoTargetType.All)]
        public void PrintAllPlayerHands()
        {
            foreach (var cardUID in m_cardUIDs)
                Debug.Log(
                    $"Card UID: {cardUID}, Card ID {ClientCard.CardsCreatedThisGame[cardUID].Data.CardId}");
        }
#endif
    }
}