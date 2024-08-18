using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonUtilities
{
    public static class JsonUtilities
    {
        private static bool expanded;
        public static bool treeExpand;
        private static ConcurrentDictionary<string, bool> foldDictionary = new ConcurrentDictionary<string, bool>();

        public static bool Fold(string key, bool fold, bool init = false)
        {
            if (!foldDictionary.TryGetValue(key, out bool existingFold))
                foldDictionary[key] = fold;

            else if (!init)
                foldDictionary[key] = fold | treeExpand;

            return foldDictionary[key];
        }

        public static void FoldExpand(bool check)
        {
            if (expanded != check)
            {
                foreach (var key in foldDictionary.Keys.ToList())
                {
                    foldDictionary[key] = check;
                }
                expanded = check;
            }
        }

        public static string GetUniqueName(JObject jObject, string originalName)
        {
            if (jObject == null)
                return originalName;

            string uniqueName = originalName;
            int suffix = 0;

            while (jObject[uniqueName] != null)
            {
                suffix++;
                if (suffix >= 100)
                {
                    Debug.LogError("Stop calling all your fields the same thing!");
                    break;
                }
                uniqueName = $"{originalName} {suffix}";
            }

            return uniqueName;
        }
    }
}