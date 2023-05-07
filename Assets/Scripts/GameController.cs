using System.IO;
using System.Net.Http;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class GameController : MonoBehaviour
{
    private DataManager dataManager;

    // Player Related
    [SerializeField] private PlayerController playerController;
    private PlayerHealth playerHealth;
    private PlayerShield playerShield;
    [SerializeField] private PlayerSkills playerSkills;
    [SerializeField] private PlayerPrimaryWeapon playerPrimaryWeapon;
    [SerializeField] private PlayerSecondaryWeapon playerSecondaryWeapon;
    [SerializeField] private UIController UIController;

    // Interactable Objects In Scene
    private Door_Script[] doors;
    private DialogueTrigger[] dialogueTriggers;

    // Other
    [SerializeField] private CameraBehavior CameraBehavior;
    [SerializeField] private EnemyCreationForTesting enemySpawner;
    public Camera cameraToUse; 

    // Trackers for states
    public string gameState;
    public bool isPaused;

    private bool pauseHelper;

    // simpler button state trackers
    public bool InteractButton, JumpButton, DashButton, ShieldButton;
    private bool pauseButtonDown;

    // need to track more due to hold interaction
    public bool AttackButtonDown, AttackButtonHeld, AttackButtonRelease;
    public bool ShootButtonDown, ShootButtonHeld, ShootButtonRelease;

    public int AttackBuffer;
    public int ShootBuffer;
    public int JumpBuffer;
    public int DashBuffer;

    // Stores analog information for other functions
    public float xInput, yInput;
    public Vector2 lookInput, lookInputScreen, lookInputWorld; // raw unprocessed input, relative to screen position, and relative to world position
    public Vector2 playerPosition, playerPositionScreen, playerPositionWorld; // relative differences are used to get rotations via vectors

    // New Input System Asset
    public PlayerInput playerInput;
    public PlayerControls playerControls;

    // Gameplay Actions from InputSystemAsset
    private PlayerControls.PlayerActions playerActions;
    private InputAction move, look, interact, attack, fire, jump, dash, shield, changeWeaponRight, changeWeaponLeft, useHealthPack, currentWeaponAmmoAdd;

    // UI Actions from Input SystemAsset
    private InputAction pause;
    private InputAction inventory;



    // Start is called before the first frame update
    void Start()
    {
        AttackBuffer = 0; ShootBuffer = 0; JumpBuffer = 0; DashBuffer = 0;

        dataManager = DataManager.Instance;
        playerSkills = new PlayerSkills();
        LoadPlayerSkills();

        playerController = FindObjectOfType<PlayerController>();
        playerHealth = playerController.gameObject.GetComponent<PlayerHealth>();
        playerShield = playerController.gameObject.GetComponentInChildren<PlayerShield>();
        playerPrimaryWeapon = playerController.GetComponentInChildren<PlayerPrimaryWeapon>();
        playerSecondaryWeapon = playerController.GetComponentInChildren<PlayerSecondaryWeapon>();
        doors = FindObjectsOfType<Door_Script>();
        dialogueTriggers = FindObjectsOfType<DialogueTrigger>();
        cameraToUse = FindObjectOfType<Camera>();
        playerSkills.UnlockAllSkills();

        //EventSystem.current.onSkillUnlock += UnlockSkill;
    }

    private void OnEnable()
    {
        // Make a reference to the version of the InputActionAsset
        playerInput = FindObjectOfType<PlayerInput>();
        playerControls = new PlayerControls();

        // Get References
        playerActions = playerControls.Player;
        move = playerActions.Move;
        look = playerActions.Look;
        interact = playerActions.Interact;
        attack = playerActions.PrimaryWeapon;
        fire = playerActions.SecondaryWeapon;
        jump = playerActions.Jump;
        shield = playerActions.Shield;
        dash = playerActions.Dash;
        changeWeaponRight = playerActions.ChangeSecondaryRight;
        changeWeaponLeft = playerActions.ChangeSecondaryLeft;
        currentWeaponAmmoAdd = playerActions.AmmoAdd;
        useHealthPack = playerActions.UseHealthPack;
        pause = playerControls.UI.Pause;
        inventory = playerControls.UI.Inventory;

        // Enable moves and subscribe to any additional button events necessary
        move.Enable();
        look.Enable();
        interact.Enable();

        // Both attack and fire have more complex button patterns e.g. "Slow Tap" (used for mechanics that require hold); hence, the need to subscribe to events
        // Reference for when events are fired, based on which interaction they are: https://docs.unity.cn/Packages/com.unity.inputsystem@1.3/manual/Interactions.html
        // We are using the 'Slow Tap' interaction to see when the player has started holding a key 'started' and released it 'performed'
        // Otherwise, default button and value interactions are used

        attack.Enable();
        attack.performed += ctx => { CheckAttackInputEvents(ctx, "Performed"); };
        attack.started += ctx => { CheckAttackInputEvents(ctx, "Started"); };
        attack.canceled += ctx => { CheckAttackInputEvents(ctx, "Canceled"); };

        fire.Enable();
        fire.performed += ctx => { CheckFireInputEvents(ctx, "Performed"); };
        fire.started += ctx => { CheckFireInputEvents(ctx, "Started"); };
        fire.canceled += ctx => { CheckFireInputEvents(ctx, "Canceled"); };

        jump.Enable(); 
        shield.Enable();
        dash.Enable();
        changeWeaponRight.Enable();
        changeWeaponLeft.Enable();
        useHealthPack.Enable();
        currentWeaponAmmoAdd.Enable();
        pause.Enable();
        inventory.Enable();
    }

    private void OnDisable() 
    {
        move.Disable();
        look.Disable();
        interact.Disable();

        attack.Disable();
        attack.performed -= ctx => { CheckAttackInputEvents(ctx, "Performed"); };
        attack.started -= ctx => { CheckAttackInputEvents(ctx, "Started"); };
        attack.canceled -= ctx => { CheckAttackInputEvents(ctx, "Canceled"); };

        fire.Disable();
        fire.performed -= ctx => { CheckFireInputEvents(ctx, "Performed"); };
        fire.started -= ctx => { CheckFireInputEvents(ctx, "Started"); };
        fire.canceled -= ctx => { CheckFireInputEvents(ctx, "Canceled"); };
        
        jump.Disable(); 
        shield.Disable();
        dash.Disable();
        changeWeaponRight.Disable();
        changeWeaponLeft.Disable();
        useHealthPack.Disable();
        currentWeaponAmmoAdd.Disable();
        pause.Disable();
        inventory.Disable();
    }

    // Update is called once per frame
    void Update() { CheckInput(); }

    private void UnlockSkill(PlayerSkills.SkillType skill) { playerSkills.UnlockSkill(skill); }

    private void SavePlayerSkills() 
    {
        for (int i = 0; i < playerSkills.unlockedSkillsList.Count; i++)
        {
            if (!dataManager.sessionData.playerSkills.unlockedSkillsList.Contains(playerSkills.unlockedSkillsList[i]))
                { dataManager.sessionData.playerSkills.unlockedSkillsList.Add(playerSkills.unlockedSkillsList[i]); }
        }
    }

    private void LoadPlayerSkills() 
    {
        for (int i = 0; i < dataManager.sessionData.playerSkills.unlockedSkillsList.Count; i++)
        {
            playerSkills.unlockedSkillsList.Add(dataManager.sessionData.playerSkills.unlockedSkillsList[i]);
        }
    }

    private void FixedUpdate()
    {
        playerController.CheckGround();
        playerController.CheckWall();
        CalculateInputs();
        if (!playerController.isDashing) { playerController.ApplyMovement(); }
    }

    public int GetHP() { return playerHealth.HP; }

    public int GetMP() { return playerController.MP; }
    public float GetSP() { return playerController.SP; }

    void GetMovementAndAnalogInputs()
    {
        xInput = move.ReadValue<Vector2>().x;
        yInput = move.ReadValue<Vector2>().y;
        lookInput = look.ReadValue<Vector2>(); // used by Gamepad
        lookInputScreen = cameraToUse.WorldToScreenPoint(lookInput); 
        lookInputWorld = cameraToUse.ScreenToWorldPoint(lookInput);
        playerPositionScreen = cameraToUse.WorldToScreenPoint(playerController.transform.position);
        playerPositionWorld = cameraToUse.ScreenToWorldPoint(playerController.transform.position);
        playerPosition = playerController.transform.position;
    }


    void CheckAttackInputEvents(InputAction.CallbackContext ctx, string check) // NOTE: THESE ARE EVENTS AND FIRE AS INITIALIZED IN OnEnable(), THEREFORE THEY ARE NOT IN CheckInput()
    {
        if ("Started" == check) { if (ctx.interaction is SlowTapInteraction) { AttackButtonHeld = true; } else { AttackButtonDown = true; } }
        if ("Performed" == check) { if (ctx.interaction is SlowTapInteraction) { AttackButtonHeld = false; AttackButtonRelease = true; } }// weapon has fired after hold }
        if ("Cancelled" == check) { AttackButtonHeld = false; AttackButtonRelease = true; }
    }

    void CheckFireInputEvents(InputAction.CallbackContext ctx, string check) // NOTE: THESE ARE EVENTS AND FIRE AS INITIALIZED IN OnEnable(), THEREFORE THEY ARE NOT IN CheckInput()
    {
        if ("Started" == check) { if (ctx.interaction is SlowTapInteraction) { ShootButtonHeld = true; } ShootButtonDown = true; }
        if ("Performed" == check) { if (ctx.interaction is SlowTapInteraction) { ShootButtonHeld = false; ShootButtonRelease = true; } }// weapon has fired after hold }
        if ("Cancelled" == check)  { ShootButtonHeld = false; ShootButtonRelease = true; }
    }

    public void CheckInput() // CALLED IN UPDATE TO GET INPUTS AND UPDATE BUTTONS FOR CalcInputs()
    {
        if (pause.triggered && gameState != "Starting")
        {
            FindObjectOfType<PauseMenuHandler_Mason>().TogglePauseUI();
            pauseHandler();
        }
        if (!isPaused)
        {
            GetMovementAndAnalogInputs();

            if (interact.triggered) { InteractButton = true; }

            if (jump.triggered) { JumpButton = true; }
            if (jump.WasReleasedThisFrame()) { JumpButton = false; JumpBuffer = 0; }

            if (attack.WasReleasedThisFrame()) // if attack is released
            {
                AttackButtonDown = false;
                AttackButtonRelease = true;
                AttackBuffer = 0;
            }

            if (dash.triggered) { DashButton = true; }
            if (dash.WasReleasedThisFrame()) // if dash is released
            {
                DashButton = false;
                DashBuffer = 0;
            }


            if (jump.triggered && hasJump()) { playerController.Jump(); }

            if (fire.WasReleasedThisFrame()) { EventSystem.current.WeaponStopTrigger(); }

            if (hasBlock())
            {
                if (shield.triggered) { playerShield.ShieldButtonDown(); }
                if (shield.ReadValue<float>() > 0f) { playerShield.ShieldButtonHeld(); }
                if (shield.WasReleasedThisFrame()) { playerShield.ShieldButtonUp(); }
            }

            // NOTE: Keyboard only commands are listed as they are primarily testing related; they don't exist in the controller input map

            // For Spawns
            if (Keyboard.current.digit1Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(0); };
            if (Keyboard.current.digit2Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(1); };
            if (Keyboard.current.digit3Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(2); };
            if (Keyboard.current.digit4Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(3); };
            if (Keyboard.current.digit5Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(4); };
            if (Keyboard.current.digit6Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(5); };

            // For Inventory
            if (currentWeaponAmmoAdd.triggered) { EventSystem.current.WeaponAddAmmoTrigger(100); }
            if (changeWeaponLeft.triggered) { EventSystem.current.WeaponChangeTrigger(-1); }
            if (changeWeaponRight.triggered) { EventSystem.current.WeaponChangeTrigger(1); }
            if (useHealthPack.triggered) { FindObjectOfType<PlayerData_UI_Mason>().UseHealthPack(); } // To-Do: move this function to a more apt script vs in a UI script

            // For Inventory
            if(inventory.triggered) { FindObjectOfType<Inventory_UI_Mason>().ToggleUI(); }

            // Debug Console
            if (Keyboard.current.backquoteKey.wasPressedThisFrame) { FindObjectOfType<DebugController_Mason>().ToggleDebugConsole(); }
            if (Keyboard.current.enterKey.wasPressedThisFrame) { FindObjectOfType<DebugController_Mason>().EnterInput(); }
            

        }
    }
    public void CalculateInputs()
    {
        if (!isPaused)
        {
            CameraRelatedLogic();
            InteractLogic();
            JumpLogic();
            DashLogic();
            MeleeLogic();
            ShootLogic();
            MomentumLogic();
            FlipLogic();
        }
    }

    void CameraRelatedLogic()
    {
        // update functions that need camera data
        playerSecondaryWeapon.HandleWeaponDirection(playerInput.currentControlScheme);
        CameraBehavior.AdjustCameraDown();
    }

    void InteractLogic() 
    { 
        if (InteractButton) 
        {
            InteractButton = false;
            foreach (Door_Script door in doors) { if(door.playerDetected == true) { door.InteractWithPlayer(); } }
            foreach (DialogueTrigger dialogue in dialogueTriggers) { dialogue.PlayerInitiatedDialogue(); }
        } 
    }

    void JumpLogic() { if (JumpButton && !(JumpBuffer > 5) && hasJump()) { playerController.Jump(); ++JumpBuffer; } }

    void DashLogic() { if (DashButton && !(DashBuffer > 5) && hasDash()) { playerController.Dash(); ++DashBuffer; } }

    void MeleeLogic()
    {
        if (AttackButtonDown && !(AttackBuffer > 5))
        {
            if (yInput > 0.2f) { playerPrimaryWeapon.Attack(0); }
            else if (yInput < -0.2f) { playerPrimaryWeapon.Attack(1); }
            else { playerPrimaryWeapon.Attack(2); }
            ++AttackBuffer;
            AttackButtonDown = false;
        }
        if (AttackButtonHeld)
        {
            if (yInput > 0.2f) { playerPrimaryWeapon.Attack(0); }
            else if (yInput < -0.2f) { playerPrimaryWeapon.Attack(1); }
            else { playerPrimaryWeapon.Attack(2); }
        }
        if(AttackButtonRelease)
        {
            if (yInput > 0.2f) { playerPrimaryWeapon.Release(0); }
            else if (yInput < -0.2f) { playerPrimaryWeapon.Release(1); }
            else { playerPrimaryWeapon.Release(2); }
            AttackButtonRelease = false;
            AttackButtonHeld = false;
        }
    }

    void ShootLogic()
    {
        if (ShootButtonDown) 
        { 
            EventSystem.current.AmmoCheckTrigger();
            playerSecondaryWeapon.HandleThrowing("Button Clicked", playerInput.currentControlScheme);
            ShootButtonDown = false;
        }
        if (ShootButtonHeld)
        {
            EventSystem.current.AmmoCheckTrigger();
            playerSecondaryWeapon.HandleThrowing("Button Held", playerInput.currentControlScheme);
        }
        else if (ShootButtonRelease)
        {
            playerSecondaryWeapon.HandleThrowing("Button Released", playerInput.currentControlScheme);
            EventSystem.current.WeaponStopTrigger();
            ShootButtonRelease = false;
            ShootButtonHeld = false;
        }
    }

    void MomentumLogic()
    {
        if (xInput > 0 && playerController.ControlMomentum < 10)
        {
            if (playerController.ControlMomentum == 0) { playerController.ControlMomentum = 5; }
            else { playerController.ControlMomentum += 1; }
        }

        else if (xInput < 0 && playerController.ControlMomentum > -10)
        {
            if (playerController.ControlMomentum == 0) { playerController.ControlMomentum = -5; }
            else { playerController.ControlMomentum -= 1; }
        }
        else if (xInput == 0)
        {
            if (playerController.ControlMomentum > 0) { playerController.ControlMomentum -= 1; }
            else if (playerController.ControlMomentum < 0) { playerController.ControlMomentum += 1; }
        }

        if (playerController.ControlMomentum > 10) { playerController.ControlMomentum -= 1; }
        else if (xInput < 0 && playerController.ControlMomentum < -10) { playerController.ControlMomentum += 1; }
    }

    void FlipLogic()
    {
        if(playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            if (xInput >= 1 && playerController.facingDirection == -1) { HandleFlipping(); }
            else if (xInput <= -1 && playerController.facingDirection == 1) { HandleFlipping(); }
            else if (playerSecondaryWeapon.WeaponIsPointedToTheRight() && playerController.facingDirection == -1) { if (xInput >= 0) { HandleFlipping(); } }
            else if (!playerSecondaryWeapon.WeaponIsPointedToTheRight() && playerController.facingDirection == 1) { if (xInput <= 0) HandleFlipping(); }
        }

        else if (playerInput.currentControlScheme == "Gamepad")
        {
            if ((xInput >= 0.2 && xInput > 0) && playerController.facingDirection == -1) { HandleFlipping(); } // slight moving right and facing left? Flip
            else if ((xInput <= -0.2 && xInput < 0) && playerController.facingDirection == 1) { HandleFlipping(); } // slight moving left and facing right? ...
            else if (xInput == 0) // standing still
            {
                if (lookInput.x > 0 && playerController.facingDirection == -1) { HandleFlipping(); } // looking right, but facing left? ...
                else if (lookInput.x < 0 && playerController.facingDirection == 1) { HandleFlipping(); } // looking left, but facing right? ...
            }
        }
    }


    public void pauseHandler()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            if (gameState == "starting") { pauseHelper = true; }
            else { pauseHelper = false; }
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
            if (pauseHelper) { gameState = "starting"; }
            else { gameState = "running"; }
        }

    }

    public void HandleFlipping()
    {
        playerController.Flip();
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
