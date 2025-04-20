using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;

namespace UnityCore.AudioManager.Editor
{
    [CustomEditor(typeof(AudioEventsDatabaseSO))]
    public class AudioEventsDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AudioEventsDatabaseSO database = (AudioEventsDatabaseSO)target;

            EditorGUILayout.Space(10);
            
            if (GUILayout.Button("Generate Audio Event Enum", GUILayout.Height(30)))
            {
                GenerateAudioEventEnum(database);
            }
        }

        private void GenerateAudioEventEnum(AudioEventsDatabaseSO database)
        {
            string enumFilePath = Path.Combine(Application.dataPath, "Scripts/UnityCore/AudioManager/AudioEventId.cs");
            
            // Collect all unique IDs
            var allIds = database.GetAllEventIds().Where(id => !string.IsNullOrEmpty(id)).ToList();
            allIds.Sort(); // Sort alphabetically

            // Generate enum content
            StringBuilder enumContent = new StringBuilder();
            enumContent.AppendLine("namespace UnityCore.AudioManager");
            enumContent.AppendLine("{");
            enumContent.AppendLine("    /// <summary>");
            enumContent.AppendLine("    /// Auto-generated enum containing all audio event IDs.");
            enumContent.AppendLine("    /// DO NOT MODIFY THIS FILE MANUALLY - it is automatically generated.");
            enumContent.AppendLine("    /// </summary>");
            enumContent.AppendLine("    public enum AudioEventId");
            enumContent.AppendLine("    {");
            enumContent.AppendLine("        None = 0,");

            // Add each ID as an enum value
            for (int i = 0; i < allIds.Count; i++)
            {
                string enumName = FormatEnumName(allIds[i]);
                enumContent.AppendLine($"        {enumName} = {i + 1},");
            }

            enumContent.AppendLine("    }");
            enumContent.AppendLine("}");

            // Write to file
            File.WriteAllText(enumFilePath, enumContent.ToString());
            AssetDatabase.Refresh();

            Debug.Log("Generated AudioEventId enum with " + allIds.Count + " entries");
        }

        private string FormatEnumName(string id)
        {
            // Keep the original ID format but ensure it's valid for an enum
            string name = id;
            
            // Ensure it starts with a letter
            if (!char.IsLetter(name[0]))
            {
                name = "Audio_" + name;
            }

            // Replace any invalid characters with underscore
            name = System.Text.RegularExpressions.Regex.Replace(name, "[^a-zA-Z0-9_]", "_");

            return name;
        }
    }
}
