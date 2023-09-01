using UnityEngine;

/// <summary>
/// Standalone hitbox used to visualize transforms 
/// </summary>
public class HitBoxVisualizer : MonoBehaviour
{
    public Transform transform1;
    public Transform transform2;
    GameObject hitboxPrefab;

    public Vector3 min;
    public Vector3 max;
    public Vector3 center;
    public Vector3 size;

    public float initialBoundsX;
    public float initialBoundsY;

    public float scaleFactorX;
    public float scaleFactorY;

    SpriteRenderer spriteRenderer;
    Collider2D hitBox;
    /// <summary>
    /// Set up references to transforms and sprite renderer, given the object will be inactive at first at runtime
    /// </summary>
    private void Awake()
    {
        gameObject.SetActive(true);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        hitBox = GetComponentInChildren<Collider2D>(true);
        hitboxPrefab = spriteRenderer.gameObject;
        
        spriteRenderer.gameObject.SetActive(true);

        initialBoundsX = spriteRenderer.bounds.size.x;
        initialBoundsY = spriteRenderer.bounds.size.y;
        
        spriteRenderer.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    /// <summary>
    ///  When the hitbox is enabled, set its position using the transforms and by scaling the renderer. The transforms are updated by an AttackManager script (or child of one)
    /// </summary>
    private void OnEnable()
    {
        spriteRenderer.gameObject.SetActive(true);

        // Calculate the bounds by considering negative values
        min = Vector3.Min(transform1.localPosition, transform2.localPosition);
        max = Vector3.Max(transform1.localPosition, transform2.localPosition);

        center = (min + max) * 0.5f;
        size = max - min;

        // Calculate the scale factors based on the size ratio
        scaleFactorX = Mathf.Abs(size.x) / initialBoundsX;
        scaleFactorY = Mathf.Abs(size.y) / initialBoundsY;

        spriteRenderer.gameObject.transform.localPosition = center;
        spriteRenderer.gameObject.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);        
        
        hitBox.gameObject.transform.localPosition = center;
        hitBox.gameObject.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1f);
    }


    private void OnDisable()
    {
        spriteRenderer.gameObject.SetActive(false);
    }
}
