using Minimax.CoreSystems;
using UnityEngine;

namespace Minimax.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PageNavigationTypeSO")]
    public class PageNavigationTypeSO : ScriptableObject
    {
        public PageNavigationType Value = PageNavigationType.Undefined;
    }
}