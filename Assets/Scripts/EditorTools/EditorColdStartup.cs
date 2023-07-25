using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Minimax
{
    public class EditorColdStartup : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset m_thisScene;
        [SerializeField] private PersistentRoot m_persistentRootPrefab;
        
        private void Awake()    
        {
            // first check if the persistentRoot is already in the scene
            if (FindObjectOfType<PersistentRoot>() == null)
            {
                Instantiate(m_persistentRootPrefab);
            }
        }
#endif
    }
}
