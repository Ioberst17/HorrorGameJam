using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerController PlayerController;
    [SerializeField]
    private MusicController MusicController;
    [SerializeField]
    private UIController UIController;
    private EnemyController EnemyController;
    [SerializeField]
    private CameraBehavior CameraBehavior;

    public string gameState;
    public bool isPaused;
    public float xInput;
    public bool isGrounded;
    private bool pauseHelper;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }
    private void FixedUpdate()
    {
        PlayerController.CheckGround();
        PlayerController.ApplyMovement();
    }

    public int GetHP()
    {
        return PlayerController.HP;
    }

    public int GetMP()
    {
        return PlayerController.MP;
    }

    public void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameState != "Starting")
        {
            Debug.Log("esc");
            pauseHandler();
        }
        if (!isPaused)
        {
            xInput = Input.GetAxis("Horizontal");
            //Debug.Log(xInput);

            if (xInput == 1 && PlayerController.facingDirection == -1)
            {
                PlayerController.Flip();
            }
            else if (xInput == -1 && PlayerController.facingDirection == 1)
            {
                PlayerController.Flip();
            }

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("up") || Input.GetKeyDown(KeyCode.W))
            {
                PlayerController.Jump();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                //Debug.Log("attack called");
                if(Input.GetKey(KeyCode.W) || Input.GetKey("up"))
                {
                    PlayerController.Attack(0);
                }
                else if((Input.GetKey(KeyCode.S) || Input.GetKey("down")) && !isGrounded){
                    PlayerController.Attack(1);
                }
                else
                {
                    PlayerController.Attack(2);
                }
            }
        }
    }

    public void pauseHandler()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            if (gameState == "starting")
            {
                pauseHelper = true;
            }
            else
            {
                pauseHelper = false;
            }
            isPaused = true;
            //MusicController.MusicSource.Pause();
            //MusicController.MusicSource2.Pause();
            //MusicController.InvincibleMusicSource.Pause();
            //MusicController.FXSource.Pause();

        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
            if (pauseHelper)
            {
                gameState = "starting";
            }
            else
            {
                gameState = "running";
            }
            //MusicController.MusicSource.UnPause();
            //MusicController.MusicSource2.UnPause();
            //MusicController.InvincibleMusicSource.UnPause();
            //MusicController.FXSource.UnPause();
        }

    }
    public void passHit(string name , int attackDamage)
    {
        EnemyController = GameObject.Find(name).GetComponent<EnemyController>();
        EnemyController.calculateHit(attackDamage);
        //Debug.Log(name);
    }
}
