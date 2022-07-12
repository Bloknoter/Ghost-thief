using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using AttributeHandlers;

public class ListDrawer
{
    public static void DrawList(SerializedProperty property, FieldInfo fieldInfo, DrawSettings drawSettings)
    {
        GUILayout.BeginVertical("Tooltip");
        
        GUIContent expandercontent = new GUIContent(drawSettings.label);
        if (property.isExpanded)
            expandercontent.image = EditorGUIUtility.IconContent("IN foldout act on").image;
        else
            expandercontent.image = EditorGUIUtility.IconContent("IN foldout").image;
        expandercontent.text += " (Array)";
        TextAnchor textAnchor = GUI.skin.button.alignment;
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        if(GUILayout.Button(expandercontent))
        {
            property.isExpanded = !property.isExpanded;
        }
        GUI.skin.button.alignment = textAnchor;
        if (property.isExpanded)
        {
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Add", GUILayout.Width(60)))
            {
                if (property.arraySize > 0)
                    property.InsertArrayElementAtIndex(property.arraySize - 1);
                else
                    property.InsertArrayElementAtIndex(0);
            }
            EditorGUILayout.LabelField("to this array");
            EditorGUILayout.EndHorizontal();
            for(int i = 0; i < property.arraySize;i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    property.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
    }
}
