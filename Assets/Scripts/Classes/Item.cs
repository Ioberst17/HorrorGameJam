using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [SerializeField] public Consumables self;
    public Collider2D boundary;
    public SpriteRenderer spriteRenderer;
    public  void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        self = new Consumables();
        boundary = gameObject.GetComponent<Collider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void OnTriggerEnter2D(Collider2D Other)
    {
        Pickup(Other);
    }

    public void Pickup(Collider2D Other)
    {
        if (Other.gameObject.GetComponent<PlayerController>() != null)
        {
            EventSystem.current.ItemPickupTrigger(self.id, self.amount);
            gameObject.SetActive(false);
        }
    }
}
