//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class GameController : MonoBehaviour
{
    // REFERENCES TO IN-SCENE OBJECTS
    // Player Related
    [SerializeField] private PlayerController playerController;
    private PlayerHealth playerHealth;
    private PlayerShield playerShield;
    private PlayerDash playerDash;
    private PlayerJump playerJump;
    [SerializeField] private PlayerSkillsManager skills;
    [SerializeField] private PlayerPrimaryWeapon playerPrimaryWeapon;
    [SerializeField] private PlayerSecondaryWeapon playerSecondaryWeapon;
    [SerializeField] private UIController UIController;

    // Interactable Objects In Scene
    private DialogueManager dialogueManager;
    private Door_Script[] doors;
    private DialogueTrigger[] dialogueTriggers;

    // Other
    [SerializeField] private CameraBehavior CameraBehavior;
    [SerializeField] private EnemyCreationForTesting enemySpawner;
    public Camera cameraToUse; 

    // INTERNAL TO GAMECONTROLLER
    // Trackers for states
    public string gameState; public bool isPaused;
    [SerializeField] private bool _isCutscene; public bool IsCutscene { get { return _isCutscene; } set { _isCutscene = value; } }

    private bool pauseHelper;

    // button state trackers: simple
    public bool InteractButton, JumpButton, DashButton, ShieldButton;
    private bool pauseButtonDown;

    // button state trackers, more complex due to hold interaction
    public bool AttackButtonDown, AttackButtonHeld, AttackButtonRelease, ShootButtonDown, ShootButtonHeld, ShootButtonRelease;

    // buffers between button presses
    public int AttackBuffer, ShootBuffer, JumpBuffer, DashBuffer;

    // Stores analog information for other functions; uses properties (vs fields) to make sure to know where values are changed or pullede.g. 'references'
    [SerializeField] private float _xInput; public float XInput { get { return _xInput; } set { _xInput = value; } }
    [SerializeField] private float _yInput;  public float YInput { get { return _yInput; } set { _yInput = value; } }
    public Vector2 lookInput, lookInputScreen, lookInputWorld; // raw unprocessed input, relative to screen position, and relative to world position
    public Vector2 playerPosition, playerPositionScreen, playerPositionWorld; // relative differences are used to get rotations via vectors

    // New Input System Asset
    public PlayerInput playerInput; public PlayerControls playerControls;

    // Gameplay Actions from InputSystemAsset
    private PlayerControls.PlayerActions playerActions;
    private InputAction move, look, interact, attack, fire, jump, dash, shield, changeWeaponRight, changeWeaponLeft, useHealthPack, currentWeaponAmmoAdd;

    // UI Actions from Input SystemAsset
    private InputAction pause, inventory, submit;

    void Start() { AttackBuffer = 0; ShootBuffer = 0; JumpBuffer = 0; DashBuffer = 0; AddReferences(); }

    void AddReferences()
    {
        
        playerController = FindObjectOfType<PlayerController>();
        playerHealth = playerController.gameObject.GetComponent<PlayerHealth>();
        playerShield = playerController.gameObject.GetComponentInChildren<PlayerShield>();
        playerDash = playerController.gameObject.GetComponentInChildren<PlayerDash>();
        playerJump = playerController.gameObject.GetComponentInChildren<PlayerJump>();
        playerPrimaryWeapon = playerController.GetComponentInChildren<PlayerPrimaryWeapon>();
        playerSecondaryWeapon = playerController.GetComponentInChildren<PlayerSecondaryWeapon>();
        skills = playerController.GetComponentInChildren<PlayerSkillsManager> ();
        dialogueManager = FindObjectOfType<DialogueManager>();  
        doors = FindObjectsOfType<Door_Script>();
        dialogueTriggers = FindObjectsOfType<DialogueTrigger>();
        cameraToUse = FindObjectOfType<Camera>();
    }

    private void OnEnable()
    {
        // Make a reference to the version of the InputActionAsset
        playerInput = FindObjectOfType<PlayerInput>(); playerControls = new PlayerControls();

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
        submit = playerControls.UI.Submit;

        // Enable moves and subscribe to any additional button events necessary
        // less complex gameplay buttons
        move.Enable(); look.Enable(); interact.Enable(); jump.Enable(); shield.Enable(); dash.Enable(); changeWeaponRight.Enable(); changeWeaponLeft.Enable(); useHealthPack.Enable(); currentWeaponAmmoAdd.Enable();

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

        // UI Actions
        pause.Enable(); inventory.Enable(); submit.Enable();
    }

    private void OnDisable() 
    {
        // less complex gameplay buttons
        move.Disable(); look.Disable(); interact.Disable(); jump.Disable(); shield.Disable(); dash.Disable(); changeWeaponRight.Disable(); changeWeaponLeft.Disable(); useHealthPack.Disable(); currentWeaponAmmoAdd.Disable();

        // more complex gameplay buttons
        attack.Disable();
        attack.performed -= ctx => { CheckAttackInputEvents(ctx, "Performed"); };
        attack.started -= ctx => { CheckAttackInputEvents(ctx, "Started"); };
        attack.canceled -= ctx => { CheckAttackInputEvents(ctx, "Canceled"); };

        fire.Disable();
        fire.performed -= ctx => { CheckFireInputEvents(ctx, "Performed"); };
        fire.started -= ctx => { CheckFireInputEvents(ctx, "Started"); };
        fire.canceled -= ctx => { CheckFireInputEvents(ctx, "Canceled"); };
        
        // ui buttons
        pause.Disable(); inventory.Disable(); submit.Disable();
    }

    // Update is called once per frame
    void Update() { CheckInput(); }

    private void FixedUpdate()
    {
        playerController.CheckGround();
        playerController.CheckWall();
        if (!playerDash.IsDashing) { playerController.ApplyMovement(); }
        CalculateInputs();
    }

    void GetMovementAndAnalogInputs()
    {
        XInput = move.ReadValue<Vector2>().x;
        YInput = move.ReadValue<Vector2>().y;
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

    public void CheckInput() // CALLED IN UPDATE TO GET INPUTS AND UPDATE BUTTON VARIABLES FOR CalcInputs()
    {
        if (pause.triggered && gameState != "Starting")
        {
            FindObjectOfType<PauseMenuHandler_Mason>().TogglePauseUI();
            pauseHandler();
        }
        else if (dialogueManager.DialogueIsPlaying) 
        {
            if(submit.triggered){ dialogueManager.ContinueStory(); }
        }
        else if (IsCutscene) { }  // let only UI actions pass through, if paused or in cutscene
        else if (!isPaused && !IsCutscene) // otherwise, get input and updated buttons i.e. play game
        {
            GetMovementAndAnalogInputs();

            if (interact.triggered) { InteractButton = true; }

            if (jump.triggered) { JumpButton = true; }
            if (jump.WasReleasedThisFrame()) { JumpButton = false; JumpBuffer = 0; }

            if (attack.WasReleasedThisFrame()) { AttackButtonDown = false; AttackButtonRelease = true; AttackBuffer = 0; }

            if (dash.triggered) { DashButton = true; }
            if (dash.WasReleasedThisFrame()) { DashButton = false; DashBuffer = 0; }

            if (jump.triggered && skills.hasJump()) { JumpButton = true; }

            if (fire.WasReleasedThisFrame()) { EventSystem.current.WeaponStopTrigger(); }

            if (skills.hasBlock())
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
            if (inventory.triggered) { FindObjectOfType<Inventory_UI_Mason>().ToggleUI(); }

            // Debug Console
            if (Keyboard.current.backquoteKey.wasPressedThisFrame) { FindObjectOfType<DebugController_Mason>().ToggleDebugConsole(); }
            if (Keyboard.current.enterKey.wasPressedThisFrame) { FindObjectOfType<DebugController_Mason>().EnterInput(); }
        }
    }
    public void CalculateInputs() // called in FixedUpdate and mostly executes based on button inputs (some executions are handled in CheckInput() )
    {
        if (!isPaused && !IsCutscene)
        {
            CameraRelatedLogic(); InteractLogic(); // camera and interactions
            JumpLogic(); DashLogic(); MomentumLogic(); FlipLogic(); // non-attack player actions
            MeleeLogic(); ShootLogic(); // player attacks
        }
        else if (IsCutscene)
        {
            MomentumLogic(); FlipLogic(); InteractLogic(); // you still want certain physics based interactions to flow through (e.g. cutscene movement)
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

    void JumpLogic() { if (JumpButton && !(JumpBuffer > 5) && skills.hasJump()) { playerJump.Execute(); ++JumpBuffer; } }

    void DashLogic() { if (DashButton && !(DashBuffer > 5) && skills.hasDash()) { playerDash.Execute(); ++DashBuffer; } }

    void MeleeLogic()
    {
        if (AttackButtonDown && !(AttackBuffer > 5))
        {
            if (YInput > 0.2f) { playerPrimaryWeapon.Attack(0); }
            else if (YInput < -0.2f) { playerPrimaryWeapon.Attack(1); }
            else { playerPrimaryWeapon.Attack(2); }
            ++AttackBuffer;
            AttackButtonDown = false;
        }
        if (AttackButtonHeld)
        {
            if (YInput > 0.2f) { playerPrimaryWeapon.Attack(0); }
            else if (YInput < -0.2f) { playerPrimaryWeapon.Attack(1); }
            else { playerPrimaryWeapon.Attack(2); }
        }
        if (AttackButtonRelease)
        {
            if (YInput > 0.2f) { playerPrimaryWeapon.Release(0); }
            else if (YInput < -0.2f) { playerPrimaryWeapon.Release(1); }
            else { playerPrimaryWeapon.Release(2); }
            AttackButtonRelease = false; AttackButtonHeld = false;
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
            ShootButtonRelease = false; ShootButtonHeld = false;
        }
    }

    void MomentumLogic() { playerController.UpdateMomentum(); }
    void FlipLogic()
    {
        if(playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            if (XInput >= 1 && playerController.FacingDirection == -1) { HandleFlipping(); }
            else if (XInput <= -1 && playerController.FacingDirection == 1) { HandleFlipping(); }
            else if (playerSecondaryWeapon.WeaponIsPointedToTheRight() && playerController.FacingDirection == -1) { if (XInput >= 0) { HandleFlipping(); } }
            else if (!playerSecondaryWeapon.WeaponIsPointedToTheRight() && playerController.FacingDirection == 1) { if (XInput <= 0) HandleFlipping(); }
        }

        else if (playerInput.currentControlScheme == "Gamepad")
        {
            if ((XInput >= 0.2 && XInput > 0) && playerController.FacingDirection == -1) { HandleFlipping(); } // slight moving right and facing left? Flip
            else if ((XInput <= -0.2 && XInput < 0) && playerController.FacingDirection == 1) { HandleFlipping(); } // slight moving left and facing right? ...
            else if (XInput == 0) // standing still
            {
                if (lookInput.x > 0 && playerController.FacingDirection == -1) { HandleFlipping(); } // looking right, but facing left? ...
                else if (lookInput.x < 0 && playerController.FacingDirection == 1) { HandleFlipping(); } // looking left, but facing right? ...
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

    public void ResetPlayerMotionAndInput() { XInput = 0; YInput = 0; playerController.IdlePlayer(true); }
    public bool PlayerInputIdle() { if (XInput != 0 && YInput != 0) { return false; } else { return true; } }

    public void TriggerInteractButton() { InteractButton = true; } // used by cutscenes 

    public void HandleFlipping() { playerController.Flip(); playerSecondaryWeapon.Flip(); }
}
