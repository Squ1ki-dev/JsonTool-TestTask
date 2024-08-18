#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonEditorExtension
{
    using Code.Tool.Service.JsonParser;

    [CustomEditor(typeof(TextAsset), true)]
    public class JsonEditorExtension : Editor
    {
        private Action onSubView;
        public static Event currentEvent;
        public static JObject jsonObject;

        private static ConcurrentDictionary<string, bool> dicFold = new ConcurrentDictionary<string, bool>();

        public static string editText = "";

        public static bool jsonShow;
        public static bool jsonTextEdit;
        public static bool jsonInspectorEdit;

        //Property
        public bool IsCompatible => EditorPath.EndsWith(".json");
        private string EditorPath => AssetDatabase.GetAssetPath(target);
        public static string Path;

        private void OnEnable()
        {
            if (IsCompatible)
            {
                jsonShow = true;
                
                dicFold.Clear();
                Path = EditorPath;
                ReadJson();
            }
        }

        private void OnDisable() => dicFold.Clear();

        public override void OnInspectorGUI()
        {
            if (IsCompatible)
            {
                GUI.enabled = true;
                JsonEditorView.BaseView(ref onSubView, ref jsonObject, ref editText, ref jsonShow, ref jsonTextEdit);
            }
            base.OnInspectorGUI();
        }

        #region File Write & Load
        public static void SaveJson(bool change = false)
        {
            if(jsonObject == null) return;
            if (jsonInspectorEdit || jsonTextEdit)
            {
                if (File.Exists(Path))
                {
                    try
                    {
                        WriteJson(change);
                        Debug.Log("Json Edit Success : " + Path);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Json Edit Fail : " + Path);
                        Debug.LogException(e);
                    }
                }
            }
        }
        public static void ReadJson()
        {
            if (IsValidPath(Path))
            {
                editText = File.ReadAllText(Path);
                jsonObject = JsonConvert.DeserializeObject<JObject>(editText) ?? new JObject();
            }
        }

        public static void ReadJson(string selectPath)
        {
            if (IsValidPath(selectPath))
            {
                editText = File.ReadAllText(selectPath);
                jsonObject = JsonConvert.DeserializeObject<JObject>(editText) ?? new JObject();
            }
        }

        public static void WriteJson(bool change = false)
        {
            if (change)
            {
                editText = jsonObject.ToString();
            }

            File.WriteAllText(Path, editText);
            AssetDatabase.Refresh();
        }

        public static void DeleteFile(string selectPath)
        {
            if (IsValidPath(selectPath))
            {
                Debug.Log($"File Delete: {selectPath}");
                File.Delete(selectPath);
            }
        }

        private static bool IsValidPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        }
        #endregion
    }
}
#endif