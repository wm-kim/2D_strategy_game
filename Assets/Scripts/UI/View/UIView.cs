using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace WMK
{
    public abstract class UIView : MonoBehaviour
    {
        [System.Serializable]
        public enum VisibleState
        {
            Undefined = -1,
            Appearing,
            Appeared,
            Disappearing,
            Disappeared
        }

        private static readonly IDictionary<Type, UIView> s_loadedViews = new Dictionary<Type, UIView>();
        private VisibleState m_currentState = VisibleState.Undefined;
        private CanvasGroup m_canvasGroup;

        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            gameObject.transform.localPosition = Vector3.zero;
        }
        
        public static T Get<T>() where T : UIView
        {
            Type viewType = typeof(T);
            if (s_loadedViews.TryGetValue(viewType, out UIView view))
            {
                return view as T;
            }
            else
            {
                view = FindObjectOfType<T>(true);
                UnityEngine.Assertions.Assert.IsNotNull(view, $"View {viewType.Name} not found in the scene.");
                s_loadedViews.Add(viewType, view);
                #if UNITY_EDITOR
                Debug.Log($"View {viewType.Name} loaded.");
                #endif
                return view as T;
            }
        }

        public void Show(float duration = 0.0f) 
        {
            if (m_currentState == VisibleState.Appearing || m_currentState == VisibleState.Appeared) return;

            m_currentState = VisibleState.Appearing;
            gameObject.SetActive(true);
            
            // Ensure duration is not negative
            duration = Mathf.Max(duration, 0.0f);
            
            // reducing DoTween call overhead
            if (duration == 0)
            {
                m_canvasGroup.alpha = 1;
                m_canvasGroup.blocksRaycasts = true;
                m_currentState = VisibleState.Appeared; 
            }
            else
            {
                m_canvasGroup.alpha = 0;
                m_canvasGroup.blocksRaycasts = false;
                m_canvasGroup.DOFade(1, duration).OnComplete(() =>
                {
                    m_currentState = VisibleState.Appeared; 
                    m_canvasGroup.blocksRaycasts = true;
                });
            }
        }

        public void Hide(float duration = 0.0f)
        {
            if (m_currentState == VisibleState.Disappearing || m_currentState == VisibleState.Disappeared)
                return;

            m_currentState = VisibleState.Disappearing;
            m_canvasGroup.blocksRaycasts = false;
            
            // Ensure duration is not negative
            duration = Mathf.Max(duration, 0.0f);

            // reducing DoTween call overhead
            if (duration == 0)
            {
                m_canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                m_currentState = VisibleState.Disappeared;
            }
            else
            {
                m_canvasGroup.DOFade(0f, duration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    m_currentState = VisibleState.Disappeared;
                });
            }
        }
    }
}
