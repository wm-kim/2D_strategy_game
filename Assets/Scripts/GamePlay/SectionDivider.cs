using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Utilities;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Minimax
{
    public class SectionDivider : MonoBehaviour
    {
        public enum Section
        {
            Default,
            MyHand,
            Map
        }
        
        [SerializeField] private RectTransform m_myHandSection;
        [SerializeField] private RectTransform m_mapSection;
        

        public Section CurrentSection { get; private set; } = Section.Default;

        private void Start()
        {
            // register this service
            GlobalManagers.Instance.ServiceLocator.RegisterService(this, nameof(SectionDivider));
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += DivideSections;
        }
        
        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnTouch -= DivideSections;
        }
        
        private void DivideSections(EnhancedTouch.Touch touch)
        {
            bool isInsideMyHandSection = RectTransformUtility.RectangleContainsScreenPoint(m_myHandSection, touch.screenPosition);
            bool isInsideMapSection = RectTransformUtility.RectangleContainsScreenPoint(m_mapSection, touch.screenPosition);
            
            if (isInsideMapSection) CurrentSection = Section.Map;
            else if (isInsideMyHandSection) CurrentSection = Section.MyHand;
            else CurrentSection = Section.Default;
        }
        
        private void OnDestroy()
        {
            // unregister this service
            GlobalManagers.Instance.ServiceLocator.UnregisterService(nameof(SectionDivider));
        }
    }
}
