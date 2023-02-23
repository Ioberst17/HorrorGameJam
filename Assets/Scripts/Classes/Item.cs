using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    [SerializeField] public Consumables self;
    public int staticID;
    public SpriteRenderer spriteRenderer;
    public  void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        self = new Consumables();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void OnCollisionEnter2D(Collision2D Other)
    {
        if (Other.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log(name + " is triggered");
            EventSystem.current.ItemPickupTrigger(staticID, self.amount);
            gameObject.SetActive(false);
        }
    }
}
