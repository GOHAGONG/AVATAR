using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TotalControl))]
public class TotalControlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw Select Method
        var selectedMethodProp = serializedObject.FindProperty("SelectedMethod");
        EditorGUILayout.PropertyField(selectedMethodProp);

        CurrentMethod selected = (CurrentMethod)selectedMethodProp.enumValueIndex;

        // Draw Customize Method Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Customize Method", EditorStyles.boldLabel);

        if (selected == CurrentMethod.Customize)
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
            EditorGUILayout.EnumPopup("WalkMethod", (CustomMethod)selected);
            EditorGUILayout.EnumPopup("CrouchMethod", (CustomMethod)selected);
            EditorGUILayout.EnumPopup("JumpMethod", (CustomMethod)selected);
            EditorGUILayout.EnumPopup("CrawlMethod", (CustomMethod)selected);
            EditorGUI.EndDisabledGroup();
        }

        // Draw other default fields below (like Animator variables)
        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, 
            "SelectedMethod", "WalkMethod", "CrouchMethod", "JumpMethod", "CrawlMethod");

        serializedObject.ApplyModifiedProperties();
    }
}
