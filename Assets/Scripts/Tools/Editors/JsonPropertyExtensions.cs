using System;
using System.IO;
using System.Text;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Code.Tool.Service.JsonPropertyExtensions
{
    public static class JsonPropertyExtensions
    {
        public static string FoldKeyPath(this JProperty jProperty)
        {
            if (jProperty == null)
                throw new ArgumentNullException(nameof(jProperty));

            StringBuilder sb = new StringBuilder();
            JContainer parent = jProperty.Parent;

            while (parent != null)
            {
                sb.Insert(0, parent.Path + ".");
                parent = parent.Parent;
            }

            sb.Append(jProperty.Name);
            return sb.ToString();
        }
    }
}