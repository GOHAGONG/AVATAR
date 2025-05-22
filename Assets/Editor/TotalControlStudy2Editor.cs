using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TotalControlStudy2))]
public class TotalControlStudy2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw Select Method
        var selectedMethodProp = serializedObject.FindProperty("SelectedMethod");
        EditorGUILayout.PropertyField(selectedMethodProp);

        Current2Method selected = (Current2Method)selectedMethodProp.enumValueIndex;

        // Draw Customize Method Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Customize Method", EditorStyles.boldLabel);

        if (selected == Current2Method.Customize)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WalkMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CrouchMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("JumpMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CrawlMethod"));
        }
        else
        {
            // Only show readonly values for clarity
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("WalkMethod", (Custom2Method)selected);
            EditorGUILayout.EnumPopup("CrouchMethod", (Custom2Method)selected);
            EditorGUILayout.EnumPopup("JumpMethod", (Custom2Method)selected);
            EditorGUILayout.EnumPopup("CrawlMethod", (Custom2Method)selected);
            EditorGUI.EndDisabledGroup();
        }

        // Draw other default fields below (like Animator variables)
        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, 
            "SelectedMethod", "WalkMethod", "CrouchMethod", "JumpMethod", "CrawlMethod");

        serializedObject.ApplyModifiedProperties();
    }
}
