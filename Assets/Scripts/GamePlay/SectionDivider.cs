using System;
using Minimax.CoreSystems;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Minimax.GamePlay
{
    public class SectionDivider : MonoBehaviour
    {
        public enum Section
        {
            Default,
            MyHand,
            Map
        }
        
        [Header("Camera")]
        [SerializeField] private Camera m_uiCamera;
        
        [Header("Sections")]
        [SerializeField] private RectTransform m_myHandSection;
        [SerializeField] private RectTransform m_mapSection;
        
        public Section CurrentSection { get; private set; } = Section.Default;

        private void OnEnable()
        {
            // register this service
            GlobalManagers.Instance.ServiceLocator.RegisterService(this, nameof(SectionDivider));
            GlobalManagers.Instance.Input.OnTouch += DivideSections;
        }

        private void OnDisable()
        {
            if (GlobalManagers.Instance == null) return;
            
            if (GlobalManagers.Instance.Input != null)
            {
                GlobalManagers.Instance.Input.OnTouch -= DivideSections;
            }
            
            if (GlobalManagers.Instance.ServiceLocator != null)
            {
                GlobalManagers.Instance.ServiceLocator.UnregisterService(nameof(SectionDivider));
            }
        }

        private void DivideSections(EnhancedTouch.Touch touch)
        {
            bool isInsideMyHandSection = RectTransformUtility.RectangleContainsScreenPoint(m_myHandSection, touch.screenPosition, m_uiCamera);
            bool isInsideMapSection = RectTransformUtility.RectangleContainsScreenPoint(m_mapSection, touch.screenPosition, m_uiCamera);
            
            if (isInsideMapSection) CurrentSection = Section.Map;
            else if (isInsideMyHandSection) CurrentSection = Section.MyHand;
            else CurrentSection = Section.Default;
        }
    }
}
