using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class BulkSpriteSlicer : EditorWindow
{
    private int sliceWidth = 250;
    private int sliceHeight = 250;
    private Texture2D[] selectedTextures;

    [MenuItem("Window/Bulk Sprite Slicer")]
    static void Init()
    {
        BulkSpriteSlicer window = (BulkSpriteSlicer)GetWindow(typeof(BulkSpriteSlicer));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Sprite Slicing Settings");
        sliceWidth = EditorGUILayout.IntField("Slice Width:", sliceWidth);
        sliceHeight = EditorGUILayout.IntField("Slice Height:", sliceHeight);

        GUILayout.Space(10);

        if (GUILayout.Button("Slice Selected Scripts"))
        {
            SliceSprites();
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Selected Textures:");
        selectedTextures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        if (selectedTextures != null && selectedTextures.Length > 0)
        {
            foreach (Texture2D texture in selectedTextures)
            {
                EditorGUILayout.LabelField(texture.name);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No PNG textures selected.");
        }
    }

    void SliceSprites()
    {
        if (selectedTextures == null || selectedTextures.Length == 0)
        {
            Debug.LogWarning("No PNG textures selected.");
            return;
        }

        foreach (Texture2D texture in selectedTextures)
        {
            Debug.Log("Slicing sprites for texture: " + texture.name);

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            for (int i = 0; i < texture.width; i += sliceWidth)
            {
                for (int j = texture.height; j > 0; j -= sliceHeight)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (texture.height - j) / sliceHeight + ", " + i / sliceWidth;
                    smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);

                    newData.Add(smd);
                }
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            Debug.Log("Done slicing sprites for texture: " + texture.name);
        }
    }
}
