using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WMK
{
    /// <summary>
    /// Base class for all scene scriptable objects
    /// </summary>
    public class SceneSO : ScriptableObject
    {
        public SceneType SceneType;
        public AssetReference SceneReference;
    }
}
