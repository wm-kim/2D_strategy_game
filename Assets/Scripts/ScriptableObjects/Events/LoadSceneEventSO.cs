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
        public UnityEvent<string, bool> OnLoadRequested;
        
        public void LoadScene(SceneTypeSelector sceneToLoad) => RaiseEvent(sceneToLoad.SceneType, false);
        
        public void LoadSceneNetwork(SceneTypeSelector sceneToLoad) => RaiseEvent(sceneToLoad.SceneType, true);
        
        public void RaiseEvent(SceneType sceneToLoad, bool useNetwork = false)
        {
            if (OnLoadRequested != null)
                OnLoadRequested.Invoke(sceneToLoad.ToString(), useNetwork);
            else
            {
                DebugWrapper.LogWarning("A Scene loading was requested, but nobody picked it up.");
            }
        }
    }
}
