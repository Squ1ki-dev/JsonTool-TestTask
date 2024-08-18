using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonGUIButtons
{
    using Code.Tool.Service.JsonUtilities;
    using Code.Tool.Service.JsonEditorExtension;

    public static class JsonGUIButtons
    {
        public static bool jsonInspectorEdit;

        public static void AddNewProperty<T>(JContainer jContainer, JTokenType type = JTokenType.None)
        {
            string typeName = typeof(T).Name.ToLower();
            object value = default(T);
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                    break;
                case TypeCode.Int32:
                    typeName = "int";
                    break;
                case TypeCode.Single:
                    typeName = "float";
                    break;
                case TypeCode.String:
                    value = "";
                    break;
                default:
                    if (typeof(T) == typeof(JArray))
                    {
                        typeName = "Empty Array";
                        value = new JArray();
                    }
                    else
                    {
                        typeName = "Empty Object";
                        value = new JObject();
                    }
                    break;
            }
            jsonInspectorEdit = true;

            JProperty property;
            JObject jObject = jContainer as JObject;

            string name = JsonUtilities.GetUniqueName(jContainer as JObject, string.Format("new {0}", typeName));
            property = new JProperty(name, value);

            if (type == JTokenType.Array)
            {
                JArray jArray = jContainer as JArray;
                if (jArray == null)
                    jContainer.Add(property);
                else
                {
                    JObject dataObject = new JObject(property);
                    jContainer.Add(dataObject);
                }
            }
            else
                jContainer.Add(property);
        }
    }
}

