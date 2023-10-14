using System;
using System.Collections.Generic;
using Minimax.UI.View.ComponentViews;
using Minimax.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minimax.UI.Controller
{
    public class ButtonGroupController : MonoBehaviour
    {
        [SerializeField, Tooltip("다중 선택 모드 여부")]    
        private bool m_isMultipleSelection = false;

        [ShowIf("m_isMultipleSelection")]
        [SerializeField, Tooltip("다중 선택 모드에서 최대 선택 가능한 버튼 수")]
        private int m_maxSelectNum = 0;
        
        [HideIf("m_isMultipleSelection")]
        [SerializeField, Tooltip("단일 선택 모드에서 모든 선택지를 선택 해제 가능한지 여부")]
        private bool m_isDeselectable = false;
        
        [SerializeField, Tooltip("Start()에서 첫 번째 버튼을 선택할지 여부")]
        private bool m_selectButtonOnStart = true;
        
        [SerializeField] private List<ButtonView> m_buttonList = new List<ButtonView>();

        [SerializeField, ReadOnly] private List<int> m_activeIndices = new List<int>();

        /// <summary>
        /// 첫 번째 버튼이 클릭된 상태인지 나타냅니다.
        /// </summary>
        private bool m_isInitialButtonClicked = false;
        
        /// <summary>
        /// m_selectButtonOnStart가 true로 설정되었을 때, Start()에서 첫 번째 버튼을 선택했는지 여부
        /// </summary>
        public bool IsInitialButtonClicked => m_selectButtonOnStart && m_isInitialButtonClicked;
        
        /// <summary>
        /// 버튼이 선택되었을 때 발생하는 이벤트. 이미 선택된 버튼을 다시 선택하면 발생하지 않습니다.
        /// </summary>
        public Action<int> OnButtonSelected; 
        
        /// <summary>
        /// 버튼이 클릭되었을 때 발생하는 이벤트, 이미 선택된 버튼을 다시 선택해도 발생합니다.
        /// </summary>
        public Action<int> OnButtonClicked;

        /// <summary>
        /// 버튼 선택이 해제되었을 때 발생하는 이벤트
        /// </summary>
        public Action<int> OnButtonDeselected;

        /// <summary>
        /// 최대 선택 가능한 버튼 수를 초과했을 때 발생하는 이벤트
        /// </summary>
        public Action OnExceedMaxSelectNum;
        
        /// <summary>
        /// Start()에서 첫 번째 버튼을 선택할 때 사용할 인덱스
        /// </summary>
        private int m_initialButtonIndex = -1;

        public void Init(int initialIndexToClick = 0)
        {
            if (m_buttonList.Count == 0) return;
            
            m_buttonList.CheckIndexWithinRange(initialIndexToClick);
            m_initialButtonIndex = initialIndexToClick;
            
            for (int i = 0; i < m_buttonList.Count; i++)
            {
                var temp_i = i;
                m_buttonList[i].Button.onClick.AddListener(() => ToggleActiveState(temp_i));
            }
        }

        private void Start()
        {
            ResetViewAndClickInitialButton();
        }

        private void ResetViewAndClickInitialButton()
        {
            Reset();
            if (!m_selectButtonOnStart) return;
            if (m_buttonList.Count > 0) m_buttonList[m_initialButtonIndex].Button.onClick.Invoke();
            m_isInitialButtonClicked = true;
        }
        
        private void ToggleActiveState(int index)
        {
            OnButtonClicked?.Invoke(index);
            
            if (m_isMultipleSelection)
            {
                // 최대 선택 가능한 버튼 수가 0보다 크고, m_activeIndices 리스트에 index가 없으면
                if (m_maxSelectNum > 0 && m_activeIndices.Count >= m_maxSelectNum && !m_activeIndices.Contains(index))
                {
                    // 최대 선택 가능한 버튼 수를 초과하면 OnExceedMaxSelectNum 이벤트 호출
                    OnExceedMaxSelectNum?.Invoke();
                    return;
                }

                bool isActive = m_activeIndices.Contains(index);
                m_buttonList[index].SetVisualActive(!isActive);

                if (isActive)
                {
                    m_activeIndices.Remove(index);
                    OnButtonDeselected?.Invoke(index);
                }
                else
                {
                    // 선택된 버튼의 인덱스를 m_activeIndices 리스트에 추가
                    m_activeIndices.Add(index); 
                }
            }
            else // 단일 선택 모드
            {
                // 처음 버튼을 선택할 때
                if (m_activeIndices.Count is 0)
                {
                    m_activeIndices.Add(index);
                    m_buttonList[index].SetVisualActive(true, true);
                }
                // 다른 버튼을 선택했을 때
                else if (m_activeIndices[0] != index)
                {
                    m_buttonList[m_activeIndices[0]].SetVisualActive(false);
                    m_buttonList[index].SetVisualActive(true);
                    OnButtonDeselected?.Invoke(m_activeIndices[0]);
                    m_activeIndices[0] = index;
                }
                // 이미 선택된 버튼을 다시 선택했을 때
                else
                {
                    // 선택 해제가 가능하면 선택 해제 후 종료
                    if (m_isDeselectable)
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
        
        /// <summary>
        /// 그룹에 새로운 버튼을 동적으로 추가합니다.
        /// </summary>
        /// <param name="newButton">추가할 새 버튼입니다</param>
        public void AddButtonView(ButtonView newButton)
        {
            // 버튼을 목록에 추가합니다.
            m_buttonList.Add(newButton);

            // 새 버튼에 대한 인덱스를 할당합니다.
            int newIndex = m_buttonList.Count - 1;

            // 새 버튼에 onClick 리스너를 추가합니다.
            newButton.Button.onClick.AddListener(() => ToggleActiveState(newIndex));
        }
        
        /// <summary>
        /// 그룹에서 버튼을 동적으로 제거합니다.
        /// </summary>
        /// <param name="buttonToRemove">제거할 버튼입니다.</param>
        public void RemoveButtonView(ButtonView buttonToRemove)
        {
            if (!m_buttonList.Contains(buttonToRemove))
            {
                DebugWrapper.LogWarning($"ButtonGroupController.RemoveButtonView: " +
                                                 $"{buttonToRemove} is not in the list.");
                return;
            }

            // 버튼에서 onClick 리스너를 제거합니다.
            buttonToRemove.Button.onClick.RemoveAllListeners();

            // 목록에서 버튼을 제거합니다.
            int removedIndex = m_buttonList.IndexOf(buttonToRemove);
            m_buttonList.Remove(buttonToRemove);

            // 제거된 버튼 이후의 버튼들에게 올바른 인덱스를 반영하기 위해 리스너를 다시 할당합니다.
            for (int i = removedIndex; i < m_buttonList.Count; i++)
            {
                // 먼저 이전 리스너를 제거합니다.
                m_buttonList[i].Button.onClick.RemoveAllListeners();

                // 그 후, 새 인덱스로 리스너를 다시 추가합니다.
                int newIndex = i; // 중요: 클로저 동작을 올바르게 하기 위해 루프 내에서 지역 변수를 생성합니다.
                m_buttonList[i].Button.onClick.AddListener(() => ToggleActiveState(newIndex));
            }

            // 필요한 경우 활성 인덱스 목록을 업데이트합니다.
            if (m_activeIndices.Contains(removedIndex))
            {
                m_activeIndices.Remove(removedIndex);
            }
        }
        
        public void Reset()
        {
            m_activeIndices.Clear();
            for (int i = 0; i < m_buttonList.Count; i++) m_buttonList[i].SetVisualActive(false, true);
        }

        public void Clear()
        {
            m_activeIndices.Clear();
            m_buttonList.Clear();
        }
    }
}
