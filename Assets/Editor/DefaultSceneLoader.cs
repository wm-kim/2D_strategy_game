#if UNITY_EDITOR
using Minimax.UnityGamingService.Multiplayer;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace Minimax
{
    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        static DefaultSceneLoader()
        {
            EditorApplication.playModeStateChanged += ModeChanged;

            var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
            EditorSceneManager.playModeStartScene = sceneAsset;
        }

        static void ModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                NetworkManager.Singleton.Shutdown();
                SessionManager<SessionPlayerData>.Instance.OnServerEnded();
            }
        }
    }
}
#endif