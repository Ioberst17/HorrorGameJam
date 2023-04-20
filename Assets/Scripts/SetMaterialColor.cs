using UnityEngine;

public class SetMaterialColor : MonoBehaviour
{
    public Material material;
    public float lightenFactor = 0f;

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color += Color.white * lightenFactor;
            spriteRenderer.color = color;
        }
    }
}
