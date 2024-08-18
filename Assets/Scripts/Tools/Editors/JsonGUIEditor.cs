#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonGUIEditor
{
    using Code.Tool.Service.JsonEditorExtention;
    using Code.Tool.Service.JsonEditorConstants;
    using Code.Tool.Service.JsonGUIStyles;
    using Code.Tool.Service.JsonGUIButtons;
    using Code.Tool.Service.JsonParser;

    public static class JsonGUIEditor
    {
        private static Event currentEvent;
        private static JProperty propertyToRename;

        private static string propertyRename;

        public static void DrawGUI(JProperty jProperty)
        {
            GUILayout.Space(JsonEditorConstants.SHORT_SPACE);
            float propertyNameWidth = GUI.skin.label.CalcSize(new GUIContent(jProperty.Name.ToString())).x * 1.25f + 10;

            GUILayout.BeginHorizontal();

            CustomButton(jProperty);
            LabelOrField(jProperty, propertyNameWidth);

            switch (jProperty.Value.Type)
            {
                case JTokenType.Boolean:
                    HandleBoolean(jProperty);
                    break;
                case JTokenType.Integer:
                    HandleInteger(jProperty);
                    break;
                case JTokenType.Float:
                    HandleFloat(jProperty);
                    break;
                case JTokenType.String:
                    HandleString(jProperty);
                    break;
                case JTokenType.Property:
                    Debug.LogWarning($"{jProperty.Value} Didn't Parsing");
                    break;
                case JTokenType.Object:
                    JsonParser.DisplayObjectJToken(jProperty.Value);
                    break;
                case JTokenType.Array:
                    JsonParser.DisplayArrayJToken(jProperty.Value);
                    break;
                case JTokenType.None:
                case JTokenType.Null:
                    Debug.LogWarning($"Json Parsing Warning: #{jProperty.Name} - {jProperty.Type} Value Null or None - Have to set Default Value");
                    break;
                default:
                    Debug.LogWarning($"Json Parsing Warning: #{jProperty.Name} - {jProperty.Type} Not Using Data Type!!");
                    break;
            }

            GUILayout.EndHorizontal();
        }

        public static void DrawGUI(JToken jToken)
        {
            GUILayout.Space(JsonEditorConstants.SHORT_SPACE);
            if (jToken.First == null)
                throw new ArgumentNullException("NULL!");

            float propertyNameWidth = GUI.skin.label.CalcSize(new GUIContent(jToken.First.ToString())).x * 1.25f + 10;

            GUILayout.BeginHorizontal();
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    JsonParser.DisplayObjectJToken(jToken);
                    break;
                case JTokenType.Array:
                    JsonParser.DisplayArrayJToken(jToken);
                    break;
                case JTokenType.None:
                case JTokenType.Null:
                    Debug.LogWarning("Json Parsing Warning : #" + jToken.First + " - " + jToken.Type + " Value  Null or None - Have to set Default Value");
                    break;
                default:
                    Debug.LogWarning("Json Parsing Warning : #" + jToken.First + " - " + jToken.Type + "Check Data Type!!");
                    break;
            }
            GUILayout.EndHorizontal();
        }

        private static void LabelOrField(JProperty jProperty, float width)
        {
            if(propertyToRename != jProperty)
                GUILayout.Label(jProperty.Name + " : ", JsonGUIStyles.LabelStyle, GUILayout.Width(width));
            else
                propertyRename = GUILayout.TextField(propertyRename, JsonGUIStyles.FiledStyle);
        }

        private static void LabelOrField(JToken jToken, float width)
        {
            if (propertyToRename != jToken.Parent)
                GUILayout.Label(jToken.First + " : ", JsonGUIStyles.LabelStyle, GUILayout.Width(width));
            else
                propertyRename = GUILayout.TextField(propertyRename, JsonGUIStyles.FiledStyle);
        }

        private static void CustomButton(JProperty jproperty)
        {
            string propertyName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jproperty.Name.ToLower()) + ":";
            float propertyNameWidth = GUI.skin.label.CalcSize(new GUIContent(propertyName)).x;

            if (propertyToRename != jproperty)
            {
                if (GUILayout.Button("►", JsonGUIStyles.SmallBtnStyle, GUILayout.Width(20), GUILayout.Height(20)))
                    ShowContextMenu(jproperty);
            }
            else
            {
                GUI.color = new Color32(55, 255, 88, 255);
                if (GUILayout.Button("✔", JsonGUIStyles.SmallBtnStyle, GUILayout.Width(20), GUILayout.Height(20)))
                    RenameProperty(jproperty);
                GUI.color = Color.white;
            }
        }

        private static void ShowContextMenu(JProperty jproperty)
        {
            GenericMenu menu = new GenericMenu();
            if (jproperty.Value.Type == JTokenType.Object || jproperty.Value.Type == JTokenType.Array)
            {
                JTokenType type = jproperty.Value.Type == JTokenType.Object ? JTokenType.Object : JTokenType.Array;
                JContainer jContainer = jproperty.Value.Value<JContainer>();

                AddMenuItems(menu, jContainer, type);
            }
            menu.AddItem(new GUIContent("Remove"), false, () => RemoveProperty(jproperty));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Rename"), false, () => Rename(jproperty));
            currentEvent = Event.current;
            menu.DropDown(new Rect(currentEvent.mousePosition.x, currentEvent.mousePosition.y, 10, 10));
        }

        private static void AddMenuItems(GenericMenu menu, JContainer jContainer, JTokenType type)
        {
            menu.AddSeparator("Add/");
            menu.AddItem(new GUIContent("Add/String"), false, () => JsonGUIButtons.AddNewProperty<string>(jContainer, type));
            menu.AddItem(new GUIContent("Add/float"), false, () => JsonGUIButtons.AddNewProperty<float>(jContainer, type));
            menu.AddItem(new GUIContent("Add/Integer"), false, () => JsonGUIButtons.AddNewProperty<int>(jContainer, type));
            menu.AddItem(new GUIContent("Add/Boolean"), false, () => JsonGUIButtons.AddNewProperty<bool>(jContainer, type));
            menu.AddItem(new GUIContent("Add/Object"), false, () => JsonGUIButtons.AddNewProperty<JObject>(jContainer, type));
            menu.AddItem(new GUIContent("Add/Array"), false, () => JsonGUIButtons.AddNewProperty<JArray>(jContainer, type));
        }

        private static void RemoveProperty(JProperty jproperty)
        {
            var grandParent = jproperty.Parent.Parent;
            if (grandParent != null && grandParent.Type == JTokenType.Array)
            {
                JArray array = grandParent as JArray;
                array.Remove(jproperty.Parent);
            }
            else
                jproperty.Remove();
        }

        private static void Rename(JProperty jproperty)
        {
            propertyToRename = jproperty;
            propertyRename = propertyToRename.Name;
        }

        private static void RenameProperty(JProperty jproperty)
        {
            JToken newToken = new JProperty(propertyRename, jproperty.Value);
            jproperty.Replace(newToken);
            EditorGUIUtility.ExitGUI();
        }
        
        private static void HandleBoolean(JProperty jProperty)
        {
            var boolValue = jProperty.Value.ToObject<bool>();
            boolValue = GUILayout.Toggle(boolValue, "");
            jProperty.Value = boolValue;
        }

        private static void HandleInteger(JProperty jProperty)
        {
            var intValue = jProperty.Value.ToObject<int>();
            intValue = EditorGUILayout.IntField(intValue, JsonGUIStyles.FiledStyle);
            jProperty.Value = intValue;
        }

        private static void HandleFloat(JProperty jProperty)
        {
            var floatValue = jProperty.Value.ToObject<float>();
            floatValue = EditorGUILayout.FloatField(floatValue, JsonGUIStyles.FiledStyle);
            jProperty.Value = floatValue;
        }

        private static void HandleString(JProperty jProperty)
        {
            var stringValue = jProperty.Value.ToObject<string>();
            stringValue = GUILayout.TextField(stringValue, JsonGUIStyles.FiledStyle);
            jProperty.Value = stringValue;
        }
    }
}
#endif