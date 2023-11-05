using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Minimax
{
    public abstract class TweenableItem : MonoBehaviour
    {
        public Tweener MoveTween  { get; set; }
        public Tweener RotTween   { get; set; }
        public Tweener ScaleTween { get; set; }

        public virtual void KillAllTweens()
        {
            MoveTween?.Kill();
            RotTween?.Kill();
            ScaleTween?.Kill();
        }

        public Tweener StartMoveTween(Vector3 targetPos, float duration = 0.0f)
        {
            if (duration.Equals(0f))
            {
                transform.position = targetPos;
                return null;
            }

            MoveTween?.Kill();
            return MoveTween = transform.DOMove(targetPos, duration);
        }

        public Tweener StartLocalMoveTween(Vector3 targetPos, float duration = 0.0f)
        {
            if (duration.Equals(0f))
            {
                transform.localPosition = targetPos;
                return null;
            }

            MoveTween?.Kill();
            return MoveTween = transform.DOLocalMove(targetPos, duration);
        }

        public Tweener StartRotTween(Vector3 targetRot, float duration = 0.0f)
        {
            if (duration.Equals(0f))
            {
                transform.eulerAngles = targetRot;
                return null;
            }

            RotTween?.Kill();
            return RotTween = transform.DORotate(targetRot, duration);
        }

        public Tweener StartLocalRotQuaternionTween(Quaternion targetRot, float duration = 0.0f)
        {
            if (duration.Equals(0f))
            {
                transform.localRotation = targetRot;
                return null;
            }

            RotTween?.Kill();
            return RotTween = transform.DOLocalRotateQuaternion(targetRot, duration);
        }

        public Tweener StartScaleTween(float targetScale, float duration = 0.0f)
        {
            if (duration.Equals(0f))
            {
                transform.localScale = targetScale * Vector3.one;
                return null;
            }

            ScaleTween?.Kill();
            return ScaleTween = transform.DOScale(targetScale, duration);
        }
    }
}