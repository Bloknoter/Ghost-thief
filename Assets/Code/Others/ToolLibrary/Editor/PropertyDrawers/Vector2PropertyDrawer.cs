using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(Vector2))]
public class Vector2PropertyDrawer : PropertyDrawer
{
    private const float BUTTONS_WIDTH = 50;
    private const float BUTTONS_SPACING = 7;
    private const float SPACING = 5;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (position.width < 2 * BUTTONS_WIDTH + 2 * BUTTONS_SPACING + 235)
        {
            property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
        }
        else
        {
            float previouswidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth /= 2.0f;
            property.vector2Value = EditorGUI.Vector2Field(new Rect(position.x, position.y, position.width - (BUTTONS_WIDTH * 2) - (BUTTONS_SPACING * 2), position.height),
                label, property.vector2Value);
            EditorGUIUtility.labelWidth = previouswidth;

            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;

            if (GUI.Button(new Rect(position.x + position.width - (BUTTONS_WIDTH * 2) - BUTTONS_SPACING, position.y, BUTTONS_WIDTH, position.height), "Copy"))
            {
                object[] vector = new object[] { property.vector2Value.x, property.vector2Value.y };
                ClipboardUtility.WriteData(vector);
            }
            if (GUI.Button(new Rect(position.x + position.width - BUTTONS_WIDTH, position.y, BUTTONS_WIDTH, position.height), "Paste"))
            {
                string[] stringvector = ClipboardUtility.ReadData();
                bool canparse = true;
                if (stringvector.Length < 2)
                {
                    canparse = false;
                }
                if (canparse)
                {
                    try
                    {
                        Vector2 vector = new Vector2(float.Parse(stringvector[0]), float.Parse(stringvector[1]));
                        property.vector2Value = vector;
                    }
                    catch (Exception) { }
                }
            }
            GUI.backgroundColor = previous;
        }
    }
}
