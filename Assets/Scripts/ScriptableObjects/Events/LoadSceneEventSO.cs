using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Minimax.ScriptableObjects.Events
{
    /// <summary>
    /// This class is used for scene-loading events.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/LoadSceneEvent")]
    public class LoadSceneEventSO : ScriptableObject
    {
        public UnityEvent<string> OnLoadRequested;
        
        public void RaiseEvent(SceneAsset sceneToLoad) => RaiseEvent(sceneToLoad.name);
        
        public void RaiseEvent(string sceneToLoad)
        {
            if (OnLoadRequested != null)
                OnLoadRequested.Invoke(sceneToLoad);
            else
            {
                DebugWrapper.LogWarning("A Scene loading was requested, but nobody picked it up.");
            }
        }
    }
}
