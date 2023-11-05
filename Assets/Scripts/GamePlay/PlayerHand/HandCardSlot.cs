using System;
using DG.Tweening;
using Minimax.PropertyDrawer;
using Minimax.ScriptableObjects.Settings;
using Minimax.UI.View.ComponentViews.GamePlay;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minimax.GamePlay.PlayerHand
{
    public class HandCardSlot : TweenableItem, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        private int m_index;

        public int Index
        {
            get => m_index;
            set
            {
                m_index         = value;
                gameObject.name = $"HandCardSlot_{m_index}";
                transform.SetSiblingIndex(m_index);
            }
        }

        [field: SerializeField] public HandCardView          HandCardView         { get; private set; }
        [field: SerializeField] public HandCardSlotSettingSO HandCardSlotSettings { get; private set; }

        public MyHandInteractionManager MyHandInteraction { get; set; }

        // States
        [ReadOnly] private HandCardSlotState m_currentState;
        public             DefaultState      DefaultState  { get; private set; }
        public             HoverState        HoverState    { get; private set; }
        public             DraggingState     DraggingState { get; private set; }

        /// <summary>
        /// Initializes all configurations for HandCardSlot.
        /// </summary>
        public void Init(MyHandInteractionManager myHandInteraction, int index, int cardUID)
        {
            MyHandInteraction = myHandInteraction;
            Index             = index;
            HandCardView.CreateClientCardAndSetVisual(cardUID);

            InitializeStates();
            SetDefaultState();
        }

        private void InitializeStates()
        {
            DefaultState  = new DefaultState(this);
            HoverState    = new HoverState(this);
            DraggingState = new DraggingState(this);
        }

        private void SetDefaultState()
        {
            ChangeState(DefaultState);
        }

        public void ChangeState(HandCardSlotState state)
        {
            m_currentState?.Exit();
            m_currentState = state;
            m_currentState.Enter();
        }

        private void Update()
        {
            m_currentState.MoveCardView();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_currentState.OnPointerEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_currentState.OnPointerExit();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_currentState.OnPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_currentState.OnPointerUp();
        }

        public void HoverOff()
        {
            ChangeState(DefaultState);
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }
    }
}