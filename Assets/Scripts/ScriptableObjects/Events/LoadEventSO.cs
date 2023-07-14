using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WMK
{
    /// <summary>
    /// This class is used for scene-loading events.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/LoadEvent")]
    public class LoadEventSO : ScriptableObject
    {
        public UnityEvent<SceneSO> OnLoadRequested;
        
        public void RaiseEvent(SceneSO sceneToLoad)
        {
            if (OnLoadRequested != null)
                OnLoadRequested.Invoke(sceneToLoad);
            else
            {
                Debug.LogWarning("A Scene loading was requested, but nobody picked it up.");
            }
        }
    }
}