#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Code.Tool.Service.JsonGUIStyles
{
    using Code.Tool.Service.JsonEditorConstants;

    public static class JsonGUIStyles
    {
        private static GUIStyle btnStyle = null;
        private static GUIStyle smallBtnStyle = null;
        private static GUIStyle showStyle = null;
        private static GUIStyle labelStyle = null;
        private static GUIStyle filedStyle = null;
        private static GUIStyle editStyle = null;

        public static GUIStyle BtnStyle 
        {
            get{
                if(btnStyle == null)
                {
                    btnStyle = new GUIStyle(EditorStyles.miniButton);
                    btnStyle.alignment = TextAnchor.MiddleCenter;
                    btnStyle.fontSize = 15;
                    btnStyle.normal.textColor = Color.white;
                    btnStyle.border = new RectOffset(5, 5, 5, 5);
                    btnStyle.wordWrap = true;

                    btnStyle.fixedHeight = JsonEditorConstants.HEIGHT;
                }

                return btnStyle;
            }
        }

        public static GUIStyle SmallBtnStyle
        {
            get
            {
                if (smallBtnStyle == null)
                {
                    smallBtnStyle = new GUIStyle(EditorStyles.miniButton);
                    smallBtnStyle.alignment = TextAnchor.MiddleCenter | TextAnchor.MiddleLeft;
                    smallBtnStyle.fontSize = 10;
                    smallBtnStyle.normal.textColor = Color.white;
                }

                return smallBtnStyle;
            }
        }

        public static GUIStyle ShowStyle
        {
            get
            {
                if (showStyle == null)
                {
                    showStyle = new GUIStyle(EditorStyles.helpBox);
                    showStyle.alignment = TextAnchor.UpperLeft;
                    showStyle.fontSize = 13;
                    showStyle.normal.textColor = Color.white;
                }
                return showStyle;
            }
        }

        public static GUIStyle LabelStyle
        {
            get
            {
                if (labelStyle == null)
                {
                    labelStyle = new GUIStyle(EditorStyles.largeLabel);
                    labelStyle.fontSize = 13;
                }
                return labelStyle;
            }
        }

        public static GUIStyle FiledStyle
        {
            get
            {
                if (filedStyle == null)
                {
                    filedStyle = new GUIStyle(EditorStyles.textField);
                    filedStyle.fontSize = 15;
                    filedStyle.fixedHeight = 20;
                }
                return filedStyle;
            }
        }

        public static GUIStyle EditStyle
        {
            get
            {
                if (editStyle == null)
                {
                    editStyle = new GUIStyle(EditorStyles.textArea);
                    editStyle.fontSize = 13;
                }
                return editStyle;
            }
        }
    }
}
#endif