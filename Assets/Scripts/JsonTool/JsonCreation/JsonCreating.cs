#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Code.Tool.Service.JsonCreating
{
    public static class JsonCreating
    {
        const string JSON_FILE = "New Json File";
        const string JSON_FORMAT = ".json";

        [MenuItem("Assets/Create/JSON File", priority = 78)]
        public static void CreateNewJsonFile()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            
            if (string.IsNullOrEmpty(path))
                path = "Assets/Data/Json/";
            else if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
                path = System.IO.Path.GetDirectoryName(path);

            string fileName = JSON_FILE;
            string fileExtension = JSON_FORMAT;
            int count = 0;
            string newFilePath;

            do
            {
                string countStr = count == 0 ? "" : $"({count})";
                newFilePath = System.IO.Path.Combine(path, $"{fileName}{countStr}{fileExtension}");
                count++;
            }
            while (File.Exists(newFilePath));

            File.WriteAllText(newFilePath, string.Empty);
            AssetDatabase.Refresh();
        }
    }
}
#endif