using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StringResourcesSettings : EditorWindow
{
    const string defaultPathToNewClass = @"D:\development\unity\MyGame - Thirty\Game\MyTestGameProject\Assets\Resources\TextAssets\Localization";
    const string defaultPathToStringResourcesDirectory = @"D:\development\unity\MyGame - Thirty\Game\MyTestGameProject\Assets\Scripts\main_scene\NonMonobehaviour\LocalizedStrings.cs";
    
    static string pathToNewClass = defaultPathToNewClass;
    static public string PathToNewClass { get { return pathToNewClass; } }

    static string pathToStringResourcesDirectory = defaultPathToStringResourcesDirectory;
    static public string PathToStringResourcesDirectory { get { return pathToStringResourcesDirectory; } }

    //temp textfield values
    string ptc = string.Empty, ptr = string.Empty;

    static public event Action<string, string> OnApply;

    [MenuItem("EditorHelperTools/StringResourcesWindow/Settings", priority = 0)]
    public static void ShowWindow()
    {
        GetWindow<StringResourcesSettings>("StrResSettings");
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey("StringResourcesSettings/pathToNewClass"))
        {
            pathToNewClass = EditorPrefs.GetString("StringResourcesSettings/pathToNewClass");
        }
        else
        {
            pathToNewClass = defaultPathToNewClass;
            EditorPrefs.SetString("StringResourcesSettings/pathToNewClass", pathToNewClass);
        }
        ptc = pathToNewClass;

        if (EditorPrefs.HasKey("StringResourcesSettings/pathToStringResourcesDirectory"))
        {
            pathToStringResourcesDirectory = EditorPrefs.GetString("StringResourcesSettings/pathToStringResourcesDirectory");
        }
        else
        {
            pathToStringResourcesDirectory = defaultPathToNewClass;
            EditorPrefs.SetString("StringResourcesSettings/pathToStringResourcesDirectory", pathToStringResourcesDirectory);
        }
        ptr = pathToStringResourcesDirectory;
    }

    private void OnDisable()
    {
        EditorPrefs.SetString("StringResourcesSettings/pathToNewClass", pathToNewClass);
        EditorPrefs.SetString("StringResourcesSettings/pathToStringResourcesDirectory", pathToStringResourcesDirectory);
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            if (ptc == pathToNewClass)
                GUILayout.Label("Путь к классу ресурсов:", EditorStyles.label);
            else
                GUILayout.Label("Путь к классу ресурсов:*", EditorStyles.boldLabel);

            ptc = GUILayout.TextField(ptc);
            EditorGUILayout.Space();

            if (ptr == PathToStringResourcesDirectory)
                GUILayout.Label("Путь к директории .xml файлов строковых ресурсов:", EditorStyles.label);
            else
                GUILayout.Label("Путь к директории .xml файлов строковых ресурсов:*", EditorStyles.boldLabel);

            ptr = GUILayout.TextField(ptr);
            EditorGUILayout.Space();
        }
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("По умолчнию"))
            {
                ptc = defaultPathToNewClass;
                ptr = defaultPathToStringResourcesDirectory;
            }

            if (GUILayout.Button("Отмена"))
            {
                ptc = pathToNewClass;
                ptr = PathToStringResourcesDirectory;
            }

            if (GUILayout.Button("Применить"))
            {
                pathToNewClass = ptc;
                pathToStringResourcesDirectory = ptr;

                if (OnApply != null) OnApply(pathToNewClass, pathToStringResourcesDirectory);
            }
        }
        GUILayout.EndHorizontal();
        
    }
}
