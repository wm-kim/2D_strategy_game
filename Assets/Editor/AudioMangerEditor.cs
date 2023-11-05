#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects;
using Minimax.Utilities;
using UnityEditor;
using UnityEngine;

namespace Minimax
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioMangerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AudioManager audioManager = (AudioManager)target;
            if (GUILayout.Button("Generate Audio Enums"))
            {
                GenerateEnums(audioManager.AudioFiles, "AudioLib");
            }
        }
        
        private void GenerateEnums(List<AudioFileSO> audioFiles, string enumName)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("public enum " + enumName);
            builder.AppendLine("{");
            
            if (audioFiles != null && audioFiles.Count > 0)
            {
                foreach (var file in audioFiles)
                {
                    builder.AppendLine($"    {file.name},");
                }
            }
            else
            {
                builder.AppendLine("    None,");
            }
            
            builder.AppendLine("}");
            
            // Save to file (adapt the path to fit your project structure)
            string path = "Assets/Scripts/Utilities/AudioEnums.cs";
            System.IO.File.WriteAllText(path, builder.ToString());
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
#endif
