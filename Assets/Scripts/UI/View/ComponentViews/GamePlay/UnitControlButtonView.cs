using DG.Tweening;
using Minimax.UI.View.ComponentViews;
using UnityEngine;

namespace Minimax
{
    public class UnitControlButtonView : ButtonView
    {
        [SerializeField]
        private DOTweenAnimation m_tweenAnimation = null;

        public override void SetVisualActive(bool active, bool _ = false)
        {
            if (active)
                m_tweenAnimation.DORestart();
            else
                m_tweenAnimation.DOPlayBackwards();
        }
    }
}