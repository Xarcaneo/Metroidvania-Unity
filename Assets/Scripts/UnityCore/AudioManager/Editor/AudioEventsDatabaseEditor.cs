using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
            
            // Try to load existing enum values to preserve them
            Dictionary<string, int> existingEnumValues = new Dictionary<string, int>();
            int nextAvailableValue = 1;
            
            if (File.Exists(enumFilePath))
            {
                string[] lines = File.ReadAllLines(enumFilePath);
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.Contains("=") && trimmedLine.EndsWith(","))
                    {
                        string[] parts = trimmedLine.Split('=');
                        if (parts.Length == 2)
                        {
                            string enumName = parts[0].Trim();
                            if (enumName == "None") continue; // Skip None which is always 0
                            
                            string valueStr = parts[1].Trim(' ', ',');
                            if (int.TryParse(valueStr, out int value))
                            {
                                existingEnumValues[enumName] = value;
                                nextAvailableValue = Mathf.Max(nextAvailableValue, value + 1);
                            }
                        }
                    }
                }
            }

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

            // Sort alphabetically for display only, but preserve values
            allIds.Sort();
            
            // Add each ID as an enum value
            foreach (string id in allIds)
            {
                string enumName = FormatEnumName(id);
                
                // Use existing value if available, otherwise assign a new one
                int enumValue;
                if (existingEnumValues.TryGetValue(enumName, out enumValue))
                {
                    // Use existing value
                }
                else
                {
                    // Assign new value
                    enumValue = nextAvailableValue++;
                }
                
                enumContent.AppendLine($"        {enumName} = {enumValue},");
            }

            enumContent.AppendLine("    }");
            enumContent.AppendLine("}");

            // Write to file
            File.WriteAllText(enumFilePath, enumContent.ToString());
            AssetDatabase.Refresh();

            Debug.Log("Generated AudioEventId enum with " + allIds.Count + " entries (preserving existing values)");
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
