using System;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Minimax.GamePlay.GridSystem;
using Minimax.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace Minimax.GamePlay.Unit
{
    public class UnitVisual : TweenableItem
    {
        public enum UnitViewType { Front, Back }
        
        [Header("References")]
        [SerializeField] private GameObject m_unitFront; 
        [SerializeField] private GameObject m_unitBack;
        
        [Header("Settings")]
        [SerializeField] private float m_moveDuration = 0.4f;
        
        #region Cached Properties

        private int m_currentState;
        private Animator m_currentAnimator;
        
        private Animator m_frontAnimator;
        private Animator m_backAnimator;
        
        private readonly int FrontIdle = Animator.StringToHash("FrontIdle");
        private readonly int FrontMove = Animator.StringToHash("FrontMove");
        private readonly int BackIdle = Animator.StringToHash("BackIdle");
        private readonly int BackMove = Animator.StringToHash("BackMove");
        
        #endregion

        public void Init(GridRotation playerRotation)
        {
            m_frontAnimator = m_unitFront.GetComponent<Animator>();
            m_backAnimator = m_unitBack.GetComponent<Animator>();
            SetInitialUnitView(playerRotation);
        }

        private void Update()
        {
            var state = GetState();
            if (m_currentState == state) return;
            m_currentAnimator.CrossFade(state, 0, 0);
            m_currentState = state;
        }

        private int GetState()
        {
            if (m_currentState == FrontIdle || m_currentState == FrontMove)
            {
                if (MoveTween != null && MoveTween.IsActive() && MoveTween.IsPlaying()) return FrontMove;
                else return FrontIdle;
            }

            if (m_currentState == BackIdle || m_currentState == BackMove)
            {
                if (MoveTween != null && MoveTween.IsActive() && MoveTween.IsPlaying()) return BackMove;
                else return BackIdle;
            }
            
            return BackIdle;
        }

        private void SetInitialUnitView(GridRotation gridRotation)
        {
            switch (gridRotation)
            {
                case GridRotation.Default:
                    SetUnitView(UnitViewType.Back);
                    gameObject.transform.localScale = new Vector3(1, 1, 1);
                    break;
                case GridRotation.Rotate90:
                    SetUnitView(UnitViewType.Back);
                    gameObject.transform.localScale = new Vector3(-1, 1, 1);
                    break;
                case GridRotation.Rotate180:
                    SetUnitView(UnitViewType.Front);
                    gameObject.transform.localScale = new Vector3(-1, 1, 1);
                    break;
                case GridRotation.Rotate270:
                    SetUnitView(UnitViewType.Front);
                    gameObject.transform.localScale = new Vector3(1, 1, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gridRotation), gridRotation, null);
            }
        }

        private void SetUnitView(UnitViewType viewType)
        {
            m_unitFront.SetActive(viewType == UnitViewType.Front);
            m_unitBack.SetActive(viewType == UnitViewType.Back);
            m_currentAnimator = viewType == UnitViewType.Front ? m_frontAnimator : m_backAnimator;
            m_currentState = viewType == UnitViewType.Front ? FrontIdle : BackIdle;
        }

        public Tweener AnimateMove(Vector2Int dir, GridRotation gridRotation, Vector3 destPos)
        {
            Vector2 rotatedDir = RotateDirection(dir, gridRotation);
            
            switch (rotatedDir)
            {
                case var v when v == Vector2Int.up:
                    SetUnitView(UnitViewType.Back);
                    gameObject.transform.localScale = new Vector3(-1, 1, 1);
                    break;
                case var v when v == Vector2Int.right:
                    SetUnitView(UnitViewType.Back);
                    gameObject.transform.localScale = new Vector3(1, 1, 1);
                    break;
                case var v when v == Vector2Int.down:
                    SetUnitView(UnitViewType.Front);
                    gameObject.transform.localScale = new Vector3(1, 1, 1);
                    break;
                case var v when v == Vector2Int.left:
                    SetUnitView(UnitViewType.Front);
                    gameObject.transform.localScale = new Vector3(-1, 1, 1);
                    break;
            }
            return StartMoveTween(destPos, m_moveDuration);
        }
        
        private Vector2Int RotateDirection(Vector2Int dir, GridRotation gridRotation)
        {
            switch (gridRotation)
            {
                case GridRotation.Default: return dir;
                case GridRotation.Rotate90: return new Vector2Int(dir.y, -dir.x);
                case GridRotation.Rotate180: return new Vector2Int(-dir.x, -dir.y);
                case GridRotation.Rotate270: return new Vector2Int(-dir.y, dir.x);
                default: throw new ArgumentOutOfRangeException(nameof(gridRotation), gridRotation, null);
            }
        }
    }
}