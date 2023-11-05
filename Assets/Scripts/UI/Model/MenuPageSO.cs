using System;
using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.Events.Primitives;
using UnityEngine;

namespace Minimax.UI.Model
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PageModels/MenuPageSO")]
    public class MenuPageSO : ScriptableObject
    {
        [Header("Text Elements")] public StringEventSO GameTitle;
        public                           StringEventSO GameVersion;
    }
}