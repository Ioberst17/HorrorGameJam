using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerWeaponAnimator))]
public class SerializableDictionaryInspector : Editor
{
    //private SerializedProperty weaponAnimationClips;

    //private void OnEnable()
    //{
    //    weaponAnimationClips = serializedObject.FindProperty("weaponAnimationClips");
    //}

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();

    //    DrawDefaultInspector();

    //    EditorGUILayout.Space();

    //    EditorGUILayout.LabelField("Weapon Animation Clips");

    //    EditorGUI.indentLevel++;

    //    weaponAnimationClips.isExpanded = EditorGUILayout.Foldout(weaponAnimationClips.isExpanded, "Dictionary", true);

    //    if (weaponAnimationClips.isExpanded)
    //    {
    //        EditorGUI.indentLevel++;

    //        SerializedProperty keys = weaponAnimationClips.FindPropertyRelative("keys");
    //        SerializedProperty values = weaponAnimationClips.FindPropertyRelative("values");

    //        if (keys == null || values == null)
    //        {
    //            EditorGUILayout.HelpBox("The keys or values property is null!", MessageType.Error);
    //        }
    //        else if (keys.arraySize != values.arraySize)
    //        {
    //            EditorGUILayout.HelpBox("The number of keys and values in the SerializableDictionary does not match!", MessageType.Error);
    //        }
    //        else
    //        {
    //            for (int i = 0; i < keys.arraySize; i++)
    //            {
    //                SerializedProperty key = keys.GetArrayElementAtIndex(i);
    //                SerializedProperty value = values.GetArrayElementAtIndex(i);

    //                EditorGUILayout.PropertyField(key);
    //                EditorGUILayout.PropertyField(value);
    //            }
    //        }

    //        EditorGUI.indentLevel--;
    //    }

    //    EditorGUI.indentLevel--;

    //    serializedObject.ApplyModifiedProperties();
    //}
}
