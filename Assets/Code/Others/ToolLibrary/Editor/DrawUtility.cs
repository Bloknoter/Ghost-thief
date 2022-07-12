using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

using AttributeHandlers;
using CuriousAttributes;
using CuriousInspectorSettings;

public class DrawUtility
{
    int a;
    private class DefaultSettings
    {
        private static Color backgroundColor;
        private static Color textColor;
        public static void Read()
        {
            backgroundColor = GUI.backgroundColor;
            textColor = EditorStyles.label.normal.textColor;
        }
        public static void Set()
        {
            GUI.backgroundColor = backgroundColor;
            GUI.contentColor = textColor;
            EditorStyles.label.normal.textColor = Color.black;
            EditorStyles.miniButton.normal.textColor = textColor;
        }
    }

    private static void Log(string message)
    {
        if(SettingsData.GetSettingsData().DrawDebug)
        {
            Debug.Log(message);
        }
    }

    #region Drawing functions
    public static void PropertyField(SerializedProperty serializedProperty, FieldInfo fieldInfo)
    {
        DefaultSettings.Read();
        DefaultSettings.Set();
        PropertyField(serializedProperty, fieldInfo, null);
    }

    public static void PropertyField(SerializedProperty serializedProperty, FieldInfo fieldInfo, DrawSettings drawSettings)
    {
        
        if (drawSettings != null)
        {
            Log($"drawing: {fieldInfo.Name}, drawingSettings: \n " + drawSettings.ToString());
            DefaultSettings.Read();
            EditorStyles.label.normal.textColor = drawSettings.textColor;
            GUI.contentColor = drawSettings.textColor;
            GUI.backgroundColor = drawSettings.backgroundColor;
        }
        else
        {
            Log("Creating new draw settings");
            drawSettings = new DrawSettings();
            Log("created draw settings: \n" + drawSettings.ToString());
        }
        if (drawSettings.label.text == "")
            drawSettings.label.text = fieldInfo.Name;
        if (fieldInfo.GetValue(serializedProperty.serializedObject.targetObject) is IList)
        {
            ListDrawer.DrawList(serializedProperty, fieldInfo, drawSettings);
        }
        else
        {
            if (!drawSettings.displayonly)
                EditorGUI.PropertyField(EditorGUILayout.GetControlRect(), serializedProperty, new GUIContent(drawSettings.label.text, drawSettings.label.image));
            else
                EditorGUI.LabelField(EditorGUILayout.GetControlRect(), drawSettings.label, new GUIContent(fieldInfo.GetValue(serializedProperty.serializedObject.targetObject).ToString()));
        }
        DefaultSettings.Set();
    }

    public static bool Button(DrawSettings drawSettings)
    {
        DefaultSettings.Read();
        GUI.backgroundColor = drawSettings.backgroundColor;
        bool result = GUI.Button(EditorGUILayout.GetControlRect(), drawSettings.label.text);

        DefaultSettings.Set();
        return result;
    }

    #endregion

    #region Reflection functions

    public static IHandler[] GetHandlersfromAttributesofTypeandInitIt<T>(MemberInfo memberInfo) where T : MyAttribute
    {
        T[] attributes = (T[])memberInfo.GetCustomAttributes<T>();
        IHandler[] handlers = new IHandler[attributes.Length];
        for(int i = 0; i < attributes.Length;i++)
        {
            handlers[i] = attributes[i].GetHandler();
            handlers[i].Initialize(attributes[i]);
        }
        return handlers;
    }

    #endregion
}
