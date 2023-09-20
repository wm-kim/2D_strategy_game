using System;
using DG.Tweening;
using Minimax.GamePlay.CommandSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace Minimax.GamePlay.Unit
{
    public class UnitVisual : MonoBehaviour
    { 
         [field: SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }
         [SerializeField] private float m_moveDuration = 0.4f;

         private Tween m_moveTween;
         
         public void MoveTo(Vector3 destPos, Action onComplete = null)
         {
             m_moveTween?.Kill();
             m_moveTween = transform.DOMove(destPos, m_moveDuration).OnComplete(() =>
             {
                 onComplete?.Invoke();
                 Command.ExecutionComplete();
             });
         }
    }
}