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
        public UnityEvent<string, bool> OnLoadRequested;
        
        public void LoadScene(SceneAsset sceneToLoad) => RaiseEvent(sceneToLoad.name, false);
        
        public void LoadSceneNetwork(SceneAsset sceneToLoad) => RaiseEvent(sceneToLoad.name, true);
        
        public void RaiseEvent(string sceneToLoad, bool useNetwork = false)
        {
            if (OnLoadRequested != null)
                OnLoadRequested.Invoke(sceneToLoad, useNetwork);
            else
            {
                DebugWrapper.LogWarning("A Scene loading was requested, but nobody picked it up.");
            }
        }
    }
}
