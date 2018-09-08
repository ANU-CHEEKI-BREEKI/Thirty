using StringResourceClassGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class StringResourcesWindow : EditorWindow
{
    bool setUp;
    CalassGenerator classGenerator;
    StringResourcesGenerator resourseGenerator;
    int selected = 0;
    Sheet activeSheet;
    List<string> allNames;
    Dictionary<string, string> allPropsValues;
    float scrollPos;

    int index = 0;
    int count = 0;

    private void OnEnable()
    {
        setUp = true;        
    }

    [MenuItem("EditorHelperTools/StringResourcesWindow/Resources")]
    public static void ShowWindow()
    {
        GetWindow<StringResourcesWindow>("StrResources");
    }

    private void SetUp()
    {
        classGenerator = new CalassGenerator
        (
            StringResourcesSettings.PathToNewClass,
            StringResourcesSettings.PathToStringResourcesDirectory
        );

        resourseGenerator = new StringResourcesGenerator(StringResourcesSettings.PathToStringResourcesDirectory);
        resourseGenerator.Init();
        activeSheet = resourseGenerator.GetSheet(resourseGenerator.AllFilesName[selected]);

        StringResourcesSettings.OnApply += (a, b) =>
        {
            classGenerator = new CalassGenerator(a, b);

            resourseGenerator = new StringResourcesGenerator(b);
            resourseGenerator.Init();
            activeSheet = resourseGenerator.GetSheet(resourseGenerator.AllFilesName[selected]);
        };

        if (allPropsValues == null)
            allPropsValues = new Dictionary<string, string>();
        else
            allPropsValues.Clear();


        if(allNames == null)
            allNames = new List<string>(resourseGenerator.AllPropNames);
        else
        {
            allNames.Clear();
            allNames.AddRange(resourseGenerator.AllPropNames);
        }


        scrollPos = 0;
    }

    private void OnGUI()
    {
        //set up
        if (setUp)
        {
            SetUp();
            setUp = false;
        }

        //draw all elements
        var arr = resourseGenerator.AllFilesName;
        var oldSelected = selected;
        selected = EditorGUILayout.Popup(selected, arr);

        if(selected != oldSelected)
        {
            oldSelected = selected;
            SetUp();
        }

        DrawAllProps(activeSheet, allNames);

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply"))
        {
            foreach (var item in allPropsValues)
                activeSheet.props.Add(item.Key, item.Value);
            resourseGenerator.Generate(activeSheet, allNames.ToArray());
            setUp = true;
        }
    }
    
    void DrawAllProps(Sheet sheet, List<string> allPropNames)
    {
        GUILayout.BeginHorizontal();
        {            
            count = (int)(this.position.height - 120) / 60 + 1;
            index = (int)(scrollPos * (((allPropNames.Count * 60) - (this.position.height - 120)) / (allPropNames.Count * 60)))/ 60;
            if (index > allPropNames.Count - count + 1)
                index = allPropNames.Count - count + 1;

            GUILayout.BeginVertical("box");
            {
                for (int i = index; i < allPropNames.Count && i < count + index; i++)
                {
                    if (sheet.props.ContainsKey(allPropNames[i]))
                    {
                        var val = sheet.props[allPropNames[i]];
                        var key = allPropNames[i];
                        if (DrawProperty(ref key, ref val, i - index + 1, i))
                            break;
                        if (key != allPropNames[i])
                        {
                            sheet.props.Remove(allPropNames[i]);
                            if (!sheet.props.ContainsKey(key))
                                sheet.props.Add(key, val);
                            allPropNames[i] = key;
                        }
                        else
                        {
                            sheet.props[key] = val;
                        }
                    }
                    else
                    {
                        if (!allPropsValues.ContainsKey(allPropNames[i]))
                            allPropsValues.Add(allPropNames[i], string.Empty);

                        var val = allPropsValues[allPropNames[i]];
                        var key = allPropNames[i];
                        if (DrawProperty(ref key, ref val, i - index + 1, i))
                            break;
                        if (key != allPropNames[i])
                        {
                            allPropsValues.Remove(key);
                            if (!allPropsValues.ContainsKey(key))
                                allPropsValues.Add(key, val);
                            allPropNames[i] = key;
                        }
                        else
                        {
                            allPropsValues[key] = val;
                        }
                    }
                }
            }
            GUILayout.EndVertical();

            var ev = Event.current;
            if (ev.isScrollWheel)
            {
                scrollPos += ev.delta.y * 20;
                Repaint();
            }

            scrollPos = GUILayout.VerticalScrollbar
            (
                scrollPos,
                1,
                0,
                resourseGenerator.AllPropNames.Length * 60,
                GUILayout.Height(count * 60)
            );// EditorGUILayout.BeginScrollView(scrollPos);

        }
        GUILayout.EndHorizontal();
    }

    //один скрол - это 60px
    //это приблезитеьно один такой блок. поэтому надо сделать этот блок ровно 60
    bool DrawProperty(ref string name, ref string value, int displayIndex, int wholeIndex)
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("name ", GUILayout.Width(40), GUILayout.Height(20));
                name = GUILayout.TextField(name, GUILayout.Height(20));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("value", GUILayout.Width(40), GUILayout.Height(20));
                value = GUILayout.TextArea(value, GUILayout.Height(31));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.Space(5);

        if(GUI.Button(new Rect(10, displayIndex * 60 + 5, 30, 15), "add"))
        {
            if(!allNames.Contains("newName"))
            allNames.Insert(wholeIndex + 1, "newName");
            return true;
        }
        return false;
    }

    
}
