using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerController playerController;
    [SerializeField] public int damageValue;
    private Vector3 startingLocation;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GameObject.Find("PlayerModel").GetComponent<PlayerController>();
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer
                == LayerMask.NameToLayer("Player"))
        {
            playerController.takeDamage(damageValue, 1);
        
            //if (isHazard)
            //{
            //    if (!playerController.goldenOn || !playerController.invincibilityOn)
            //    {
            //        playerController.takeDamage(damageValue, 1);
            //        MusicController.hazardDamage();
            //    }


            //}
        }
    }
   // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
