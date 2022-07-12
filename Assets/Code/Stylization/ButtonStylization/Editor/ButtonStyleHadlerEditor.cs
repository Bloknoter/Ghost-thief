using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

using Stylization;

[CustomEditor(typeof(ButtonStyleHandler))]
[CanEditMultipleObjects]
public class ButtonStyleHadlerEditor : StyleHandlerEditor
{

    private SerializedProperty ButtonBG;

    private SerializedProperty ButtonText;

    private void OnEnable()
    {
        ButtonBG = serializedObject.FindProperty("ButtonBG");
        ButtonText = serializedObject.FindProperty("ButtonText");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ButtonStyleHandler buttonStyleHandler = (ButtonStyleHandler)target;

        DrawStyleProperty();

        if (ButtonBG.objectReferenceValue == null)
        {
            Image img = buttonStyleHandler.GetComponent<Image>();
            ButtonBG.objectReferenceValue = img;

            Text text = buttonStyleHandler.GetComponentInChildren<Text>(true);
            ButtonText.objectReferenceValue = text;
        }
        if ((ButtonStyle)Style.objectReferenceValue != null)
        {
            EditorGUILayout.ObjectField(ButtonBG, new GUIContent("BG"));
            if (((ButtonStyle)Style.objectReferenceValue).HasTextInside)
                EditorGUILayout.ObjectField(ButtonText, new GUIContent("Text"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
