using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlexibleGridLayout))]
public class EditorFlexibleGridLayout : Editor
{
    new FlexibleGridLayout target;

    private void OnEnable()
    {
        target = base.target as FlexibleGridLayout;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("horisontal space");
        EditorGUILayout.FloatField(target.FSpaceHorisontal);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("verical space");
        EditorGUILayout.FloatField(target.FSpaceVertical);
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

}
