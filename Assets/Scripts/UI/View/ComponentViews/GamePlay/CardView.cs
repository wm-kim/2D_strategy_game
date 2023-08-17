using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Minimax.UI.View.ComponentViews.GamePlay
{
    public class CardView : MonoBehaviour
    {
        public Tween PosTween { get; set; }
        public Tween RotTween { get; set; }
        public Tween ScaleTween { get; set; }
        
        public void KillTweens()
        {
            PosTween?.Kill();
            RotTween?.Kill();
            ScaleTween?.Kill();
        }

        private void OnDestroy()
        {
            KillTweens();
        }
    }
}