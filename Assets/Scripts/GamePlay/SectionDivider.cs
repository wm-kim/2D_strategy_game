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
            bool isInsideMyHandSection = RectTransformUtility.RectangleContainsScreenPoint(m_myHandSection, touch.screenPosition, m_uiCamera);
            bool isInsideMapSection = RectTransformUtility.RectangleContainsScreenPoint(m_mapSection, touch.screenPosition, m_uiCamera);
            
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
