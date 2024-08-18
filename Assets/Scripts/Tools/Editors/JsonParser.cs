#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonParser
{
    using Code.Tool.Service.JsonGUIEditor;

    public static class JsonParser
    {
        public static void DisplayObjectJToken(JToken token)
        {
            if (token == null)
            {
                Debug.LogError("The JToken parameter passed to DisplayChildJToken is null.");
                return;
            }

            GUILayout.BeginVertical();
            foreach (JToken childToken in token.Children())
            {
                if (childToken is JProperty jProperty)
                    JsonGUIEditor.DrawGUI(jProperty);
            }
            GUILayout.EndVertical();
        }

        public static void DisplayArrayJToken(JToken token)
        {
            if (token == null)
            {
                Debug.LogError("The JToken parameter passed to DisplayArrayJToken is null");
                return;
            }

            GUILayout.BeginVertical();
            foreach (JToken arrayToken in token)
            {
                JsonGUIEditor.DrawGUI(arrayToken);
            }
            GUILayout.EndVertical();
        }
    }
}
#endif