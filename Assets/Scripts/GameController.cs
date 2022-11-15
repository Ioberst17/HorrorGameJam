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

            if (xInput >= 1 && PlayerController.facingDirection == -1)
            {
                PlayerController.Flip();
                PlayerWeapon.Flip();
            }
            else if (xInput <= -1 && PlayerController.facingDirection == 1)
            {
                PlayerController.Flip();
                PlayerWeapon.Flip();
            }

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("up") ) //|| Input.GetKeyDown(KeyCode.W))
            {
                PlayerController.Jump();
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                //Debug.Log("attack called");
                if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
                {
                    PlayerController.Attack(0);
                    EventSystem.current.AmmoCheckTrigger(1);
                    //EventSystem.current.WeaponAmmoTrigger(1, -1, 0);
                }
                else if((Input.GetKey(KeyCode.S) || Input.GetKey("down")) && !isGrounded){
                    PlayerController.Attack(1);
                    EventSystem.current.AmmoCheckTrigger(0);
                    //EventSystem.current.WeaponAmmoTrigger(1, -1, 1);
                }
                else
                {
                    PlayerController.Attack(2);
                    EventSystem.current.AmmoCheckTrigger(0);
                    //EventSystem.current.WeaponAmmoTrigger(1, -1, 2);
                }
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                PlayerController.Dash();
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
    public void passHit(string enemyname, int attackDamage, Vector3 playerPosition)
    {
        //Debug.Log("flag2");
        EnemyController = GameObject.Find(enemyname).GetComponent<EnemyController>();
        EnemyController.calculateHit(attackDamage, playerPosition);
        Debug.Log("passing hit to " + enemyname);

    }
}
