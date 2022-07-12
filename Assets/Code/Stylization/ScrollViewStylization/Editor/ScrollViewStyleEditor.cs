using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


namespace Stylization
{
    [CustomEditor(typeof(ScrollViewStyle))]
    [CanEditMultipleObjects]
    public class ScrollViewStyleEditor : Editor
    {
        //public Sprite ScrollBarHandlerSprite;
        //public Color ScrollBarHandlerColor;

        //public Color ScrollBarBGColor;

        //public Color BGColor;

        private SerializedProperty ScrollBarHandlerSprite;
        private SerializedProperty ScrollBarHandlerColor;
        private SerializedProperty ScrollBarBGColor;

        private SerializedProperty BGSprite;
        private SerializedProperty BGColor;


        private void OnEnable()
        {
            // Initializing serialized properties

            ScrollBarHandlerSprite = serializedObject.FindProperty("ScrollBarHandlerSprite");
            ScrollBarHandlerColor = serializedObject.FindProperty("ScrollBarHandlerColor");
            ScrollBarBGColor = serializedObject.FindProperty("ScrollBarBGColor");

            BGSprite = serializedObject.FindProperty("BGSprite");
            BGColor = serializedObject.FindProperty("BGColor");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ScrollViewStyle scrollViewStyle = (ScrollViewStyle)target;

            // Creating custom inspector elements

            EditorGUILayout.LabelField("BG", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(BGSprite, new GUIContent("Sprite"));
            BGColor.colorValue = EditorGUILayout.ColorField("Color", BGColor.colorValue);


            EditorGUILayout.LabelField("Scrollbar: BG", EditorStyles.boldLabel);

            ScrollBarBGColor.colorValue = EditorGUILayout.ColorField("Color", ScrollBarBGColor.colorValue);

            EditorGUILayout.LabelField("Scrollbar: handler", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(ScrollBarHandlerSprite, new GUIContent("Sprite"));
            ScrollBarHandlerColor.colorValue = EditorGUILayout.ColorField("Color", ScrollBarHandlerColor.colorValue);

            EditorGUILayout.HelpBox("ScrollViews with this style: " + (scrollViewStyle.GetListenersAmount()).ToString(), MessageType.None);

            EditorGUILayout.Space(50);

            if (GUILayout.Button("Apply", GUILayout.MinWidth(100)))
            {
                scrollViewStyle.OnStyleHasChangedNotification();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
