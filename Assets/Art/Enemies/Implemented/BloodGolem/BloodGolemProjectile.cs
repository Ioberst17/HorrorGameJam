using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGolemProjectile : EnemyProjectile
{
    //private EnemyController enemyController;
    //private BloodGolemBehavior BGController;
    //private Vector3 MyDirection;
    //public float Movespeed;
    //public string ParentName;

    //// Start is called before the first frame update
    //new void Start()
    //{
    //    base.Start();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    transform.position += MyDirection * Movespeed * Time.deltaTime;
    //}
    //public void Setup(Vector3 ShotDirection, string nameofParent)
    //{
    //    MyDirection = ShotDirection;
    //    ParentName = nameofParent;
    //    enemyController = GameObject.Find(ParentName).GetComponent<EnemyController>();
    //    BGController = GameObject.Find(ParentName).GetComponent<BloodGolemBehavior>();
    //}
    //private void OnTriggerStay2D(Collider2D collider)
    //{
    //    //Debug.Log("I See something " + collider.name);
    //    if (collider.gameObject.layer == LayerMask.NameToLayer("Player")){
    //        BGController.BloodBallReset();
    //        Destroy(gameObject);
    //    }
    //    else if(collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
    //    {
    //        //Debug.Log("I See environment " + collider.name);
    //        BGController.BloodBallReset();
    //        Destroy(gameObject);
    //    }
    //}
}