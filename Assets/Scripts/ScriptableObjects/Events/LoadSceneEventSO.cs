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
        public UnityEvent<UnityEditor.SceneAsset> OnLoadRequested;
        
        public void RaiseEvent(UnityEditor.SceneAsset sceneToLoad)
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
