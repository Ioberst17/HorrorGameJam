using UnityEngine;

public class AnimatedTexture : MonoBehaviour
{
    public float duration = 1.0f; // Duration of the animation in seconds
    int timePerFrame;
    public Texture2D[] frames;

    private int frameIndex;
    private MeshRenderer rendererMy;

    private float frameDuration;
    private float timer;

    private void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();

        frameDuration = duration / frames.Length;
        timer = frameDuration;

        NextFrame();
    }

    private void NextFrame()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 1) % frames.Length;

        if (frameIndex == 0)
        {
            StopFramesAnimation();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            NextFrame();
            timer = frameDuration;
        }
    }

    private void StopFramesAnimation()
    {
        Destroy(gameObject);
    }
}
