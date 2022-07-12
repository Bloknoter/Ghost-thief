using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

using CuriousAttributes;
using AttributeHandlers;
using GUIContainers;
using CuriousInspectorSettings;

[CustomEditor(typeof(UnityEngine.Object),true, isFallback = true)]
[CanEditMultipleObjects]
public class CuriousInspector : Editor
{
    private Type currType;


    private List<FieldInfo> fieldInfos;
    private List<SerializedProperty> serializedProperties;
    private List<List<IHandler>> handlers;
    private List<DrawSettings> settings;

    private MethodInfo[] methodInfos;

    private Container MainContainer;

    private void OnEnable()
    {
        serializedProperties = new List<SerializedProperty>();
        handlers = new List<List<IHandler>>();
        settings = new List<DrawSettings>();
        currType = target.GetType();
        fieldInfos = new List<FieldInfo>(currType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
        methodInfos = currType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        for (int i = 0; i < fieldInfos.Count; i++)
        {
            SerializedProperty serializedProperty = serializedObject.FindProperty(fieldInfos[i].Name);

            Log($"field: {fieldInfos[i]}, serializedProperty: {serializedProperty != null}");
            if (serializedProperty != null)
            {
                Log("Gathering information about the field...");
                serializedProperties.Add(serializedProperty);

                MyAttribute[] custattributes = (MyAttribute[])fieldInfos[i].GetCustomAttributes<MyAttribute>(true);
                List<IHandler> currhandlers = new List<IHandler>();

                for(int h = 0; h < custattributes.Length;h++)
                {
                    IHandler handler = custattributes[h].GetHandler();
                    handler.Initialize(custattributes[h]);                   
                    currhandlers.Add(handler);
                }

                handlers.Add(currhandlers);

                DrawSettings drawSettings = new DrawSettings();
                settings.Add(drawSettings);
                
            }
            else
            {
                Log("No serialized property found, removing field from drawing list...");
                fieldInfos.RemoveAt(i);
                i--;
            }
        }

        MainContainer = new Container("Main");

        for(int i = 0; i < handlers.Count;i++)
        {
            bool selectedtoanothercontainer = false;
            DrawSettings drawSettings = new DrawSettings();
            FieldContainer fieldContainer = new FieldContainer(serializedProperties[i], fieldInfos[i], drawSettings);
            for (int h = 0; h < handlers[i].Count; h++)
            {
                handlers[i][h].SetDrawSettings(drawSettings);
                if(handlers[i][h] is IContainerHandler)
                {
                    selectedtoanothercontainer = true;
                    IContainerHandler containerHandler = (IContainerHandler)handlers[i][h];

                    if (MainContainer.ContainsContainerLocaly(containerHandler.ContainerName))
                    {
                        Container container = MainContainer.GetContainerLocalyorNull(containerHandler.ContainerName);
                        container.AddComponent(fieldContainer);
                    }
                    else
                    {
                        Container container = ContainerFabric.GetContainer(containerHandler);
                        container.AddComponent(fieldContainer);
                        MainContainer.AddComponent(container);
                    }
                }
                if (handlers[i][h] is ISingleLocalContainerHandler)
                {
                    selectedtoanothercontainer = true;
                    ISingleLocalContainerHandler containerHandler = (ISingleLocalContainerHandler)handlers[i][h];

                    if (MainContainer.ContainsContainerLocaly<SingleLocalContainer>())
                    {
                        SingleLocalContainer container = MainContainer.GetContainerLocalyorNull<SingleLocalContainer>();
                        container.AddComponentandInitIt(fieldContainer, containerHandler);
                    }
                    else
                    {
                        SingleLocalContainer container = (SingleLocalContainer)ContainerFabric.GetContainer(containerHandler);
                        container.AddComponentandInitIt(fieldContainer, containerHandler);
                        MainContainer.AddComponent(container);
                    }
                }
            }
            Log($"field: {fieldInfos[i]}, drawsettings: {drawSettings},\n selectedtoanothercontainer: {selectedtoanothercontainer}");
            if (!selectedtoanothercontainer)
            {
                MainContainer.AddComponent(fieldContainer);
            }
        }

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        MainContainer.Draw();
        // НУЖНО ДВА ЦИКЛА !!! Первый инициализирует обработчики атрибутов, второй нужен для отрисвоки полей и 
        // специальных элементов (например, панели вкладок. Это атрибут Tab и обработчик TabHandler)

        /*for (int i = 0; i < fieldInfos.Length; i++)
        {
            for (int a = 0; a < attributes[i].Count; a++)
            {
                IHandler fieldHandler = attributes[i][a].GetHandler();
                fieldHandler.Initialize(attributes[i][a]);
                fieldHandler.SetDrawSettings(settings[i]);
            }
        }*/

        /*for (int i = 0; i < fieldInfos.Length; i++)
        {

            for (int a = 0; a < attributes[i].Count; a++)
            {
                IHandler fieldHandler = attributes[i][a].GetHandler();
                if(fieldHandler is IGroupHandler)
                {
                    if (!((IGroupHandler)fieldHandler).IsHeaderDrawn())
                    {
                        ((IGroupHandler)fieldHandler).DrawHeader();
                    }
                }
            }

            DrawUtility.PropertyField(serializedProperties[i], fieldInfos[i], settings[i]);
        }*/

        for (int i = 0; i < methodInfos.Length; i++)
        {
            // Debug.Log($"{memberInfos[i].MemberType} {i} : {memberInfos[i]}");
            IHandler[] handlers = DrawUtility.GetHandlersfromAttributesofTypeandInitIt<MyAttribute>(methodInfos[i]);
            if (handlers.Length > 0)
            {
                DrawSettings drawSettings = new DrawSettings();

                for (int a = 0; a < handlers.Length; a++)
                {
                    handlers[a].SetDrawSettings(drawSettings);
                }
                if(DrawUtility.Button(drawSettings))
                {
                    methodInfos[i].Invoke(target, null);
                }
            }

        }

        serializedObject.ApplyModifiedProperties();
    }


    private void Log(string message)
    {
        if (SettingsData.GetSettingsData().InspectorDebug)
            Debug.Log(message);
    }
}
