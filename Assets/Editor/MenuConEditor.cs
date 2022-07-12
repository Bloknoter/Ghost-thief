using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MenuController))]
[CanEditMultipleObjects]
public class MenuConEditor : Editor
{
    private const string ASSET_FOLDER = "ToolData";
    private const string ASSET_NAME = "PagesData";

    private PagesData pagesData;

    private SerializedProperty StartPageName;


    private MenuController controller;

    private void OnEnable()
    {
        StartPageName = serializedObject.FindProperty("StartPageName");
        controller = (MenuController)target;
        if (pagesData == null)
            LoadData();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        controller = (MenuController)target;

        if (controller.pagesobjects == null)
        {
            controller.pagesobjects = new List<GameObject>();
        }

        EditorGUILayout.HelpBox("The work with the names of pages does not depends on the string case and the spaces.", MessageType.Info);

        if (pagesData == null)
        {
            EditorGUILayout.HelpBox("The pages data file could not be loaded. Be sure that this file is situated in this directory: " +
                "Assets/" + ASSET_FOLDER, MessageType.Warning);
            if (GUILayout.Button("Reload pages data", GUILayout.Width(250)))
            {
                LoadData();
            }
            return;
        }

        //  Начальная страница, открывающаяся при запуске сцены. Поле и предупреждение, если поле пусто
        if (StartPageName.stringValue == null)
            StartPageName.stringValue = "";
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Start page");
        StartPageName.stringValue = EditorGUILayout.TextField(StartPageName.stringValue);
        EditorGUILayout.EndHorizontal();
        if (StartPageName.stringValue.Length == 0)
        {
            EditorGUILayout.HelpBox("Start page is not set. On start the firts page in list will be opened", MessageType.Warning);
        }

        //  Кнопка добавления новой страницы
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add page", GUILayout.Width(100)))
        {
            AddPage();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("___________________________________________________________________");
        // Цикл, отображающий информацию об каждой странице
        for (int i = 0; i < pagesData.pages.Count; i++)
        {
            Page currpage = pagesData.pages[i];

            //  Первая строка, Поле названия страницы и кнопка удаления страницы
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name");
            currpage.Name = EditorGUILayout.TextField(currpage.Name);
            if (GUILayout.Button("Delete", GUILayout.Width(70)))
            {
                DeletePage(i);
            }
            EditorGUILayout.EndHorizontal();

            if (currpage.Name == null)
                currpage.Name = "";

            //Предупреждение, если имя страницы пустое
            if (currpage.Name.Length == 0)
            {
                EditorGUILayout.HelpBox("Name is not set!", MessageType.Warning);
            }
            GameObject pageGameObject;
            // Поле объекта страницы и ошибка, если поле равно null
            if (currpage.objectindex != -1)
            {
                pageGameObject = (GameObject)EditorGUILayout.ObjectField("Object", controller.pagesobjects[currpage.objectindex],
                    typeof(GameObject), true);
            }
            else
            {
                pageGameObject = (GameObject)EditorGUILayout.ObjectField("Object", null, typeof(GameObject), true);
            }

            if (pageGameObject == null)
            {
                currpage.objectindex = -1;
                EditorGUILayout.HelpBox("Object of page is null!", MessageType.Error);
            }
            else
            {
                if(currpage.objectindex == -1)
                {
                    if (controller.pagesobjects.Contains(pageGameObject))
                    {
                        currpage.objectindex = -1;
                        EditorGUILayout.HelpBox("Object is already used by another page!", MessageType.Error);
                    }
                    else
                    {
                        controller.pagesobjects.Add(pageGameObject);
                        currpage.objectindex = controller.pagesobjects.Count - 1;
                    }
                }
                else
                {
                    if (pageGameObject != controller.pagesobjects[currpage.objectindex])
                    {
                        if (controller.pagesobjects.Contains(pageGameObject))
                        {
                            EditorGUILayout.HelpBox("Object is already used by another page!", MessageType.Error);
                        }
                        else
                        {
                            controller.pagesobjects.RemoveAt(currpage.objectindex);
                            controller.pagesobjects.Add(pageGameObject);
                            currpage.objectindex = controller.pagesobjects.Count - 1;
                        }
                    }
                    
                }
                
            }

            // Если список переходов равен null , то создаётся пустой список
            if (currpage.transitions == null)
            {
                currpage.transitions = new List<string>();
            }

            // Кнопка добавления перехода
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Transitions on pages");
            if (GUILayout.Button("Add transition", GUILayout.Width(140)))
            {
                currpage.transitions.Add("transition");
            }
            EditorGUILayout.EndHorizontal();

            // Цикл, отображающий все переходы
            for(int t = 0; t < currpage.transitions.Count;t++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("transition on");
                currpage.transitions[t] = EditorGUILayout.TextField(currpage.transitions[t]);
                if (GUILayout.Button("Delete", GUILayout.Width(70)))
                {
                    currpage.transitions.RemoveAt(t);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.LabelField("______________________________________________________________________________");
        }

        if (GUI.changed)
        {
            if (pagesData != null)
                EditorUtility.SetDirty(pagesData);
            EditorUtility.SetDirty((MenuController)target);
        }

        serializedObject.ApplyModifiedProperties();

        controller.pagesData = pagesData;
    }

    private void LoadData()
    {
        if(!AssetDatabase.IsValidFolder(GetAssetFolderPath()))
        {
            AssetDatabase.CreateFolder("Assets", ASSET_FOLDER);
        }
        pagesData = (PagesData)AssetDatabase.LoadAssetAtPath(GetAssetPath(), typeof(PagesData));
        if(pagesData == null)
        {
            AssetDatabase.CreateAsset(CreateInstance<PagesData>(), GetAssetPath());
            AssetDatabase.ImportAsset(GetAssetPath());
            pagesData = (PagesData)AssetDatabase.LoadAssetAtPath(GetAssetPath(), typeof(PagesData));
            if (pagesData == null)
            {
                Debug.LogError("The new pages data file could not be created at path: " + GetAssetPath() + " !");
            }
            return;
        }
        pagesData = (PagesData)AssetDatabase.LoadAssetAtPath(GetAssetPath(), typeof(PagesData));
        if (pagesData == null)
        {
            Debug.LogError("The new pages data file could not be loaded at path: " + GetAssetPath() + " !");
            return;
        }
        Object[] objectspages = AssetDatabase.LoadAllAssetRepresentationsAtPath(GetAssetPath());
        if (objectspages != null && objectspages.Length > 0)
        {
            pagesData.pages.Clear();
            for (int i = 0; i < objectspages.Length; i++)
            {
                pagesData.pages.Add((Page)objectspages[i]);
            }
        }
    }

    private void AddPage()
    {
        Page page = CreateInstance<Page>();
        page.name = "Page";
        page.objectindex = -1;
        AssetDatabase.AddObjectToAsset(page, pagesData);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(page));
        pagesData.pages.Add(page);
    }

    private void DeletePage(int index)
    {
        Page page = pagesData.pages[index];
        pagesData.pages.RemoveAt(index);
        if (page.objectindex != -1)
            controller.pagesobjects.RemoveAt(page.objectindex);
        AssetDatabase.RemoveObjectFromAsset(page);
    }

    private string GetAssetFolderPath()
    {
        return "Assets/" + ASSET_FOLDER;
    }

    private string GetAssetPath()
    {
        return "Assets/" + ASSET_FOLDER + "/" + ASSET_NAME + ".asset";
    }
}
