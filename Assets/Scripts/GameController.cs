using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private DataManager dataManager;
    [SerializeField]
    private PlayerController PlayerController;
    private PlayerHealth playerHealth;
    private Shield playerShield;
    [SerializeField]
    private PlayerSkills playerSkills;
    [SerializeField]
    private PlayerPrimaryWeapon playerPrimaryWeapon;
    [SerializeField]
    private PlayerSecondaryWeapon playerSecondaryWeapon;
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
    public float yInput;
    public bool isGrounded;
    private bool pauseHelper;
    public bool AttackButton;
    public bool ShootButton;
    public bool JumpButton;
    public bool DashButton;
    public int AttackBuffer;
    public int ShootBuffer;
    public int JumpBuffer;
    public int DashBuffer;



    // Start is called before the first frame update
    void Start()
    {
        AttackBuffer = 0;
        ShootBuffer = 0;
        JumpBuffer = 0;
        DashBuffer = 0;

        dataManager = DataManager.Instance;
        playerSkills = new PlayerSkills();
        LoadPlayerSkills();

        playerHealth = PlayerController.gameObject.GetComponent<PlayerHealth>();
        playerShield = PlayerController.gameObject.GetComponentInChildren<Shield>();
        playerPrimaryWeapon = PlayerController.GetComponentInChildren<PlayerPrimaryWeapon>();
        playerSecondaryWeapon = PlayerController.GetComponentInChildren<PlayerSecondaryWeapon>();
        playerSkills.UnlockAllSkills();

        //EventSystem.current.onSkillUnlock += UnlockSkill;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }*/

        CheckInput();
        if (Input.GetKeyDown(KeyCode.M)) { SavePlayerSkills(); }
    }

    private void UnlockSkill(PlayerSkills.SkillType skill) { playerSkills.UnlockSkill(skill); }

    private void SavePlayerSkills() 
    {
        for (int i = 0; i < playerSkills.unlockedSkillsList.Count; i++)
        {
            if (!dataManager.gameData.playerSkills.unlockedSkillsList.Contains(playerSkills.unlockedSkillsList[i]))
                { dataManager.gameData.playerSkills.unlockedSkillsList.Add(playerSkills.unlockedSkillsList[i]); }
        }
    }

    private void LoadPlayerSkills() 
    {
        for (int i = 0; i < dataManager.gameData.playerSkills.unlockedSkillsList.Count; i++)
        {
            playerSkills.unlockedSkillsList.Add(dataManager.gameData.playerSkills.unlockedSkillsList[i]);
        }
    }

    private void FixedUpdate()
    {
        PlayerController.CheckGround();
        PlayerController.CheckWall();
        CalculateInputs();
        if (!PlayerController.isDashing)
        {
            PlayerController.ApplyMovement();
        }
    }

    public int GetHP() { return playerHealth.HP; }

    public int GetMP() { return PlayerController.MP; }
    public float GetSP() { return PlayerController.SP; }

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
            yInput = Input.GetAxisRaw("Vertical");
            //Debug.Log(xInput);

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("up")) //|| Input.GetKeyDown(KeyCode.W))
            {
                JumpButton = true;
            }
            if (Input.GetButtonUp("Jump") || Input.GetKeyUp("up")) //|| Input.GetKeyDown(KeyCode.W))
            {
                JumpButton = false;
                JumpBuffer = 0;
            }
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.U)) // if attack is triggered
            {
                AttackButton = true;
            }
            if (Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.U)) // if attack is triggered
            {
                AttackButton = true;
            }
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.U)) // if attack is released
            {
                AttackButton = false;
                AttackBuffer = 0;
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Y)) // if shoot is triggered
            {
                ShootButton = true;
            }
            if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Y)) // if shoot is released
            {
                ShootButton = false;
                ShootBuffer = 0;
                EventSystem.current.WeaponStopTrigger();
            }
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.LeftShift)) // if dash is triggered
            {
                DashButton = true;
            }
            if (Input.GetKeyUp(KeyCode.I) || Input.GetKeyUp(KeyCode.LeftShift)) // if dash is released
            {
                DashButton = false;
                DashBuffer = 0;
            }


            if ((Input.GetButtonDown("Jump") || Input.GetKeyDown("up")) && hasJump()) //|| Input.GetKeyDown(KeyCode.W))
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
                        playerPrimaryWeapon.Attack(0);
                    } 
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(1); } // shoot if Y, same logic used in below branches
                }
                else if((Input.GetKey(KeyCode.S) || Input.GetKey("down")) && !isGrounded){
                    if (Input.GetMouseButtonDown(0)) 
                    {
                        playerPrimaryWeapon.Attack(1);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(-1); }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0)) 
                    {
                        playerPrimaryWeapon.Attack(2);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(0); }
                }
            }

            if (Input.GetMouseButton(1) || Input.GetMouseButtonUp(0)) // if either attack or shoot is released
            {
                //Debug.Log("attack called");
                if (Input.GetKey(KeyCode.W) || Input.GetKey("up"))
                {
                    if (AttackButton == false) // melee attack if U
                    {
                        playerPrimaryWeapon.Release(0);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(1); } // shoot if Y, same logic used in below branches
                }
                else if ((Input.GetKey(KeyCode.S) || Input.GetKey("down")) && !isGrounded)
                {
                    if (AttackButton == false)
                    {
                        playerPrimaryWeapon.Release(1);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(-1); }
                }
                else
                {
                    if (AttackButton == false)
                    {
                        playerPrimaryWeapon.Release(2);
                    }
                    if (Input.GetMouseButton(1)) { EventSystem.current.AmmoCheckTrigger(0); }
                }
            }

            if (Input.GetMouseButtonUp(1)) { EventSystem.current.WeaponStopTrigger(); }
            if (Input.GetKeyDown(KeyCode.LeftShift) && hasDash()) { PlayerController.Dash(); }

            if(Input.GetKeyDown(KeyCode.F) && hasBlock()) { /* PORT CODE FROM PLAYERSHIELD HERE */}
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
    public void CalculateInputs()
    {
        if (Input.GetKeyDown(KeyCode.F) && hasBlock()) { }

        if (JumpButton && !(JumpBuffer > 5) && hasJump())
        {
            PlayerController.Jump();
            ++JumpBuffer;
        }
        if (DashButton && !(DashBuffer > 5) && hasDash())
        {
            PlayerController.Dash();
            ++DashBuffer;
        }
        if (AttackButton && !(AttackBuffer > 5))
        {
            //Debug.Log("attack called");
            if (yInput > 0.2f)
            {
                playerPrimaryWeapon.Attack(0);
            }
            else if (yInput < -0.2f && !isGrounded)
            {
                playerPrimaryWeapon.Attack(1);
            }
            else
            {
                playerPrimaryWeapon.Attack(2);
            }
            ++AttackBuffer;
        }
        if (ShootButton && !(ShootBuffer > 5))
        {
            //Debug.Log("attack called");
            if (yInput > 0.2f)
            {
                EventSystem.current.AmmoCheckTrigger(1);  // shoot if Y, same logic used in below branches
            }
            else
            {
                EventSystem.current.AmmoCheckTrigger(0);
            }
            ++ShootBuffer;
        }
        if (xInput > 0 && PlayerController.ControlMomentum < 10)
        {
            if (PlayerController.ControlMomentum == 0)
            {
                PlayerController.ControlMomentum = 5;
            }
            else
            {
                PlayerController.ControlMomentum += 1;
            }

        }
        else if (xInput < 0 && PlayerController.ControlMomentum > -10)
        {
            if (PlayerController.ControlMomentum == 0)
            {
                PlayerController.ControlMomentum = -5;
            }
            else
            {
                PlayerController.ControlMomentum -= 1;
            }
        }
        else if (xInput == 0)
        {
            if (PlayerController.ControlMomentum > 0)
            {
                PlayerController.ControlMomentum -= 1;
            }
            else if (PlayerController.ControlMomentum < 0)
            {
                PlayerController.ControlMomentum += 1;
            }
        }
        if (PlayerController.ControlMomentum > 10)
        {
            PlayerController.ControlMomentum -= 1;
        }
        else if (xInput < 0 && PlayerController.ControlMomentum < -10)
        {
            PlayerController.ControlMomentum += 1;
        }
        if (xInput >= 1 && PlayerController.facingDirection == -1)
        {
            HandleFlipping();
        }
        else if (xInput <= -1 && PlayerController.facingDirection == 1)
        {
            HandleFlipping();
        }
        else if (playerSecondaryWeapon.WeaponIsPointedToTheRight() && PlayerController.facingDirection == -1)
        {
            if (xInput >= 0) { HandleFlipping(); }
        }
        else if (!playerSecondaryWeapon.WeaponIsPointedToTheRight() && PlayerController.facingDirection == 1)
        {
            if (xInput <= 0) HandleFlipping();
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
        }

    }
    //public void passHit(string enemyname, int attackDamage, Vector3 playerPosition)
    //{
    //    //Debug.Log("flag2");
    //    EnemyController = GameObject.Find(enemyname).GetComponent<EnemyController>();
    //    EnemyController.Hit(attackDamage, playerPosition);
    //    Debug.Log("passing hit to " + enemyname);

    //}

    public void HandleFlipping()
    {
        PlayerController.Flip();
        playerSecondaryWeapon.Flip();
    }

    public bool hasJump() { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Jump); }
    public bool hasDash() { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Dash); }
    public bool hasFire() { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Fire); }
    public bool hasBlock() { return playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Block); }

    private void OnDestroy()
    {
        SavePlayerSkills();
        //EventSystem.current.onSkillUnlock -= UnlockSkill;
    }
}
