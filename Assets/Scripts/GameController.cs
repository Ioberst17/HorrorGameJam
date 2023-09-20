//using UnityEditor.Experimental.GraphView;
using Ink.Runtime;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameController : MonoBehaviour
{
    // REFERENCES TO IN-SCENE OBJECTS
    // Player Related
    private SecondaryWeaponsManager secondaryWeaponsManager;
    private PlayerController playerController;
    private PlayerShield playerShield;
    private PlayerDash playerDash;
    private PlayerJump playerJump;
    private PlayerSkillsManager skills;
    private PlayerAttackManager playerPrimaryWeapon;
    private PlayerSecondaryWeapon playerSecondaryWeapon;
    private Inventory_UI_Mason inventoryUI;
    private PauseMenuHandler_Mason pauseMenu;

    // Interactable Objects In Scene
    private DialogueManager dialogueManager;
    private Door_Script[] doors;
    private DialogueTrigger[] dialogueTriggers;

    // Other
    private CameraBehavior CameraBehavior;
    private EnemyCreationForTesting enemySpawner;
    private Camera cameraToUse;

    public enum ButtonState { Pressed, Held, Released}
    
    // GAMECONTROLLER VARIABLES
    // Trackers for states
    [SerializeField] private string _gameState;  public string GameState { get { return _gameState; } set { _gameState = value; } }
    [SerializeField] private bool _isPaused;  public bool IsPaused { get { return _isPaused; } set { _isPaused = value; } }
    [SerializeField] private bool _isCutscene; public bool IsCutscene { get { return _isCutscene; } set { _isCutscene = value; } }

    private bool pauseHelper;

    // button state
    private bool _playerIsInputting; public bool PlayerIsInputting { get { return _playerIsInputting; } set { _playerIsInputting = value; } }
    public float timeSinceInput;

    // button state trackers: simple
    private bool InteractButton, JumpButton, DashButton, ShieldButton;
    private bool pauseButtonDown, submitButtonDown, cancelButtonDown;

    // button state trackers, more complex due to hold interaction
    [SerializeField] private bool AttackButtonDown, AttackButtonHeld, AttackButtonRelease, ShootButtonDown, ShootButtonHeld, ShootButtonRelease;

    // buffers between button presses
    private int AttackBuffer, JumpBuffer, DashBuffer;

    // Stores analog information for other functions; uses properties (vs fields) to make sure to know where values are changed or pullede.g. 'references'
    [SerializeField] private float _xInput; public float XInput { get { return _xInput; } set { _xInput = value; } }
    [SerializeField] private float _yInput;  public float YInput { get { return _yInput; } set { _yInput = value; } }
    public Vector2 lookInput, lookInputScreen, lookInputWorld; // raw unprocessed input, relative to screen position, and relative to world position
    public Vector2 playerPosition, playerPositionScreen, playerPositionWorld; // relative differences are used to get rotations via vectors

    // New Input System Asset
    public PlayerInput PlayerInput { get; set; }
    public InputActionAsset playerControls;
    private bool onlyUIActionsToggle;
    [SerializeField] private string _currentControlScheme; public string CurrentControlScheme { get { return _currentControlScheme; } set { _currentControlScheme = value; } }

    // Player 'Gameplay' Actions from InputSystemAsset
    private InputAction move, look, interact, attack, fire, jump, dash, shield, changeWeaponRight, changeWeaponLeft, healthKit, currentWeaponAmmoAdd;

    // UI Actions from InputSystemAsset
    private InputAction pause, inventory, submit, cancel;

    float directionalInputThreshold = 0.8f;

    void Start() { AttackBuffer = 0; JumpBuffer = 0; DashBuffer = 0; AddReferences(); }

    void AddReferences()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0) // if not title screen
        {
            secondaryWeaponsManager = FindObjectOfType<SecondaryWeaponsManager>();
            playerController = secondaryWeaponsManager.GetComponentInChildren<PlayerController>();
            playerShield = playerController.GetComponentInChildren<PlayerShield>();
            playerDash = playerController.GetComponentInChildren<PlayerDash>();
            playerJump = playerController.GetComponentInChildren<PlayerJump>();
            playerPrimaryWeapon = playerController.GetComponentInChildren<PlayerAttackManager>();
            playerSecondaryWeapon = playerController.GetComponentInChildren<PlayerSecondaryWeapon>();
            skills = playerController.GetComponentInChildren<PlayerSkillsManager>();
            inventoryUI = FindObjectOfType<Inventory_UI_Mason>();
            pauseMenu = FindObjectOfType<PauseMenuHandler_Mason>();
            dialogueManager = FindObjectOfType<DialogueManager>();
            doors = FindObjectsOfType<Door_Script>();
            enemySpawner = FindObjectOfType<EnemyCreationForTesting>();
            dialogueTriggers = FindObjectsOfType<DialogueTrigger>();
            cameraToUse = FindObjectOfType<Camera>();
            CameraBehavior = FindObjectOfType<CameraBehavior>();
        }
    }

    private void OnEnable()
    {
        // Make a reference to the version of the InputActionAsset
        PlayerInput = FindObjectOfType<PlayerInput>();
        playerControls = PlayerInput.actions;

        // Get References
        move = playerControls.FindAction("Move");
        look = playerControls.FindAction("Look");
        interact = playerControls.FindAction("Interact");
        attack = playerControls.FindAction("PrimaryWeapon");
        fire = playerControls.FindAction("SecondaryWeapon");
        jump = playerControls.FindAction("Jump");
        shield = playerControls.FindAction("Shield");
        dash = playerControls.FindAction("Dash");
        changeWeaponRight = playerControls.FindAction("ChangeSecondaryRight");
        changeWeaponLeft = playerControls.FindAction("ChangeSecondaryLeft");
        currentWeaponAmmoAdd = playerControls.FindAction("AmmoAdd");
        healthKit = playerControls.FindAction("HealthKit");
        pause = playerControls.FindAction("Pause");
        inventory = playerControls.FindAction("Inventory");
        submit = playerControls.FindAction("Submit");
        cancel = playerControls.FindAction("Cancel");

        // Enable moves and subscribe to any additional button events necessary
        // less complex gameplay buttons
        move.Enable(); look.Enable(); interact.Enable(); jump.Enable(); shield.Enable(); dash.Enable(); changeWeaponRight.Enable(); changeWeaponLeft.Enable(); healthKit.Enable(); currentWeaponAmmoAdd.Enable();

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
        pause.Enable(); inventory.Enable(); submit.Enable(); cancel.Enable();
    }

    private void OnDisable() 
    {
        // less complex gameplay buttons
        move.Disable(); look.Disable(); interact.Disable(); jump.Disable(); shield.Disable(); dash.Disable(); changeWeaponRight.Disable(); changeWeaponLeft.Disable(); healthKit.Disable(); currentWeaponAmmoAdd.Disable();

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
        pause.Disable(); inventory.Disable(); submit.Disable(); cancel.Disable();
    }

    // Update is called once per frame
    void Update() 
    {
        PlayerIsInputting = false;
        CurrentControlScheme = PlayerInput.currentControlScheme;
        CheckInput(); 
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) // only needed in non title screen as these are gameplay related actions
        {
            playerController.CheckGround();
            playerController.CheckWall();
            if (!playerDash.IsDashing) { playerController.ApplyMovement(); }
            CalculateInputs();
        }
    }

    void GetMovementAndAnalogInputs()
    {
        XInput = move.ReadValue<Vector2>().x;
        YInput = move.ReadValue<Vector2>().y;
        if(XInput != 0 || YInput != 0) { PlayerIsInputting = true; }
        lookInput = look.ReadValue<Vector2>(); // used by Gamepad
        lookInputScreen = cameraToUse.WorldToScreenPoint(lookInput); 
        lookInputWorld = cameraToUse.ScreenToWorldPoint(lookInput);
        playerPositionScreen = cameraToUse.WorldToScreenPoint(playerController.transform.position);
        playerPositionWorld = cameraToUse.ScreenToWorldPoint(playerController.transform.position);
        playerPosition = playerController.transform.position;
    }

    // NOTE: THESE ARE EVENTS AND FIRE AS INITIALIZED IN OnEnable(), THEREFORE THEY ARE NOT IN CheckInput()
    void CheckAttackInputEvents(InputAction.CallbackContext ctx, string check) 
    {
        if ("Started" == check)
        {
            if (ctx.interaction is TapInteraction) { AttackButtonDown = true; Debug.Log(ctx.interaction + " - Started"); PlayerIsInputting = true; }
            if (ctx.interaction is HoldInteraction) { Debug.Log(ctx.interaction + " - Started"); PlayerIsInputting = true; }
        }
        if ("Performed" == check)
        {
            if (ctx.interaction is TapInteraction) { Debug.Log(ctx.interaction + " - Performed"); }
            if (ctx.interaction is HoldInteraction) { AttackButtonHeld = true; Debug.Log(ctx.interaction + " - Performed"); PlayerIsInputting = true; }
        }
        if ("Cancelled" == check) 
        {
            if (ctx.interaction is TapInteraction) { AttackButtonHeld = true; Debug.Log(ctx.interaction + " - Cancelled"); PlayerIsInputting = true; }
            if (ctx.interaction is HoldInteraction) { AttackButtonHeld = true; Debug.Log(ctx.interaction + " - Cancelled"); PlayerIsInputting = true; }
        }
    }

    void CheckFireInputEvents(InputAction.CallbackContext ctx, string check) // NOTE: THESE ARE EVENTS AND FIRE AS INITIALIZED IN OnEnable(), THEREFORE THEY ARE NOT IN CheckInput()
    {
        if ("Started" == check) { if (ctx.interaction is SlowTapInteraction) { ShootButtonHeld = true; } ShootButtonDown = true; PlayerIsInputting = true; }
        if ("Performed" == check) { if (ctx.interaction is SlowTapInteraction) { ShootButtonHeld = false; ShootButtonRelease = true; } PlayerIsInputting = true; }// weapon has fired after hold }
        if ("Cancelled" == check)  { ShootButtonHeld = false; ShootButtonRelease = true; PlayerIsInputting = true; }
    }

    public void CheckInput() // CALLED IN UPDATE TO GET INPUTS AND UPDATE BUTTON VARIABLES FOR CalcInputs()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) // if not title screen
        { 
            PauseTriggerCheck(); // pause will always be checked, regardless of condition
            if (dialogueManager.DialogueIsPlaying)
            {
                LimitToOnlyUIActions(true);
                UITriggerChecks();
                if (submitButtonDown) { dialogueManager.ContinueStory(); }
                if (cancelButtonDown) { dialogueManager.ExitDialogueMode(); }
            }
            else if (IsCutscene) // if is cutscene
            {
                LimitToOnlyUIActions(true);
                // allow only pause actions, no inventory or cancel / if in dialogue while in cutscene it is handled in previous branch
            }
            else if (IsPaused && pauseMenu.pauseMenu.activeSelf) // navigation and submits are handled stock by Unity's UI Input System
            {
                LimitToOnlyUIActions(true);
            }
            else if (IsPaused && pauseMenu.controlsMenu.activeSelf)
            {
                LimitToOnlyUIActions(true);
            }
            else if (IsPaused && inventoryUI.InventoryOpen)
            {
                LimitToOnlyUIActions(true);
                // allow UI actions and inventory
                InventoryTriggerCheck();
            }
            else if (!IsPaused && !IsCutscene) // else let standard gameplay inputs play
            {
                LimitToOnlyUIActions(false);
                GetMovementAndAnalogInputs();

                if (interact.triggered) { InteractButton = true; PlayerIsInputting = true; }

                if (jump.triggered) { JumpButton = true; PlayerIsInputting = true; }
                if (jump.WasReleasedThisFrame()) { JumpButton = false; JumpBuffer = 0; }

                if (attack.WasReleasedThisFrame()) { /*AttackButtonDown = false;*/ AttackButtonRelease = true; AttackBuffer = 0; PlayerIsInputting = true; }

                if (dash.triggered) { DashButton = true; PlayerIsInputting = true; }
                if (dash.WasReleasedThisFrame()) { DashButton = false; DashBuffer = 0; }

                if (jump.triggered && skills.hasJump()) { JumpButton = true; PlayerIsInputting = true; }

                if (fire.WasReleasedThisFrame()) { EventSystem.current.WeaponStopTrigger(); PlayerIsInputting = true; }

                if (skills.hasBlock())
                {
                    if (shield.triggered) { playerShield.ShieldButtonDown(); PlayerIsInputting = true; }
                    if (shield.ReadValue<float>() > 0f) { playerShield.ShieldButtonHeld(); PlayerIsInputting = true; }
                    if (shield.WasReleasedThisFrame()) { playerShield.ShieldButtonUp(); PlayerIsInputting = true; }
                }

                // NOTE: Keyboard only commands are listed as they are primarily testing related; they don't exist in the controller input map

                // For Spawns
                if (Keyboard.current.digit1Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(0); };
                if (Keyboard.current.digit2Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(1); };
                if (Keyboard.current.digit3Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(2); };
                if (Keyboard.current.digit4Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(3); };
                if (Keyboard.current.digit5Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(4); };
                if (Keyboard.current.digit6Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(5); };
                if (Keyboard.current.digit7Key.wasPressedThisFrame) { enemySpawner.SpawnEnemy(6); };

                // For Inventory
                InventoryTriggerCheck();
                if (currentWeaponAmmoAdd.triggered) { EventSystem.current.WeaponAddAmmoTrigger(100); }
                if (changeWeaponLeft.triggered) { EventSystem.current.WeaponChangeTrigger(-1); PlayerIsInputting = true; }
                if (changeWeaponRight.triggered) { EventSystem.current.WeaponChangeTrigger(1); PlayerIsInputting = true; }
                if (healthKit.triggered) { FindObjectOfType<PlayerData_UI_Mason>().UseHealthPack(); PlayerIsInputting = true; } // To-Do: move this function to a more apt script vs in a UI script

                // Debug Console
                if (Keyboard.current.backquoteKey.wasPressedThisFrame) { FindObjectOfType<DebugController_Mason>().ToggleDebugConsole(); }
                if (Keyboard.current.enterKey.wasPressedThisFrame) { FindObjectOfType<DebugController_Mason>().EnterInput(); }

                if (!PlayerIsInputting) { timeSinceInput += Time.deltaTime; }
                else if (PlayerIsInputting) { timeSinceInput = 0; }
            }
        }
    }

    // BLOCKS OF INPUTS FOR CHECKINPUT
    void PauseTriggerCheck() { if (pause.triggered && GameState != "starting") { PauseHandler(); } }

    // CHECKS NONPAUSE BUTTONS
    private void UITriggerChecks()
    {
        SubmitCheck();
        CancelCheck();
    }

    void CancelCheck()
    {
        if (cancel.triggered) { cancelButtonDown = true; }
        if (cancel.WasReleasedThisFrame()) { cancelButtonDown = false; }
    }

    void SubmitCheck()
    {
        if (submit.triggered) { submitButtonDown = true; }
        if (submit.WasReleasedThisFrame()) { submitButtonDown = false; }
    }

    // INVENTORY TRIGGER CHECK
    private void InventoryTriggerCheck() { if (inventory.WasPressedThisFrame()) { PauseHandler("Inventory"); } } // let inventory toggle

    public void CalculateInputs() // called in FixedUpdate and mostly executes based on button inputs (some executions are handled in CheckInput() )
    {
        if (!IsPaused && !IsCutscene)
        {
            CameraRelatedLogic(); InteractLogic(); // camera and interactions
            JumpLogic(); DashLogic(); MomentumLogic(); FlipLogic(); // non-attack player actions
            MeleeLogic(); ShootLogic(); // player attacks
        }
        else if (DialogueManager.GetInstance().DialogueIsPlaying)
        {

        }
        else if (IsCutscene)
        {
            MomentumLogic(); FlipLogic(); InteractLogic(); // you still want certain physics based interactions to flow through (e.g. cutscene movement)
        }
    }

    void CameraRelatedLogic()
    {
        // update functions that need camera data
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

    void JumpLogic() { if (JumpButton && !(JumpBuffer > 5) && skills.hasJump()) { playerJump.Execute(); JumpBuffer = 6; } }

    void DashLogic() { if (DashButton && !(DashBuffer > 5) && skills.hasDash()) { playerDash.Execute(); ++DashBuffer; } }

    void MeleeLogic()
    {
        if (AttackButtonDown && !(AttackBuffer > 5))
        {
            if (YInput > directionalInputThreshold) { playerPrimaryWeapon.Attack(0, ButtonState.Pressed); }
            else if (YInput < -directionalInputThreshold) { playerPrimaryWeapon.Attack(1, ButtonState.Pressed); }
            else { playerPrimaryWeapon.Attack(2, ButtonState.Pressed); }
            ++AttackBuffer;
            AttackButtonDown = false;
        }
        if (AttackButtonHeld)
        {
            if (YInput > directionalInputThreshold) { playerPrimaryWeapon.Attack(0, ButtonState.Held); }
            else if (YInput < -directionalInputThreshold) { playerPrimaryWeapon.Attack(1, ButtonState.Held); }
            else { playerPrimaryWeapon.Attack(2, ButtonState.Held); }
        }
        if (AttackButtonRelease)
        {
            if (YInput > directionalInputThreshold) { playerPrimaryWeapon.Release(0); }
            else if (YInput < -directionalInputThreshold) { playerPrimaryWeapon.Release(1); }
            else { playerPrimaryWeapon.Release(2); }
            AttackButtonRelease = false; AttackButtonHeld = false;
        }
    }

    void ShootLogic()
    {
        if (ShootButtonDown) 
        {
            secondaryWeaponsManager.CanWeaponBeFired(); // used for shooting, in SecondaryWeaponManager
            playerSecondaryWeapon.HandleThrowing("Button Clicked", CurrentControlScheme); // used for throwing
            ShootButtonDown = false;
        }
        if (ShootButtonHeld)
        {
            secondaryWeaponsManager.CanWeaponBeFired();
            playerSecondaryWeapon.HandleThrowing("Button Held", CurrentControlScheme);
        }
        else if (ShootButtonRelease)
        {
            playerSecondaryWeapon.HandleThrowing("Button Released", CurrentControlScheme);
            EventSystem.current.WeaponStopTrigger();
            ShootButtonRelease = false; ShootButtonHeld = false;
        }
    }

    void MomentumLogic() { playerController.UpdateMomentum(); }
    void FlipLogic()
    {
        if(CurrentControlScheme == "Keyboard and Mouse")
        {
            if (XInput >= 1 && playerController.FacingDirection == -1) { HandleFlipping(); }
            else if (XInput <= -1 && playerController.FacingDirection == 1) { HandleFlipping(); }
            else if (playerSecondaryWeapon.WeaponIsPointedToTheRight() && playerController.FacingDirection == -1) { if (XInput >= 0) { HandleFlipping(); } }
            else if (!playerSecondaryWeapon.WeaponIsPointedToTheRight() && playerController.FacingDirection == 1) { if (XInput <= 0) HandleFlipping(); }
        }

        else if (CurrentControlScheme == "Gamepad")
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

    public void PauseHandler(string pauseSource = "Pause")
    {
        if(pauseSource == "Pause") // if no value was placed, assume this was a 'regular' start-based pause (not an inventory one)
        {
            pauseMenu.TogglePauseUI();
            PauseToggleSupport();
        }
        else if(pauseSource == "Inventory")
        {
            if (pauseMenu.pauseMenu.activeSelf || pauseMenu.controlsMenu.activeSelf) { } // if pause menu is up, do nothing
            else { inventoryUI.ToggleUI(); PauseToggleSupport(); }
        }
    }

    void PauseToggleSupport()
    {
        if (!IsPaused)
        {
            if (GameState == "starting") { pauseHelper = true; }
            else { pauseHelper = false; }
            LimitToOnlyUIActions(true);
            IsPaused = true;
            Time.timeScale = 0;
        }
        else
        {
            IsPaused = false;
            if (pauseHelper) { GameState = "starting"; }
            else { GameState = "running"; }
            LimitToOnlyUIActions(false);
            Time.timeScale = 1;
        }
    }

    void LimitToOnlyUIActions(bool enableOnlyUIActions) // acts as a toggle that's called in Update, the onlyUIActionsToggle helps prevent the function contents from constantly being called
    {
        if (enableOnlyUIActions && onlyUIActionsToggle == false) { PlayerInput.actions.FindActionMap("UI").Enable(); PlayerInput.actions.FindActionMap("Player").Disable();  onlyUIActionsToggle = true; }
        else if (!enableOnlyUIActions && onlyUIActionsToggle == true) { PlayerInput.actions.FindActionMap("Player").Enable(); PlayerInput.actions.FindActionMap("UI").Disable(); onlyUIActionsToggle = false; }
    }

    public IEnumerator PlayHaptics(float optionalModifier = 1, float optionalTimeLength = .5f)
    {
        if (CurrentControlScheme == "Gamepad") { Gamepad.current.SetMotorSpeeds(.25f * optionalModifier, .75f * optionalModifier); } // shake the controller        
        yield return new WaitForSecondsRealtime(optionalTimeLength);
        InputSystem.ResetHaptics();
    }

    public void ResetPlayerMotionAndInput() { XInput = 0; YInput = 0; playerController.PlayerIdle(true); }
    public bool PlayerInputIdle() { if (XInput != 0 && YInput != 0) { return false; } else { return true; } }

    public void TriggerInteractButton() { InteractButton = true; } // used by cutscenes 

    public void HandleFlipping() { playerController.Flip(); PhysicsExtensions.FlipYScaleAxis(playerSecondaryWeapon.gameObject); }
}
