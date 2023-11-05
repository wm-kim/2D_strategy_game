using System.Collections.Generic;
using Minimax.UI.View.Pages;
using UnityEngine;

namespace Minimax.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PageStackSO", menuName = "ScriptableObjects/PageStackSO")]
    public class PageStackSO : ScriptableObject
    {
        public Stack<PageType> PageStack = new();
    }
}