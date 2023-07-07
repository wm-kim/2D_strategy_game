using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace WMK
{
    public class ButtonGroupController : MonoBehaviour
    {
        /// <summary>
        /// 다중 선택 모드 여부
        /// </summary>
        public bool IsMultipleSelection = false;

        /// <summary>
        /// 단일 선택 모드에서 모든 선택지를 선택 해제 가능한지 여부
        /// </summary>
        public bool IsDeselectable = false;

        /// <summary>
        /// 최대 선택 가능한 버튼 수
        /// </summary>
        public int MaxSelectNum = 0;
        
        [SerializeField] private List<ButtonView> m_buttonList = new List<ButtonView>();

        [SerializeField, ReadOnly] private List<int> m_activeIndices = new List<int>();

        /// <summary>
        /// 버튼이 선택되었을 때 발생하는 이벤트
        /// </summary>
        public Action<int> OnButtonSelected;

        /// <summary>
        /// 버튼 선택이 해제되었을 때 발생하는 이벤트
        /// </summary>
        public Action<int> OnButtonDeselected;

        /// <summary>
        /// 최대 선택 가능한 버튼 수를 초과했을 때 발생하는 이벤트
        /// </summary>
        public Action OnExceedMaxSelectNum;

        public void Awake()
        {
            for (int i = 0; i < m_buttonList.Count; i++)
            {
                var temp_i = i;
                m_buttonList[i].Button.onClick.AddListener(() => ToggleActiveState(temp_i));
            }
        }

        private void Start()
        {
            Reset();
            m_buttonList[0].Button.onClick.Invoke();
        }

        private void ToggleActiveState(int index)
        {
            if (IsMultipleSelection)
            {
                if (MaxSelectNum > 0 && m_activeIndices.Count >= MaxSelectNum && !m_activeIndices.Contains(index))
                {
                    // 최대 선택 가능한 버튼 수를 초과하면 OnExceedMaxSelectNum 이벤트 호출
                    OnExceedMaxSelectNum?.Invoke();
                    return;
                }

                bool isActive = m_activeIndices.Contains(index);
                m_buttonList[index].SetVisualActive(!isActive);

                if (isActive)
                {
                    // 선택이 해제된 버튼의 인덱스를 m_activeIndices 리스트에서 제거
                    m_activeIndices.Remove(index);
                    // 선택이 해제된 버튼의 인덱스를 인자로 OnButtonDeselected 이벤트 호출
                    OnButtonDeselected?.Invoke(index);
                }
                else
                {
                    // 선택된 버튼의 인덱스를 m_activeIndices 리스트에 추가
                    m_activeIndices.Add(index); 
                }
            }
            else
            {
                // 처음 버튼을 선택할 때
                if (m_activeIndices.Count is 0)
                {
                    m_activeIndices.Add(index);
                    m_buttonList[index].SetVisualActive(true, true);
                }
                else if (m_activeIndices[0] != index)
                {
                    m_buttonList[m_activeIndices[0]].SetVisualActive(false);
                    m_buttonList[index].SetVisualActive(true);
                    OnButtonDeselected?.Invoke(m_activeIndices[0]);
                    m_activeIndices[0] = index;
                }
                else
                {
                    if (IsDeselectable)
                    {
                        m_buttonList[index].SetVisualActive(false);
                        OnButtonDeselected?.Invoke(index);
                        m_activeIndices.Clear();
                    }

                    return;
                }
            }

            // 선택된 버튼의 인덱스를 인자로 OnButtonSelected
            OnButtonSelected?.Invoke(index);
        }

        public void Reset()
        {
            m_activeIndices.Clear();
            for (int i = 0; i < m_buttonList.Count; i++) m_buttonList[i].SetVisualActive(false);
        }

        public void Clear()
        {
            m_activeIndices.Clear();
            m_buttonList.Clear();
        }
    }
}
