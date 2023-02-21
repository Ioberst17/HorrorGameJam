using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerController PlayerController;
    [SerializeField]
    private PlayerWeapon PlayerWeapon;
    [SerializeField]
    private MusicController MusicController;
    [SerializeField]
    private UIController UIController;
    private EnemyController EnemyController;
    [SerializeField]
    private CameraBehavior CameraBehavior;
    [SerializeField]
    private SpawnManager SpawnManager;

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
        /*if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }*/

        CheckInput();
    }
    private void FixedUpdate()
    {
        PlayerController.CheckGround();
        PlayerController.CheckWall();
        if (!PlayerController.isDashing)
        {
            PlayerController.ApplyMovement();
        }
        
    }

    public int GetHP()
    {
        return PlayerController.HP;
    }

    public int GetMP()
    {
        return PlayerController.MP;
    }
    public int GetSP()
    {
        return PlayerController.SP;
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
            xInput = Input.GetAxisRaw("Horizontal");
            if(xInput>0 && PlayerController.ControlMomentum < 50)
            {
                PlayerController.ControlMomentum += 1;
            }
            else if (xInput < 0 && PlayerController.ControlMomentum > -50)
            {
                PlayerController.ControlMomentum -= 1;
            }
            else if(xInput == 0)
            {
                if(PlayerController.ControlMomentum > 0)
                {
                    PlayerController.ControlMomentum -= 1;
                }
                else if (PlayerController.ControlMomentum < 0)
                {
                    PlayerController.ControlMomentum += 1;
                }
            }
            if (PlayerController.ControlMomentum > 50)
            {
                PlayerController.ControlMomentum -= 1;
            }
            else if (PlayerController.ControlMomentum < -50)
            {
                PlayerController.ControlMomentum += 1;
            }
            //Debug.Log(xInput);

            if ((xInput >= 1 || PlayerWeapon.WeaponIsPointedToTheRight()) && PlayerController.facingDirection == -1)
            {
                PlayerController.Flip();
                PlayerWeapon.Flip();
            }
            else if ((xInput <= -1 || !PlayerWeapon.WeaponIsPointedToTheRight()) && PlayerController.facingDirection == 1)
            {
                PlayerController.Flip();
                PlayerWeapon.Flip();
            }

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("up") ) //|| Input.GetKeyDown(KeyCode.W))
            {
                PlayerController.Jump();
            }
            if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(0)) // if either attack or shoot is triggered
            {
                //Debug.Log("attack called");
                if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
                {
                    if (Input.GetMouseButtonDown(0)) // melee attack if U
                    { 
                        PlayerController.Attack(0);
                    } 
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(1); } // shoot if Y, same logic used in below branches
                }
                else if((Input.GetKey(KeyCode.S) || Input.GetKey("down")) && !isGrounded){
                    if (Input.GetMouseButtonDown(0)) 
                    { 
                        PlayerController.Attack(1);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(-1); }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0)) 
                    { 
                        PlayerController.Attack(2);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(0); }
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                PlayerController.Dash();
            }
            // For Spawns
            if (Input.GetKeyDown(KeyCode.Alpha1)) { SpawnManager.SpawnEnemy(0); };
            if (Input.GetKeyDown(KeyCode.Alpha2)) { SpawnManager.SpawnEnemy(1); };
            if (Input.GetKeyDown(KeyCode.Alpha3)) { SpawnManager.SpawnEnemy(2); };
            if (Input.GetKeyDown(KeyCode.Alpha4)) { SpawnManager.SpawnEnemy(3); };
            if (Input.GetKeyDown(KeyCode.Alpha5)) { SpawnManager.SpawnEnemy(4); };
            if (Input.GetKeyDown(KeyCode.Alpha6)) { SpawnManager.SpawnEnemy(5); };
            if (Input.GetKeyDown(KeyCode.Alpha7)) { SpawnManager.SpawnEnemy(6); };

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
    public void passHit(string enemyname, int attackDamage, Vector3 playerPosition)
    {
        //Debug.Log("flag2");
        EnemyController = GameObject.Find(enemyname).GetComponent<EnemyController>();
        EnemyController.calculateHit(attackDamage, playerPosition);
        Debug.Log("passing hit to " + enemyname);

    }
}
