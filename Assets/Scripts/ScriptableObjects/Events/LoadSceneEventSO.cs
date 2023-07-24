using Minimax.ScriptableObjects.SceneDatas;
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
        public UnityEvent<SceneType> OnLoadRequested;
        
        /// <summary>
        /// This is a workaround for the fact that UnityEvents cannot take enums as parameters.
        /// </summary>
        public void RaiseEvent(SceneTypeSelector sceneTypeSelector)
        {
            var sceneToLoad = sceneTypeSelector.SceneType;
            RaiseEvent(sceneToLoad);
        }
        
        public void RaiseEvent(SceneType sceneToLoad)
        {
            if (OnLoadRequested != null)
                OnLoadRequested.Invoke(sceneToLoad);
            else
            {
                DebugWrapper.Instance.LogWarning("A Scene loading was requested, but nobody picked it up.");
            }
        }
    }
}
