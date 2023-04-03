//using UnityEngine;
//using UnityEditor;
//using UnityEngine.SceneManagement;

//[CustomEditor(typeof(EnemySpawnManager))]
//public class EnemySpawnManagerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        DrawDefaultInspector();

//        EnemySpawnManager manager = (EnemySpawnManager)target;

//        // Draw a button for copying the position of a selected transform to the clipboard
//        if (GUILayout.Button("Copy Position"))
//        {
//            GameObject selectedObject = Selection.activeGameObject;
//            if (selectedObject != null)
//            {
//                Transform selectedTransform = selectedObject.transform;
//                string positionString = selectedTransform.position.x + "," + selectedTransform.position.y + "," + selectedTransform.position.z;
//                EditorGUIUtility.systemCopyBuffer = positionString;
//            }
//        }

//        // Draw a button for pasting the clipboard contents as a new spawn point
//        if (GUILayout.Button("Paste Position"))
//        {
//            string positionString = EditorGUIUtility.systemCopyBuffer;
//            if (!string.IsNullOrEmpty(positionString))
//            {
//                string[] positionValues = positionString.Split(',');
//                if (positionValues.Length == 3 && float.TryParse(positionValues[0], out float x) && float.TryParse(positionValues[1], out float y) && float.TryParse(positionValues[2], out float z))
//                {
//                    GameObject selectedObject = Selection.activeGameObject;
//                    if (selectedObject != null)
//                    {
//                        int waveIndex = manager.currentWaveIndex;
//                        SerializedProperty spawnPoints = serializedObject.FindProperty("waves").GetArrayElementAtIndex(waveIndex).FindPropertyRelative("spawnPoints");
//                        int index = spawnPoints.arraySize;
//                        spawnPoints.InsertArrayElementAtIndex(index);
//                        SerializedProperty spawnPoint = spawnPoints.GetArrayElementAtIndex(index);
//                        spawnPoint.FindPropertyRelative("position").vector3Value = new Vector3(x, y, z);
//                        spawnPoint.FindPropertyRelative("rotation").quaternionValue = Quaternion.identity;
//                        serializedObject.ApplyModifiedProperties();
//                    }
//                }
//            }
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
