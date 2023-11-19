#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using Minimax.CoreSystems;
using Minimax.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Minimax
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioMangerEditor : Editor
    {
        private string m_audioLibPath = string.Empty;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            AudioManager audioManager = (AudioManager)target;
            m_audioLibPath = audioManager.AudioLibPath;
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
            System.IO.File.WriteAllText(m_audioLibPath, builder.ToString());
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
#endif
