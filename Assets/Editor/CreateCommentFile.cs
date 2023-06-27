using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateCommentFile : EditorWindow
{
    private string fileName = "";
    private string fileExtension = ".txt"; // Change the extension to ".md" for Markdown files

    [MenuItem("Assets/Create/Comment File")]
    private static void Init()
    {
        CreateCommentFile window = GetWindow<CreateCommentFile>("Create Comment File");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Comment File", EditorStyles.boldLabel);
        fileName = EditorGUILayout.TextField("File Name", fileName);

        if (GUILayout.Button("Create"))
        {
            CreateFile();
        }
    }

    private void CreateFile()
    {
        string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Invalid folder path!");
            return;
        }

        string filePath = Path.Combine(folderPath, fileName + fileExtension);
        if (File.Exists(filePath))
        {
            Debug.LogWarning("A file with the same name already exists!");
            return;
        }

        File.WriteAllText(filePath, "");

        AssetDatabase.Refresh();

        Debug.Log("Comment file created successfully at: " + filePath);
    }
}
