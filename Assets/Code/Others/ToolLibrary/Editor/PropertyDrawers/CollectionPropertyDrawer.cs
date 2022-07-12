using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IList))]
public class CollectionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        /*EditorGUI.BeginChangeCheck();
        if (!(fieldInfo.GetValue(serializedProperty.serializedObject.targetObject) is ICollection))
            EditorGUI.PropertyField(EditorGUILayout.GetControlRect(), serializedProperty, new GUIContent(Name));
        else
        {
            EditorGUI.BeginChangeCheck();
            Rect rect = EditorGUILayout.GetControlRect();
            Rect bgrect = new Rect(3, rect.y, rect.width, rect.height);
            EditorGUI.DrawRect(bgrect, Color.grey);
            if (EditorGUI.Foldout(rect, true, Name, true))
            {
                rect = EditorGUILayout.GetControlRect();
                rect = new Rect(rect.x, rect.y, 150, 20);
                if (GUI.Button(rect, "Add element"))
                {

                }
            }
            EditorGUI.EndChangeCheck();
        }*/
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f;
    }
}
