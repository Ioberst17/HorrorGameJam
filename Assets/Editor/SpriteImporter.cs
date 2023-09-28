using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SpriteImporter : MonoBehaviour
{
    public static SpriteImporter instance;

    [Serializable]
    public class AnimationAction
    {
        public string key;
        public List<ActionDirection> direction;
    }

    [Serializable]
    public class ActionDirection
    {
        public string key;
        public List<FrameSprite> frames;
    }

    [Serializable]
    public class FrameSprite
    {
        public string key;
        public Sprite sprite;
    }

    public string savePath = "Assets/Art/Spritesheets/anim/";

    public List<AnimationAction> frameSprites = new List<AnimationAction>();

    void Start()
    {
        if (instance == null)
        {
            Debug.Log("Creating Instance of SpriteImporter");
            instance = this;
        }
    }

    static void CreateInstance()
    {
        // Get spriteImporter Object
        GameObject spriteImporter = GameObject.FindWithTag("SpriteImporter");

        // Delete all components
        foreach (var comp in spriteImporter.GetComponents<Component>())
        {
            if (!(comp is Transform))
            {
                DestroyImmediate(comp);
            }
        }

        // Add SpriteImporter Component
        instance = spriteImporter.AddComponent(typeof(SpriteImporter)) as SpriteImporter;
    }

    [MenuItem("AssetDatabase/ImportSprite")]
    static void ImportSprite()
    {
        if (instance == null)
            CreateInstance();

        ResetData();

        Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Art/Spritesheets/Prince.png", typeof(Texture2D));
        string spriteSheet = AssetDatabase.GetAssetPath(t);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
        foreach (Sprite s in sprites)
        {
            string[] parts = s.name.Split("-");

            // Get or Create Animation Action
            AnimationAction action = GetAction(parts[0]);
            if (action == null)
            {
                action = new AnimationAction();
                action.key = parts[0];
                action.direction = new List<ActionDirection>();
                AddAction(action);
            }

            // Get or Create ActionDirection
            ActionDirection direction = GetDirection(action, parts[1]);
            if (direction == null)
            {
                direction = new ActionDirection();
                direction.key = parts[1];
                direction.frames = new List<FrameSprite>();
                AddDirection(action, direction);
            }

            // Add Frames to Direction
            FrameSprite fs = new FrameSprite();
            fs.key = parts[2];
            fs.sprite = s;
            direction.frames.Add(fs);
        }

        // Process Animations
        CreateAnimations();
    }

    static void ResetData()
    {
        instance.frameSprites = new List<AnimationAction>();
    }

    static AnimationAction GetAction(string key)
    {
        foreach (var item in instance.frameSprites)
        {
            if (item.key == key)
                return item;
        }
        return null;
    }

    static void AddAction(AnimationAction action)
    {
        instance.frameSprites.Add(action);
    }

    static ActionDirection GetDirection(AnimationAction action, string dir)
    {
        foreach (var item in action.direction)
        {
            if (item.key == dir)
                return item;
        }
        return null;
    }

    static void AddDirection(AnimationAction action, ActionDirection direction)
    {
        action.direction.Add(direction);
    }

    static void CreateAnimations()
    {
        string newPath = Path.GetDirectoryName(instance.savePath);
        foreach (AnimationAction action in instance.frameSprites)
        {
            foreach (ActionDirection direction in action.direction)
            {
                Debug.Log("Creating " + newPath + "\\" + action.key + "-" + direction.key + ".anim");

                //Create the base AnimationClip
                AnimationClip clip = new AnimationClip();
                clip.frameRate = 12f;

                //Create the CurveBinding
                EditorCurveBinding spriteBinding = new EditorCurveBinding();
                spriteBinding.type = typeof(SpriteRenderer);
                spriteBinding.path = "";
                spriteBinding.propertyName = "m_Sprite";

                //Create the KeyFrames
                ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[direction.frames.Count];
                int j = 0;
                foreach (FrameSprite sprite in direction.frames)
                {
                    spriteKeyFrames[j] = new ObjectReferenceKeyframe();
                    spriteKeyFrames[j].time = j / clip.frameRate;
                    spriteKeyFrames[j].value = sprite.sprite;
                    j++;
                }
                AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);

                //Set Loop Time to True
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);

                //Save the clip
                AssetDatabase.CreateAsset(clip, newPath + "\\" + action.key + "-" + direction.key + ".anim");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}