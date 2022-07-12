using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stylization
{
    [CustomEditor(typeof(StyleHandler))]
    [CanEditMultipleObjects]
    public class StyleHandlerEditor : Editor
    {
        private SerializedProperty style;

        protected SerializedProperty Style
        {
            get 
            {
                if(style == null)
                    style = serializedObject.FindProperty("style");
                return style;
            }
        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            if (Style.objectReferenceValue != null)
            {
                ((Style)style.objectReferenceValue).AddListener((StyleHandler)target);
            }
        }

        protected void DrawStyleProperty()
        {
            style = serializedObject.FindProperty("style");
            object oldobj = style.objectReferenceValue;

            EditorGUILayout.ObjectField(style, new GUIContent("Style"));

            if ((Object)oldobj != style.objectReferenceValue)
            {
                if (oldobj != null)
                {
                    Style oldstyle = (Style)oldobj;
                    oldstyle.RemoveListener((StyleHandler)target);
                }
                if (style.objectReferenceValue != null)
                {
                    Style newstyle = (Style)style.objectReferenceValue;
                    newstyle.AddListener((StyleHandler)target);
                }
            }
        }
    }
}
