using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static ComponentFinder;
using static PlayerAnimator;

public class PlayerAnimator : BodyPartAnimator 
{
    [SerializeField] private Transform body, rightArm, leftArm;
    [SerializeField] private Animator baseAnimator, rightArmAnimator, leftArmAnimator;
    [SerializeField] private BaseAnimator baseAnimatorScript;
    [SerializeField] private RightArmAnimator rightArmScript;
    [SerializeField] private LeftArmAnimator leftArmScript;
    public enum PlayerPart { All, Body, RightArm, LeftArm }

    [System.Serializable]
    public class AnimatorAndScript<T> where T : BodyPartAnimator
    {
        public Animator anim;
        [SerializeField]
        public T script;

        public AnimatorAndScript(Animator anim, T script)
        {
            this.anim = anim;
            this.script = script;
        }
    }

    // dictionary that maps a enum to a relevant animator and a script (for calling more specific animations)
    [SerializeField] private SerializableDictionary<PlayerPart, AnimatorAndScript<BodyPartAnimator>> EnumToAnimatorMap = new SerializableDictionary<PlayerPart, AnimatorAndScript<BodyPartAnimator>>();

    [SerializeField] private SpriteRenderer[] spriteRenderers;

    // Get all references
    override public void Start()
    {
        body = GetComponentInChildrenByNameAndType<Transform>("Base", gameObject);
        baseAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", body.gameObject);
        baseAnimatorScript = GetComponentInChildrenByNameAndType<BaseAnimator>("SpriteAndAnimations", body.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.Body, new AnimatorAndScript<BodyPartAnimator>(baseAnimator, null)); // update null with middle line

        rightArm = GetComponentInChildrenByNameAndType<Transform>("RightArm", gameObject);
        rightArmAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", rightArm.gameObject);
        rightArmScript = GetComponentInChildrenByNameAndType<RightArmAnimator>("SpriteAndAnimations", rightArm.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.RightArm, new AnimatorAndScript<BodyPartAnimator>(rightArmAnimator, rightArmScript));

        leftArm = GetComponentInChildrenByNameAndType<Transform>("LeftArm", gameObject);
        leftArmAnimator = GetComponentInChildrenByNameAndType<Animator>("SpriteAndAnimations", leftArm.gameObject);
        leftArmScript = GetComponentInChildrenByNameAndType<LeftArmAnimator>("SpriteAndAnimations", leftArm.gameObject);
        EnumToAnimatorMap.Add(PlayerPart.LeftArm, new AnimatorAndScript<BodyPartAnimator>(leftArmAnimator, null)); // update null with middle line

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // play an animation
    public void Play(string animationName, PlayerPart playerPart = PlayerPart.All)
    {
        if (playerPart == PlayerPart.All)
        {
            baseAnimatorScript.Play(animationName);
            rightArmScript.Play(animationName);
            leftArmScript.Play(animationName);
        }
        else 
        { 
            EnumToAnimatorMap.TryGetValue(playerPart, out AnimatorAndScript<BodyPartAnimator> animatorAndScript);
            animatorAndScript.anim.Play(animationName);
        }
    }

    // play a coroutine that triggers a series of animations
    virtual public void PlayCoroutine(string animationName, PlayerPart playerPart = PlayerPart.All)
    {
        if (playerPart == PlayerPart.All)
        {
            baseAnimator.Play(animationName);
            rightArmAnimator.Play(animationName);
            leftArmAnimator.Play(animationName);
        }
        else
        {
            EnumToAnimatorMap.TryGetValue(playerPart, out AnimatorAndScript<BodyPartAnimator> animatorAndScript);
            animatorAndScript.script.PlayCoroutine(animationName);
        }
    }

    public bool CheckIfAnimationIsPlaying(string animationName, PlayerPart playerPart = PlayerPart.All)
    {
        if (playerPart == PlayerPart.All)
        {
            return CheckAnimationState(animationName, baseAnimator);
            //return CheckAnimationState(animationName, rightArmAnimator); // find a way to better implement reaching any animator
            //return CheckAnimationState(animationName, leftArmAnimator);
        }
        else
        {
            EnumToAnimatorMap.TryGetValue(playerPart, out AnimatorAndScript<BodyPartAnimator> animatorAndScript);
            return CheckAnimationState(animationName, animatorAndScript.anim);
        }
    }

    public void SpriteEnabled(bool state)
    {
        foreach(SpriteRenderer sprite in spriteRenderers)
        {
            sprite.enabled = state;
        }
    }

    //called by GameController to flip child objects when player should turn
    public void Flip()
    {
        foreach(PlayerPart part in EnumToAnimatorMap.Keys)
        {
            EnumToAnimatorMap.TryGetValue(part, out AnimatorAndScript<BodyPartAnimator> animatorAndScript);
            PhysicsExtensions.Flip(animatorAndScript.anim.gameObject);
        }
    }

    public Sprite ReturnPlayerImage() { return CombineSprites(spriteRenderers); }

    // RETURNS A SPRITE THAT IS A COMPOSITE OF THE PLAYER'S CHILD SPRITES
    private Sprite CombineSprites(SpriteRenderer[] spriteRenderers)
    {
        // Calculate the combined bounds
        Bounds combinedBounds = CalculateCombinedBounds(spriteRenderers);

        // Create a new GameObject to hold the combined sprite
        GameObject combinedObject = new GameObject("CombinedSprite");
        SpriteRenderer combinedRenderer = combinedObject.AddComponent<SpriteRenderer>();

        // Set the combined sprite's position and sorting order to match the first sprite
        combinedObject.transform.position = spriteRenderers[0].transform.position;
        combinedRenderer.sortingOrder = spriteRenderers[0].sortingOrder;

        // Set the combined sprite's size to match the combined bounds
        combinedRenderer.size = combinedBounds.size;

        // Create a texture to hold the combined pixels
        float pixelDensity = spriteRenderers[0].sprite.pixelsPerUnit;
        Texture2D combinedTexture = new Texture2D(
            Mathf.RoundToInt(combinedBounds.size.x * pixelDensity),
            Mathf.RoundToInt(combinedBounds.size.y * pixelDensity)
        );


        // Draw all the sprites onto the combined texture
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            SpriteRenderer spriteRenderer = spriteRenderers[i];
            Vector2 spritePosition = GetSpritePosition(spriteRenderer, combinedBounds);

            DrawSpriteOntoTexture(spriteRenderer.sprite, spritePosition, combinedTexture);
        }

        // Set the sprite for the combined renderer using the combined texture
        combinedRenderer.sprite = Sprite.Create(combinedTexture, new Rect(0, 0, combinedBounds.size.x, combinedBounds.size.y), Vector2.one * 0.5f);

        // Return the combined sprite
        return combinedRenderer.sprite;
    }

    // used by CombineSprites to combine bounds of individual sprites
    private Bounds CalculateCombinedBounds(SpriteRenderer[] spriteRenderers)
    {
        Bounds combinedBounds = new Bounds();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            Sprite sprite = spriteRenderer.sprite;
            Bounds spriteBounds = new Bounds(spriteRenderer.transform.position, Vector3.zero);
            spriteBounds.Encapsulate(spriteRenderer.bounds.min);
            spriteBounds.Encapsulate(spriteRenderer.bounds.max);

            combinedBounds.Encapsulate(spriteBounds);
        }

        return combinedBounds;
    }


    private Vector2 GetSpritePosition(SpriteRenderer spriteRenderer, Bounds combinedBounds)
    {
        return spriteRenderer.transform.position - combinedBounds.min;
    }

    private void DrawSpriteOntoTexture(Sprite sprite, Vector2 position, Texture2D texture)
    {
        int startX = Mathf.RoundToInt(position.x);
        int startY = Mathf.RoundToInt(position.y);
        int width = Mathf.RoundToInt(sprite.textureRect.width);
        int height = Mathf.RoundToInt(sprite.textureRect.height);

        Debug.Log("Sprite Rect: " + sprite.textureRect);
        Debug.Log("StartX: " + startX + ", StartY: " + startY);
        Debug.Log("Width: " + width + ", Height: " + height);

        Color[] spritePixels = sprite.texture.GetPixels(Mathf.RoundToInt(sprite.textureRect.min.x), Mathf.RoundToInt(sprite.textureRect.min.y), width, height);
        Color[] texturePixels = texture.GetPixels(startX, startY, width, height);

        for (int i = 0; i < spritePixels.Length; i++)
        {
            // Combine the sprite pixels with the texture pixels using alpha blending
            Color spritePixel = spritePixels[i];
            Color texturePixel = texturePixels[i];
            Color combinedPixel = Color.Lerp(texturePixel, spritePixel, spritePixel.a);
            texturePixels[i] = combinedPixel;
        }

        texture.SetPixels(startX, startY, width, height, texturePixels);
        texture.Apply();
    }



}
