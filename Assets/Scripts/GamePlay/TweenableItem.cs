using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Minimax
{
    public abstract class TweenableItem : MonoBehaviour
    {
        public Tweener PosTween { get; set; }
        public Tweener RotTween { get; set; }
        public Tweener ScaleTween { get; set; }
        public Tweener FadeTween { get; set; }
        
        public void KillAllTweens()
        {
            PosTween?.Kill();
            RotTween?.Kill();
            ScaleTween?.Kill();
            FadeTween?.Kill();
        }
        
        public void StartPosTween(Vector3 targetPos, float duration)
        {
            PosTween?.Kill();
            PosTween = transform.DOMove(targetPos, duration);
        }
        
        public void StartLocalPosTween(Vector3 targetPos, float duration)
        {
            PosTween?.Kill();
            PosTween = transform.DOLocalMove(targetPos, duration);
        }
        
        public void StartRotTween(Vector3 targetRot, float duration)
        {
            RotTween?.Kill();
            RotTween = transform.DORotate(targetRot, duration);
        }
        
        public void StartLocalRotQuaternionTween(Quaternion targetRot, float duration)
        {
            RotTween?.Kill();
            RotTween = transform.DOLocalRotateQuaternion(targetRot, duration);
        }
        
        public void StartScaleTween(float targetScale, float duration)
        {
            ScaleTween?.Kill();
            ScaleTween = transform.DOScale(targetScale, duration);
        }
        
        public void StartFadeTween(float targetAlpha, float duration)
        {
            FadeTween?.Kill();
            var canvasGroup = GetComponent<CanvasGroup>();
            FadeTween = canvasGroup.DOFade(targetAlpha, duration);
        }
    }
}
