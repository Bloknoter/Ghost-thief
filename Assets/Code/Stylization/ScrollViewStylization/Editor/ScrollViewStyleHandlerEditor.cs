using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Stylization
{
    [CustomEditor(typeof(ScrollViewStyleHandler))]
    [CanEditMultipleObjects]
    public class ScrollViewStyleHandlerEditor : StyleHandlerEditor
    {

        private SerializedProperty BG;

        private SerializedProperty VerticalScrollBarHandler;
        private SerializedProperty VerticalScrollBarBG;
        private SerializedProperty HorizontalScrollBarHandler;
        private SerializedProperty HorizontalScrollBarBG;

        private SerializedProperty scrollingType;

        private SerializedProperty style;

        private void OnEnable()
        {
            style = Style;

            BG = serializedObject.FindProperty("BG");

            scrollingType = serializedObject.FindProperty("scrollingType");

            VerticalScrollBarHandler = serializedObject.FindProperty("VerticalScrollBarHandler");
            VerticalScrollBarBG = serializedObject.FindProperty("VerticalScrollBarBG");
            HorizontalScrollBarHandler = serializedObject.FindProperty("HorizontalScrollBarHandler");
            HorizontalScrollBarBG = serializedObject.FindProperty("HorizontalScrollBarBG");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ScrollViewStyleHandler scrollViewStyleHandler = (ScrollViewStyleHandler)target;

            DrawStyleProperty();

            if (BG.objectReferenceValue == null)
            {
                ScrollRect scrollRect = ((ScrollViewStyleHandler)target).GetComponent<ScrollRect>();

                if(scrollRect == null)
                {
                    Debug.Log("You add this component to GameObject that does not have a ScrollRect component. This script won't work");
                    return;
                }

                BG.objectReferenceValue = scrollRect.GetComponent<Image>();

                if (scrollRect.horizontalScrollbar != null)
                {
                    HorizontalScrollBarHandler.objectReferenceValue = scrollRect.horizontalScrollbar.GetComponentInChildren<Image>();
                    HorizontalScrollBarBG.objectReferenceValue = scrollRect.horizontalScrollbar.GetComponent<Image>();
                }
                if (scrollRect.verticalScrollbar != null)
                {
                    VerticalScrollBarHandler.objectReferenceValue = scrollRect.verticalScrollbar.GetComponentInChildren<Image>();
                    VerticalScrollBarBG.objectReferenceValue = scrollRect.verticalScrollbar.GetComponent<Image>();
                }
            }
            if (Style.objectReferenceValue != null)
            {
                EditorGUILayout.ObjectField(BG, new GUIContent("BG"));

                scrollingType.enumValueIndex = EditorGUILayout.Popup("Scrolling type", scrollingType.enumValueIndex, scrollingType.enumNames);

                switch(scrollViewStyleHandler.scrollingType)
                {
                    case ScrollViewStyleHandler.ScrollingTypeEnum.Vertical:
                        EditorGUILayout.LabelField("Vertical slider", EditorStyles.boldLabel);
                        EditorGUILayout.ObjectField(VerticalScrollBarHandler, new GUIContent("Handler"));
                        EditorGUILayout.ObjectField(VerticalScrollBarBG, new GUIContent("BG"));
                        break;
                    case ScrollViewStyleHandler.ScrollingTypeEnum.Horizontal:
                        EditorGUILayout.LabelField("Horizontal slider", EditorStyles.boldLabel);
                        EditorGUILayout.ObjectField(HorizontalScrollBarHandler, new GUIContent("Handler"));
                        EditorGUILayout.ObjectField(HorizontalScrollBarBG, new GUIContent("BG"));
                        break;
                    case ScrollViewStyleHandler.ScrollingTypeEnum.Horizontal_and_Vertical:
                        EditorGUILayout.LabelField("Horizontal slider", EditorStyles.boldLabel);
                        EditorGUILayout.ObjectField(HorizontalScrollBarHandler, new GUIContent("Handler"));
                        EditorGUILayout.ObjectField(HorizontalScrollBarBG, new GUIContent("BG"));

                        EditorGUILayout.LabelField("Vertical slider", EditorStyles.boldLabel);
                        EditorGUILayout.ObjectField(VerticalScrollBarHandler, new GUIContent("Handler"));
                        EditorGUILayout.ObjectField(VerticalScrollBarBG, new GUIContent("BG"));
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
