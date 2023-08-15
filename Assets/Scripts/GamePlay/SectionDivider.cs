using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using UnityEngine;
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

        private void Awake()
        {
            // register this service
            GlobalManagers.Instance.ServiceLocator.RegisterService(this);
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnTouch += DivideSections;
        }
        
        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnTouch -= DivideSections;
        }
        
        private void DivideSections(Vector2 position, TouchPhase phase)
        {
            bool isInsideMyHandSection = RectTransformUtility.RectangleContainsScreenPoint(m_myHandSection, position);
            bool isInsideMapSection = RectTransformUtility.RectangleContainsScreenPoint(m_mapSection, position);
            
            if (isInsideMapSection) CurrentSection = Section.Map;
            else if (isInsideMyHandSection) CurrentSection = Section.MyHand;
            else CurrentSection = Section.Default;
        }
    }
}
