using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaceholderScript))]
public class PrefabDropDownEditor : Editor
{
    SerializedProperty selectedScript;

    private void OnEnable()
    {
        selectedScript = serializedObject.FindProperty("selectedScript");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(selectedScript);

        serializedObject.ApplyModifiedProperties();
    }
}
