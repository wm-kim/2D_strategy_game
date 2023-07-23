using System.Collections;
using System.Collections.Generic;
using Minimax.Managers;
using Minimax.UI.View.Pages;
using UnityEngine;

namespace Minimax
{
    [CreateAssetMenu(fileName = "UIStateSO", menuName = "ScriptableObjects/UIStateSO")]
    public class UIStateSO : ScriptableObject
    {
        public PageNavigationType pageNavigationType;
        // only storing history of latest pageNavigationType
        public Stack<PageType> pageHistory = new Stack<PageType>();
    }
}
