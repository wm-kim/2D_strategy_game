using System;
using DG.Tweening;
using Minimax.ScriptableObjects.Settings;
using Minimax.UI.View.ComponentViews.GamePlay;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minimax.GamePlay.PlayerHand
{
    public class HandCardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public int Index { get; set; }
        
        public Tween PosTween { get; set; }
        public Tween RotTween { get; set; }
        
        public void KillTweens()
        {
            PosTween?.Kill();
            RotTween?.Kill();
        }
        
        [field: SerializeField] public HandCardView HandCardView { get; private set; }
        [field: SerializeField] public HandCardSlotSettingSO HandCardSlotSettings { get; private set; }
        public ClientPlayerHandManager HandManager { get; private set; }

        // States
        [ReadOnly] private HandCardSlotState m_currentState;
        public DefaultState DefaultState { get; private set; }
        public HoverState HoverState { get; private set; }
        public DraggingState DraggingState { get; private set; }
        
        public void Init(ClientPlayerHandManager handManager, int index, string objectName)
        {
            HandManager = handManager;
            Index = index;
            gameObject.name = objectName;
            
            // Init states
            DefaultState = new DefaultState(this);
            HoverState = new HoverState(this);
            DraggingState = new DraggingState(this);
            
            // Set default state as the initial state
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
        
        private void OnDestroy() { KillTweens(); }
    }
}
