using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WMK
{
    [FilePath("ProjectSettings/GameSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    [CreateAssetMenu(menuName = "ScriptableObjects/GameSettings")]
    public class GameSettings : ScriptableSingleton<GameSettings>
    {
        public string GameTitle;
        public string GameVersion;
    }
}
