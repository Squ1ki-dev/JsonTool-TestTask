#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonEditorExtention
{
    using Code.Tool.Service.JsonGUIStyles;
    using Code.Tool.Service.JsonEditorConstants;
    using Code.Tool.Service.JsonGUIButtons;
    using Code.Tool.Service.JsonGUIEditor;

    public static class JsonEditorView
    {
        private static Action onSubView;
        private static Vector2 scrollPos;
        private static Rect initialRect;

        private static float limitSpace;
        
        public static void BaseView(ref Action onSubView, ref JObject jsonObject, ref string editText, ref bool jsonShow, ref bool jsonTextEdit)
        {
            var lineCount = File.ReadLines(JsonEditorExtention.Path).Count();
            initialRect = new Rect(JsonEditorConstants.START_RECT_X, JsonEditorConstants.START_RECT_Y, EditorGUIUtility.currentViewWidth - JsonEditorConstants.HUGE_SPACE, lineCount * 5 + 500);
            limitSpace = initialRect.size.y >= JsonEditorConstants.LIMIT_SPACE ? JsonEditorConstants.LIMIT_SPACE : initialRect.size.y;

            GUILayout.BeginVertical();
            GUILayout.BeginArea(new Rect(initialRect.x, initialRect.y, initialRect.size.x, initialRect.size.y));

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox), GUILayout.Width(initialRect.width), GUILayout.Height(initialRect.size.y), GUILayout.MaxHeight(JsonEditorConstants.LIMIT_SPACE));
            if (jsonShow)
            {
                ShowView(false, jsonObject);
                onSubView = ShowSubView;
            }
            else if (jsonTextEdit)
            {
                TextEditMainView(ref editText);
                onSubView = () => TextEditSubView();
            }
            else
            {
                ShowView(true, jsonObject);
                onSubView = () => InspecterEditSubView();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();
            GUILayout.EndVertical();

            if (onSubView != null)
                onSubView();
        }

        public static void ShowView(bool showTree, JObject jsonObject)
        {
            if (jsonObject != null)
            {
                if (showTree)
                {
                    IEnumerable tokenAble = jsonObject.Values<JProperty>();
                    IEnumerator tokenRator = tokenAble.GetEnumerator();
                    while (tokenRator.MoveNext())
                    {
                        JProperty jProperty = tokenRator.Current as JProperty;
                        JsonGUIEditor.DrawGUI(jProperty);
                    }
                }
                else
                    GUILayout.Label(jsonObject.ToString(), JsonGUIStyles.ShowStyle);
            }
        }

        public static void ShowSubView()
        {
            GUILayout.Space(limitSpace + JsonEditorConstants.BIG_SPACE);
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
            if (GUILayout.Button("Edit Inspector", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x / 2 - JsonEditorConstants.NORMAL_SPACE), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
                JsonEditorExtention.jsonShow = false;

            
            if (GUILayout.Button("Edit Text", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x / 2 - JsonEditorConstants.NORMAL_SPACE), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
            {
                if (JsonEditorExtention.jsonObject != null)
                    JsonEditorExtention.editText = JsonEditorExtention.jsonObject.ToString();

                JsonEditorExtention.jsonShow = false;
                JsonEditorExtention.jsonTextEdit = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void TextEditMainView(ref string editText) => editText = GUILayout.TextArea(editText, JsonGUIStyles.EditStyle);

        public static void TextEditSubView(bool tool = false)
        {
            if (tool || initialRect == Rect.zero)
            {
                var lineCount = File.ReadLines(JsonEditorExtention.Path).Count();
                initialRect.size = new Vector2(200, lineCount * 5 + 500);
            }
            SubTextEditControlView();
        }

        private static void SubTextEditShowView()
        {
            GUILayout.Space(JsonEditorConstants.NORMAL_SPACE);
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));

            if (GUILayout.Button("Show Json", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x / 2 - JsonEditorConstants.NORMAL_SPACE), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
            {
                JsonEditorExtention.editText = "";
                JsonEditorExtention.jsonShow = true;
                JsonEditorExtention.jsonTextEdit = false;
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void SubTextEditControlView()
        {
            if (GUILayout.Button("Save", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
            {
                JsonEditorExtention.SaveJson();
                JsonEditorExtention.ReadJson();
            }
        }

        public static void InspecterEditSubView(bool tool = false)
        {
            if (tool || initialRect == Rect.zero)
            {
                var lineCount = File.ReadLines(JsonEditorExtention.Path).Count();
                initialRect.size = new Vector2(200, lineCount * 5 + 500);
            }

            SubInspectorEditControlView();
        }

        private static void InspecterEditSubView()
        {
            GUILayout.Space(limitSpace + JsonEditorConstants.BIG_SPACE);
            SubInspectorEditControlView();

            SubInspectorEditShowView();
        }

        private static void SubInspectorEditControlView()
        {
            if (GUILayout.Button("Save", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x - JsonEditorConstants.NORMAL_SPACE), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
                SaveJsonInspector();

            GUILayout.Space(JsonEditorConstants.NORMAL_SPACE);

            if (GUILayout.Button("Add new Property", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x - JsonEditorConstants.NORMAL_SPACE), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
                ShowAddPropertyMenu();
        }

        private static void SaveJsonInspector()
        {
            JsonEditorExtention.jsonInspectorEdit = true;
            JsonEditorExtention.SaveJson(true);
            JsonEditorExtention.ReadJson();
        }
        
        public static void ShowAddPropertyMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddSeparator("");

            JContainer jContainer = JsonEditorExtention.jsonObject.Value<JContainer>();

            AddMenuItem(menu, "String", () => JsonGUIButtons.AddNewProperty<string>(jContainer));
            AddMenuItem(menu, "Float", () => JsonGUIButtons.AddNewProperty<float>(jContainer));
            AddMenuItem(menu, "Int", () => JsonGUIButtons.AddNewProperty<int>(jContainer));
            AddMenuItem(menu, "Bool", () => JsonGUIButtons.AddNewProperty<bool>(jContainer));
            AddMenuItem(menu, "Object", () => JsonGUIButtons.AddNewProperty<JObject>(jContainer, JTokenType.Object));
            AddMenuItem(menu, "Array", () => JsonGUIButtons.AddNewProperty<JArray>(jContainer, JTokenType.Array));

            JsonEditorExtention.currentEvent = Event.current;
            menu.DropDown(new Rect(JsonEditorExtention.currentEvent.mousePosition.x, JsonEditorExtention.currentEvent.mousePosition.y, 10, 10));
        }

        private static void AddMenuItem(GenericMenu menu, string content, GenericMenu.MenuFunction action) => menu.AddItem(new GUIContent(content), false, action);
        
        private static void SubInspectorEditShowView()
        {
            GUILayout.Space(JsonEditorConstants.NORMAL_SPACE);
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth));
            if (GUILayout.Button("Show Json", JsonGUIStyles.BtnStyle, GUILayout.Width(initialRect.size.x - JsonEditorConstants.NORMAL_SPACE), GUILayout.Height(JsonEditorConstants.HUGE_SPACE)))
            {
                JsonEditorExtention.jsonShow = true;
                JsonEditorExtention.ReadJson();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif