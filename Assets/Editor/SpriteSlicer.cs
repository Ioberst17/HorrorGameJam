using UnityEditor;
using UnityEngine;

public class SpriteSlicer : EditorWindow
{
    [MenuItem("Custom/Slice Selected Sprite")]
    private static void SliceSelectedSprite()
    {
        Object selectedObject = Selection.activeObject;
        if (selectedObject == null || !(selectedObject is Texture2D))
        {
            Debug.LogWarning("Please select a Texture2D of TextureType Sprite (2D and UI) in the Project window.");
            return;
        }

        Texture2D texture = (Texture2D)selectedObject;
        string texturePath = AssetDatabase.GetAssetPath(texture);

        // Step 1: Resize the sprite to the specified pixel size
        int cellWidth = 256;
        int cellHeight = 181;
        Texture2D resizedTexture = new Texture2D(cellWidth, cellHeight);
        Graphics.ConvertTexture(texture, resizedTexture);

        // Step 2: Slice the sprite using grid by cell size
        int gapBetweenRows = 75;
        int rows = Mathf.FloorToInt((texture.height - gapBetweenRows) / (float)(cellHeight + gapBetweenRows));
        int columns = Mathf.FloorToInt(texture.width / (float)cellWidth);
        int totalSprites = rows * columns;

        SpriteMetaData[] spriteMetaData = new SpriteMetaData[totalSprites];
        int index = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Rect rect = new Rect(x * cellWidth, y * (cellHeight + gapBetweenRows), cellWidth, cellHeight);
                spriteMetaData[index].rect = rect;
                spriteMetaData[index].name = texture.name + "_" + index;
                index++;
            }
        }

        // Apply the sprite slicing to the existing texture
        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        textureImporter.isReadable = true;
        textureImporter.spritesheet = spriteMetaData;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);

        // Step 3: Save the changes
        EditorUtility.SetDirty(texture);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Sprite slicing completed for: " + texture.name);
    }
}
