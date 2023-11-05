using DG.Tweening;
using Minimax.CoreSystems;
using Minimax.GamePlay.GridSystem;
using UnityEngine;
using Utilities;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax.GamePlay.PlayerHand
{
    public class DraggingState : HandCardSlotState
    {
        public DraggingState(HandCardSlot slot)
        {
            m_slot = slot;
        }

        // caching variables
        private Camera  m_camera;
        private float   m_frustumSize        = 0;
        private float   m_zdepth             = 0;
        private Vector3 cachedTargetPosition = Vector3.zero;

        public override void Enter()
        {
            CalculateFrustumSize();
            m_slot.MyHandInteraction.SelectCard(m_slot.Index);
            GlobalManagers.Instance.Input.OnTouch += MoveCardViewToTouchPosition;
        }

        public override void Exit()
        {
            GlobalManagers.Instance.Input.OnTouch -= MoveCardViewToTouchPosition;
        }

        /// <summary>
        /// Calculates the clipping size of the canvas
        /// </summary>
        private void CalculateFrustumSize()
        {
            if (m_camera == null)
            {
                var canvas = m_slot.MyHandInteraction.Canvas;
                m_zdepth = canvas.transform.position.z;
                m_camera = canvas.worldCamera;
                var planeDistance = canvas.planeDistance;
                m_frustumSize = m_camera.CalculateFrustumSize(planeDistance).y * 0.5f;
            }
        }

        private void MoveCardViewToTouchPosition(EnhancedTouch.Touch touch)
        {
            if (IsTouchMovingOrStationary(touch.phase))
                UpdateCardPositionBasedOnTouch(touch);
            else if (touch.phase is TouchPhase.Ended) HandleCardRelease(touch);
        }

        private bool IsTouchMovingOrStationary(TouchPhase touchPhase)
        {
            return touchPhase is TouchPhase.Moved || touchPhase is TouchPhase.Began ||
                   touchPhase is TouchPhase.Stationary;
        }

        private void UpdateCardPositionBasedOnTouch(EnhancedTouch.Touch touch)
        {
            var ray   = m_camera.ScreenPointToRay(touch.screenPosition);
            var plane = new Plane(Vector3.forward, new Vector3(0, 0, m_zdepth));

            if (plane.Raycast(ray, out var enter))
            {
                var hitPoint = ray.GetPoint(enter);
                var sqrDist  = (m_slot.HandCardView.transform.position - hitPoint).sqrMagnitude;

                if (sqrDist <= m_positionThreshold * m_positionThreshold) return;

                cachedTargetPosition = Vector3.Lerp(m_slot.HandCardView.transform.position, hitPoint,
                    Time.deltaTime * m_slot.HandCardSlotSettings.DraggingSpeed);
                m_slot.HandCardView.transform.position = cachedTargetPosition;
            }
        }

        private void HandleCardRelease(EnhancedTouch.Touch touch)
        {
            if (m_slot.MyHandInteraction.TryGetPlayableCellForCard(touch, out var playableCell))
            {
                m_slot.ChangeState(m_slot.DefaultState);
                m_slot.MyHandInteraction.RequestPlayCard(playableCell);
            }
            else
            {
                m_slot.MyHandInteraction.ReleaseSelectingCard();
                m_slot.ChangeState(m_slot.DefaultState);
            }
        }

        public override void MoveCardView()
        {
            // This can be left empty if you're controlling the card view via the OnTouch event
        }

        public override void OnPointerEnter()
        {
        }

        public override void OnPointerExit()
        {
        }

        public override void OnPointerDown()
        {
        }

        public override void OnPointerUp()
        {
        }
    }
}