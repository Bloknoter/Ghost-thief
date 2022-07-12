using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Stylization
{
    [CustomEditor(typeof(ButtonStyle))]
    [CanEditMultipleObjects]
    public class ButtonStyleEditor : Editor
    {

        private SerializedProperty BGSprite;

        private SerializedProperty BGColor;


        private SerializedProperty HasTextInside;


        private SerializedProperty TextFontSize;

        private SerializedProperty Foreground;

        private SerializedProperty TextFont;


        private void OnEnable()
        {
            // Initializing serialized properties

            BGSprite = serializedObject.FindProperty("BGSprite");
            BGColor = serializedObject.FindProperty("BGColor");

            HasTextInside = serializedObject.FindProperty("HasTextInside");

            TextFontSize = serializedObject.FindProperty("textfontsize");
            Foreground = serializedObject.FindProperty("Foreground");
            TextFont = serializedObject.FindProperty("TextFont");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ButtonStyle buttonStyle = (ButtonStyle)target;

            // Creating custom inspector elements

            EditorGUILayout.LabelField("BG", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField(BGSprite, new GUIContent("Sprite"));

            BGColor.colorValue = EditorGUILayout.ColorField("Color", BGColor.colorValue);


            HasTextInside.boolValue = EditorGUILayout.Toggle("Has text inside", HasTextInside.boolValue);

            if (buttonStyle.HasTextInside)
            {
                EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);

                buttonStyle.TextFontSize = EditorGUILayout.IntField("Font size", TextFontSize.intValue);

                EditorGUILayout.ObjectField(TextFont, new GUIContent("Font"));

                Foreground.colorValue = EditorGUILayout.ColorField("Foreground", Foreground.colorValue);
            }

            EditorGUILayout.HelpBox("Buttons with this style: " + (buttonStyle.GetListenersAmount()).ToString(), MessageType.None);

            EditorGUILayout.Space(50);

            if (GUILayout.Button("Apply", GUILayout.MinWidth(100)))
            {
                buttonStyle.OnStyleHasChangedNotification();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
