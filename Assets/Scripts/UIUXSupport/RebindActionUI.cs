using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.LowLevel;
using System.Security.Cryptography;

////TODO: localization support

////TODO: deal with composites that have parts bound in different control schemes 

/// <summary>
/// A reusable component with a self-contained UI for rebinding a single action.
/// </summary>
public class RebindActionUI : MonoBehaviour
{
    // EXTERNAL REFERENCES

    UnityEngine.UI.Button resetButton;
    UnityEngine.UI.Button changeBindingButton;

    // buttons under changeBindingButton i.e. TriggerRebindButton in hierarchy
    GameObject actionBindingText;
    GameObject actionBindingIcon;
    GameObject actionBindingCompositeParent;

    PlayerInputIcons iconReference;
    PlayerInput playerInput;



    // CUSTOM CODE TO HANDLE INPUT CHANGES
    InputAction action;
    bool actionIsComposite;
    
    string currentControlScheme;
    public string KeyboardAndMouseSchemeName { get; set; } = "Keyboard and Mouse";
    public string GamepadSchemeName { get; set; } = "Gamepad";

    bool noActiveInputDisplay; // used as tracker for composite bindings that don't load on start


    IEnumerator<float> displayErrorMessage;

    [System.Serializable]
    public class InfoToPass // passed by events
    {
        public string displayString;
        public string deviceLayoutName;
        public string controlPath;
        public bool isCompositeHeadOrPart;
        public bool isCompositePart;
        public bool isCompositeHead;
        public int bindingIndex;
        public int compositeCounter;
    }

    bool errorMessageIsUp;

    [Tooltip("State of game object that this script is attached to")]
    [SerializeField]
    private bool m_isMenuObjectActive;

    [Tooltip("Reference to action that is to be rebound from the UI.")]
    [SerializeField]
    private InputActionReference m_Action;

    [SerializeField]
    private string m_BindingId;

    [SerializeField]
    private InputBinding.DisplayStringOptions m_DisplayStringOptions;

    [Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the "
        + "rebind UI not show a label for the action.")]
    [SerializeField]
    private Text m_ActionLabel;

    [Tooltip("Text label that will receive the current, formatted binding string.")]
    [SerializeField]
    private Text m_BindingText;

    [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
    [SerializeField]
    private GameObject m_RebindOverlay;

    [Tooltip("Optional text label that will be updated with prompt for user input.")]
    [SerializeField]
    private Text m_RebindText;

    [Tooltip("Optional text label that will be shown when an erroneous input is given.")]
    [SerializeField]
    private Text m_ErrorText;

    [Tooltip("Optional text label that shows how to cancel.")]
    [SerializeField]
    private Text m_CancellationText;

    [Tooltip("Optional image for button to cancel.")]
    [SerializeField]
    private UnityEngine.UI.Image m_CancellationImage;

    [Tooltip("Decide whether to include an image for the cancellation button in display; if false, only uses text")]
    [SerializeField]
    private bool m_CancellationBool;

    [Tooltip("Information captured by each action to Pass")]
    [SerializeField]
    private InfoToPass m_infoToPass;

    [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
        + "bindings in custom ways, e.g. using images instead of text.")]
    [SerializeField]
    private UpdateBindingUIEvent m_UpdateBindingUIEvent;

    [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
        + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
        + "customize the rebind.")]
    [SerializeField]
    private InteractiveRebindEvent m_RebindStartEvent;

    [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
    [SerializeField]
    private InteractiveRebindEvent m_RebindStopEvent;

    [Tooltip("Used for composite rebindings, as an addition to interactive rebind events")]
    [SerializeField]
    private CompositeRebindingEvent m_CompositeRebindEvent;

    private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

    private static List<RebindActionUI> s_RebindActionUIs;

    // NOTE:
    // ADDITIONAL KEY PROPERTIES LISTED AT BOTTOM OF SCRIPT

    private void Start()
    {
        // get external references
        playerInput = FindObjectOfType<PlayerInput>();
        action = playerInput.actions.FindAction(m_Action.name);
        actionIsComposite = action.bindings[action.GetBindingIndex(playerInput.currentControlScheme)].isComposite;
        iconReference = FindObjectOfType<PlayerInputIcons>();

        // apply listeners to buttons
        resetButton = ComponentFinder.GetComponentInChildrenByNameAndType<UnityEngine.UI.Button>("ResetToDefaultButton", this.gameObject, true);
        resetButton.onClick.AddListener(ResetToDefault);        
        
        changeBindingButton = ComponentFinder.GetComponentInChildrenByNameAndType<UnityEngine.UI.Button>("TriggerRebindButton", this.gameObject, true);
        changeBindingButton.onClick.AddListener(StartInteractiveRebind);

        // get other references
        actionBindingText = ComponentFinder.GetComponentInChildrenByNameAndType<Text>("ActionBindingText", changeBindingButton.gameObject, true).gameObject;
        actionBindingIcon = ComponentFinder.GetComponentInChildrenByNameAndType<ActionBindingIcon>("ActionBindingIcon", changeBindingButton.gameObject, true).gameObject;
        actionBindingCompositeParent = ComponentFinder.GetComponentInChildrenByNameAndType<ActionBindingIconCompositeParent>("ActionBindingIconComposite", changeBindingButton.gameObject, true).gameObject;


        m_ActionLabel = ComponentFinder.GetComponentInChildrenByNameAndType<Text>("ActionName", gameObject, true);
        m_BindingText = ComponentFinder.GetComponentInChildrenByNameAndType<Text>("ActionBindingText", gameObject, true);

        m_RebindText = ComponentFinder.GetComponentInChildrenByNameAndType<Text>("RebindingText", transform.parent.gameObject, true);
        m_RebindOverlay = m_RebindText.gameObject.transform.parent.gameObject;
        m_ErrorText = ComponentFinder.GetComponentInChildrenByNameAndType<Text>("ErrorMessage", m_RebindOverlay, true);
        m_CancellationText = ComponentFinder.GetComponentInChildrenByNameAndType<Text>("CancelObject", m_RebindOverlay, true);
        m_CancellationImage = ComponentFinder.GetComponentInChildrenByNameAndType<UnityEngine.UI.Image>("CancelIcon", m_RebindOverlay, true);
    }

    void Update()
    {
        // used as a workaround for composite bindings which do not always display on control menu load, it forces calls to update binding display
        // used if the binding button is displayed, but not text or image is displayed
        if (changeBindingButton.gameObject.activeSelf && GetCompositeDisplayStatus()) { UpdateBindingDisplay(); }

        CheckIfControlSchemChanged();
    }

    // returns true if none of the child binding displays of an action are active
    bool GetCompositeDisplayStatus()
    {
        // return true if none of the children of 
        if (actionBindingCompositeParent == null) { return false; } // not needed for non-composites
        else
        { // if none of these are active, return true
            if (!actionBindingText.activeSelf &&
            !actionBindingIcon.activeSelf &&
            !actionBindingCompositeParent.activeSelf)
            {
                return true;
            }
            else { return false; }
        }
    }


    void CheckIfControlSchemChanged()
    {
        // handles if the player changes input method, uses 'currentControlScheme' string as a tracker to ensure reassignment only needs to occur ones
        if (playerInput.currentControlScheme.Equals(KeyboardAndMouseSchemeName) && currentControlScheme != KeyboardAndMouseSchemeName)
        {
            currentControlScheme = KeyboardAndMouseSchemeName;
            actionIsComposite = action.bindings[action.GetBindingIndex(playerInput.currentControlScheme)].isComposite;
            SetBindingToNewControlScheme();
        }

        else if (playerInput.currentControlScheme.Equals(GamepadSchemeName) && currentControlScheme != GamepadSchemeName)
        {
            currentControlScheme = GamepadSchemeName;
            actionIsComposite = action.bindings[action.GetBindingIndex(playerInput.currentControlScheme)].isComposite;
            SetBindingToNewControlScheme();
        }
    }

    void SetBindingToNewControlScheme()
    {
        // if composite action / can't use binding index or need to specify
        int bindingIndex = action.GetBindingIndex(group: playerInput.currentControlScheme);

        if (action.bindings[bindingIndex].isComposite || action.bindings[bindingIndex].isPartOfComposite)
        {
            // find the head of the composite using FindCompositeBindingIndex, then get the first part using +1
            int intToPass = GetCompositeBindingIndex(action, playerInput.currentControlScheme, action.name);
            if (intToPass == -1) { Debug.Log("Could not find a composite binding with the current player control scheme of: " + playerInput.currentControlScheme); }
            else { bindingId = action.bindings[intToPass].id.ToString(); }
        }
        else { bindingId = action.bindings[bindingIndex].id.ToString(); } // if not a composite, bindingId can just be updated
    }

    // Get the index of a composite function within a group
    int GetCompositeBindingIndex(InputAction action, string groupName, string compositeName)
    {
        var compositeBinding = action.bindings
            .FirstOrDefault(binding => binding.isComposite && binding.groups.Contains(groupName));

        if (compositeBinding != null)
        {
            return action.bindings.IndexOf(x => x.name == compositeBinding.name) + 1;
        }

        return -1; // Composite head binding not found 
    }

    // CUSTOM CODE ABOVE

    /// <summary>
    /// When an interactive rebind is in progress, this is the rebind operation controller.
    /// Otherwise, it is <c>null</c>.
    /// </summary>
    public InputActionRebindingExtensions.RebindingOperation ongoingRebind => m_RebindOperation;

    /// <summary>
    /// Return the action and binding index for the binding that is targeted by the component
    /// according to
    /// </summary>
    /// <param name="action"></param>
    /// <param name="bindingIndex"></param>
    /// <returns></returns>
    public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;

        action = m_Action?.action;

        if (action == null)
            return false;

        if (string.IsNullOrEmpty(m_BindingId))
            return false;

        // Look up binding index.
        var bindingId = new Guid(m_BindingId);
        bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);

        if (bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Trigger a refresh of the currently displayed binding.
    /// </summary>
    public void UpdateBindingDisplay(int optionalCompositeCounter = 0) // optional value is to force a composite to be updated without an interactive rebind
    {
        var action = m_Action?.action; // get the InputAction
        var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == bindingId); // get the index of the current ID
        int bindingIndexForCompositeParts = bindingIndex + optionalCompositeCounter;

        if (optionalCompositeCounter != 0 && bindingIndex + optionalCompositeCounter > action.bindings.Count) { bindingIndexForCompositeParts = bindingIndex + optionalCompositeCounter; }

        bool bindingIsComposite = action.bindings[bindingIndex].isComposite; // refers to the head
        bool bindingIsCompositePart = action.bindings[bindingIndexForCompositeParts].isPartOfComposite; // refers to a composite part

        // get the right information to pass into binding display
        if (bindingIsCompositePart) // if a composite, get composite information by using the binding index
        {
            (string displayString, string deviceLayoutName, string controlPath) = GetDisplayStringInfo(bindingIndexForCompositeParts);
            infoToPass.displayString = displayString;
            infoToPass.deviceLayoutName = deviceLayoutName;
            infoToPass.controlPath = controlPath;
            infoToPass.compositeCounter = optionalCompositeCounter;

            // Set on label (if any).
            if (m_BindingText != null)
                m_BindingText.text = displayString;
        }
        else// assume a non-comosite 'standard' single button input
        {
            (string displayString, string deviceLayoutName, string controlPath) = GetDisplayStringInfo();
            infoToPass.displayString = displayString;
            infoToPass.deviceLayoutName = deviceLayoutName;
            infoToPass.controlPath = controlPath;
            infoToPass.compositeCounter = 0;

            // Set on label (if any).
            if (m_BindingText != null)
                m_BindingText.text = displayString;
        }

        infoToPass.isCompositePart = bindingIsCompositePart;
        infoToPass.isCompositeHead = action.bindings[bindingIndex].isComposite;
        infoToPass.isCompositeHeadOrPart = infoToPass.isCompositePart || infoToPass.isCompositeHead;
        infoToPass.bindingIndex = bindingIndex;

        // Give listeners a chance to configure UI in response, but don't send a UI update for a composite event e.g. WASD (only for the underlying W-A-S-D
        m_UpdateBindingUIEvent?.Invoke(this, infoToPass);

        // check to see if the next action would be part of a composite
        bool nextIsPartOfComposite = false;
        if (bindingIndexForCompositeParts + 1 < action.bindings.Count)
        {
            nextIsPartOfComposite = action.bindings[bindingIndexForCompositeParts + 1].isPartOfComposite;
        }

        // if so, call the function again
        optionalCompositeCounter++;
        if (nextIsPartOfComposite)
        { UpdateBindingDisplay(optionalCompositeCounter); } // if we want to force an update and the next binding would be part of a composite - go
    }

    public  (string, string, string) GetDisplayStringInfo(int useCompositeInt = -1)
    {
        var displayString = string.Empty;
        var deviceLayoutName = default(string);
        var controlPath = default(string);
        int bindingIndex = -1;
        var action = m_Action?.action;

        if (useCompositeInt == -1) // if a default value is used, it's not a composite
        {
            bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
            if (bindingIndex != -1)
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
        }
        else // else use the composite Int
        {
            displayString = action.GetBindingDisplayString(useCompositeInt, out deviceLayoutName, out controlPath, displayStringOptions);
        }

        return (displayString, deviceLayoutName, controlPath);
    }

    /// <summary>
    /// Remove currently applied binding overrides.
    /// </summary>
    /// 

    public void ResetToDefault()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex)) { return; }

        // Check for duplicate bindings before resetting to default, and if found, swap the two controls.

        if (SwapResetBindings(action, bindingIndex))
        {
            UpdateBindingDisplay();
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            // It's a composite. Remove overrides from the PART bindings, since composite bindings have a head and parts e.g. WASD and it's parts W, A, S, D.
            for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
            {
                action.RemoveBindingOverride(i);
            }
            UpdateBindingDisplay(); 
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
    /// for the action.
    /// </summary>
    public void StartInteractiveRebind()
    {
        if (!ResolveActionAndBinding(out var action, out var bindingIndex)) { return; }

        MakeErrorMessageInvisible();

        // If the binding is a composite, we need to rebind each part in turn.
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
        }
        else { PerformInteractiveRebind(action, bindingIndex); } // if it's not composite, just do a single rebind
    }

    private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.

        void CleanUp()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;
        }

        // CUSTOM CODE BELOW
        // Disable the action before use (needs to be done for any rebinding)
        action.Disable();
        // CUSTOM CODE ABOVE


        // Configure the rebind.
        m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)   // CUSTOM CODE BELOW
            .WithControlsExcluding("<Mouse>/leftButton") // don't allow left click mouse rebinding
            .WithControlsExcluding("<Mouse>/rightButton")
            .WithControlsExcluding("<Mouse>/press")
            .WithControlsExcluding("<Pointer>/position")
            .WithCancelingThrough("<Keyboard>/escape") // allow escape to exit
                                                       // CUSTOM CODE ABOVE
            .OnCancel(
                operation =>
                {
                    action.Enable();
                    m_RebindStopEvent?.Invoke(this, operation);
                    m_RebindOverlay?.SetActive(false);
                    UpdateBindingDisplay();
                    CleanUp();
                })
            .OnComplete(
                operation =>
                {
                    action.Enable();
                    m_RebindOverlay?.SetActive(false);
                    m_RebindStopEvent?.Invoke(this, operation);

                    //CUSTOM CODE BELOW
                    // handle duplicate bindings
                    if (CheckDuplicateBindings(action, bindingIndex, allCompositeParts))
                    {
                        action.RemoveBindingOverride(bindingIndex);
                        CleanUp();
                        PerformInteractiveRebind(action, bindingIndex, allCompositeParts);
                        return;
                    }
                    //CUSTOM CODE ABOVE

                    UpdateBindingDisplay();

                    CleanUp();

                    // If there's more composite parts we should bind, initiate a rebind
                    //Debug.Log("The currently rebinded action is a composite: " + allCompositeParts);
                    if (allCompositeParts)
                    {
                        var nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                    }
                });

        // If it's a part binding, show the name of the part in the UI.
        var partName = default(string);
        if (action.bindings[bindingIndex].isPartOfComposite)
            partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

        // Bring up rebind overlay, if we have one.
        m_RebindOverlay?.SetActive(true);
        // display rebind text
        if (m_RebindText != null)
        {
            var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                ? $"{partName}Waiting for {m_RebindOperation.expectedControlType} input for {m_RebindOperation.action.name}"
                : $"{partName}Waiting for input...";
            m_RebindText.text = text;
        }

        // decide on cancellation text
        if (m_CancellationBool == true)
        {
            // get the right image
            cancellationImage.gameObject.SetActive(true);

            // get pause Icon
            var displayString = string.Empty; var deviceLayoutName = default(string); var controlPath = default(string);

            var pauseAction = playerInput.actions.FindAction("Pause");
            if (pauseAction != null)
            {
                var bindingIndexForPause = pauseAction.bindings.IndexOf(x => x.groups.Contains(currentControlScheme));
                if (bindingIndexForPause != -1)
                    displayString = pauseAction.GetBindingDisplayString(bindingIndexForPause, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            cancellationImage.sprite = iconReference.UpdateStaticIconImages(this, deviceLayoutName, controlPath, false, "CancelIcon");
        }
        else if (m_CancellationText != null)
        {
            var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                ? $"{partName}Press {playerInput.actions.FindAction("Pause").name} to cancel"
                : $"{partName}Press menu button to cancel";
            m_CancellationText.text = text;
        }

        // If we have no rebind overlay and no callback but we have a binding text label,
        // temporarily set the binding text label to "<Waiting>".
        if (m_RebindOverlay == null && m_RebindText == null && m_RebindStartEvent == null && m_BindingText != null)
            m_BindingText.text = "<Waiting...>";

        // Give listeners a chance to act on the rebind starting.
        m_RebindStartEvent?.Invoke(this, m_RebindOperation);

        m_RebindOperation.Start();
    }

    // CUSTOM FUNCTION BELOW

    private bool CheckDuplicateBindings(InputAction action, int bindingIndex, bool allCompositeParts)  // pass in action, its location, and a bool to see if it's a composite action
    {
        InputBinding newBinding = action.bindings[bindingIndex];
        foreach (InputBinding binding in action.actionMap.bindings)
        {
            if (binding.action == action.bindings[bindingIndex].action) // if the input action is itself skip / keep going
            {
                continue;
            }
            if (binding.effectivePath == newBinding.effectivePath)
            {

                Debug.Log("Duplicate key found: " + newBinding.effectivePath);
                StartErrorMessageDisplay("That key is already in use! Try another key");
                return true;
            }
        }
        // Check for duplicates in composite bindings e.g. WASD movement
        if (allCompositeParts)
        {
            for (int i = 1; i < bindingIndex; i++)
            {
                if (action.bindings[i].effectivePath == newBinding.effectivePath)
                {
                    Debug.Log("Duplicate key found: " + newBinding.effectivePath);
                    StartErrorMessageDisplay("That key is already in use! Try another key");
                    return true;
                }
            }
        }
        return false;
    }

    void StartErrorMessageDisplay(string errorMessage)
    {
        if (errorMessageIsUp)
        {
            StopCoroutine(DisplayErrorMessage(errorMessage));
            StartCoroutine(DisplayErrorMessage(errorMessage));
        }
        else { StartCoroutine(DisplayErrorMessage(errorMessage)); }
    }

    void MakeErrorMessageInvisible()
    {
        StopCoroutine(DisplayErrorMessage(""));
        errorMessageIsUp = false;
        m_ErrorText.gameObject.SetActive(true);
        m_ErrorText.CrossFadeAlpha(0f, 0, true); // set alpha to 0
    }


    // DISPLAYS ERROR MESSAGES
    IEnumerator DisplayErrorMessage(string errorMessage)
    {
        errorMessageIsUp = true;

        float fadeInTime = 0.7f;
        float displayTime = 0.5f;
        float fadeOutTime = 1;

        m_ErrorText.text = errorMessage;
        m_ErrorText.CrossFadeAlpha(0f, 0f, true);
        m_ErrorText.CrossFadeAlpha(1f, fadeInTime, true);
        m_ErrorText.gameObject.SetActive(true);

        // Wait for display time
        yield return new WaitForSecondsRealtime(displayTime);

        // Fade out error message
        m_ErrorText.CrossFadeAlpha(0f, fadeOutTime, true);

        // Wait for fade out time
        yield return new WaitForSecondsRealtime(fadeOutTime);

        errorMessageIsUp = false;
        yield return null;
    }

    // CUSTOM FUNCTION ABOVE

    // CUSTOM FUNCTION BELOW for if duplicate bindings would be created from resetting

    /// <summary>
    /// Check for duplicate rebindings when the binding is going to be set to default.
    /// </summary>
    /// <param name="action">InputAction we are resetting.</param>
    /// <param name="bindingIndex">Current index of the control we are rebinding.</param>
    /// <returns></returns>
    private bool SwapResetBindings(InputAction action, int bindingIndex)
    {
        // Cache a reference to the current binding.
        InputBinding newBinding = action.bindings[bindingIndex];

        // Check all of the bindings in the current action map to make sure there are no duplicates.
        for (int i = 0; i < action.actionMap.bindings.Count; ++i)
        {
            InputBinding binding = action.actionMap.bindings[i];
            if (binding.action == newBinding.action)
            {
                continue;
            }

            if (binding.effectivePath == newBinding.path)
            {
                Debug.Log("Duplicate binding found for reset to default: " + newBinding.effectivePath);
                StartErrorMessageDisplay("Duplicate binding found when resetting to default. Swapping bindings.");

                // Swap the two actions.

                action.actionMap.FindAction(binding.action).ApplyBindingOverride(i, newBinding.overridePath);

                action.RemoveBindingOverride(bindingIndex);

                return true;
            }
        }
        return false;
    }
    // CUSTOM FUNCTION ABOVE


    protected void OnEnable()
    {
        if (s_RebindActionUIs == null)
            s_RebindActionUIs = new List<RebindActionUI>();
        s_RebindActionUIs.Add(this);
        if (s_RebindActionUIs.Count == 1)
            InputSystem.onActionChange += OnActionChange;


    }

    protected void OnDisable()
    {
        m_RebindOperation?.Dispose();
        m_RebindOperation = null;

        s_RebindActionUIs.Remove(this);
        if (s_RebindActionUIs.Count == 0)
        {
            s_RebindActionUIs = null;
            InputSystem.onActionChange -= OnActionChange;
        }
    }

    // When the action system re-resolves bindings, we want to update our UI in response. While this will
    // also trigger from changes we made ourselves, it ensures that we react to changes made elsewhere. If
    // the user changes keyboard layout, for example, we will get a BoundControlsChanged notification and
    // will update our UI to reflect the current keyboard layout.
    private static void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.BoundControlsChanged)
            return;

        var action = obj as InputAction;
        var actionMap = action?.actionMap ?? obj as InputActionMap;
        var actionAsset = actionMap?.asset ?? obj as InputActionAsset;

        for (var i = 0; i < s_RebindActionUIs.Count; ++i)
        {
            var component = s_RebindActionUIs[i];
            var referencedAction = component.actionReference?.action;
            if (referencedAction == null)
                continue;

            if (referencedAction == action ||
                referencedAction.actionMap == actionMap ||
                referencedAction.actionMap?.asset == actionAsset)
                component.UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Reference to the action that is to be rebound.
    /// </summary>
    public InputActionReference actionReference
    {
        get => m_Action;
        set
        {
            m_Action = value;
            UpdateActionLabel();
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// ID (in string form) of the binding that is to be rebound on the action.
    /// </summary>
    /// <seealso cref="InputBinding.id"/>
    public string bindingId
    {
        get => m_BindingId;
        set
        {
            m_BindingId = value;
            UpdateBindingDisplay();
        }
    }

    public InputBinding.DisplayStringOptions displayStringOptions
    {
        get => m_DisplayStringOptions;
        set
        {
            m_DisplayStringOptions = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Text component that receives the name of the action. Optional.
    /// </summary>
    public Text actionLabel
    {
        get => m_ActionLabel;
        set
        {
            m_ActionLabel = value;
            UpdateActionLabel();
        }
    }

    /// <summary>
    /// Text component that receives the display string of the binding. Can be <c>null</c> in which
    /// case the component entirely relies on <see cref="updateBindingUIEvent"/>.
    /// </summary>
    public Text bindingText
    {
        get => m_BindingText;
        set
        {
            m_BindingText = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Optional text component that receives a text prompt when waiting for a control to be actuated.
    /// </summary>
    /// <seealso cref="startRebindEvent"/>
    /// <seealso cref="rebindOverlay"/>
    public Text rebindPrompt
    {
        get => m_RebindText;
        set => m_RebindText = value;
    }

    // CUSTOM CODE BELOW

    /// <summary>
    /// Optional text component that displays how to cancel
    /// </summary>
    /// <seealso cref="startRebindEvent"/>
    /// <seealso cref="rebindOverlay"/>
    public Text cancellationPrompt
    {
        get => m_CancellationText;
        set => m_CancellationText = value;
    }

    /// <summary>
    /// Optional text component that displays how to cancel
    /// </summary>
    /// <seealso cref="startRebindEvent"/>
    /// <seealso cref="rebindOverlay"/>
    public UnityEngine.UI.Image cancellationImage
    {
        get => m_CancellationImage;
        set => m_CancellationImage = value;
    }

    /// <summary>
    /// Optional text component that display an error, if wrong input is given
    /// </summary>
    /// <seealso cref="startRebindEvent"/>
    /// <seealso cref="rebindOverlay"/>
    public Text errorPrompt
    {
        get => m_ErrorText;
        set => m_ErrorText = value;
    }

    /// <summary>
    /// Optional text component that display an error, if wrong input is given
    /// </summary>
    /// <seealso cref="startRebindEvent"/>
    /// <seealso cref="rebindOverlay"/>
    public InfoToPass infoToPass
    {
        get => m_infoToPass;
        set => m_infoToPass = value;
    }

    public bool isMenuObjectActive
    {
        get => m_isMenuObjectActive;
        set => m_isMenuObjectActive = value;
    }



    // CUSTOM CODE ABOVE

    /// <summary>
    /// Optional UI that is activated when an interactive rebind is started and deactivated when the rebind
    /// is finished. This is normally used to display an overlay over the current UI while the system is
    /// waiting for a control to be actuated.
    /// </summary>
    /// <remarks>
    /// If neither <see cref="rebindPrompt"/> nor <c>rebindOverlay</c> is set, the component will temporarily
    /// replaced the <see cref="bindingText"/> (if not <c>null</c>) with <c>"Waiting..."</c>.
    /// </remarks>
    /// <seealso cref="startRebindEvent"/>
    /// <seealso cref="rebindPrompt"/>
    public GameObject rebindOverlay
    {
        get => m_RebindOverlay;
        set => m_RebindOverlay = value;
    }

    /// <summary>
    /// Event that is triggered every time the UI updates to reflect the current binding.
    /// This can be used to tie custom visualizations to bindings.
    /// </summary>
    public UpdateBindingUIEvent updateBindingUIEvent
    {
        get
        {
            if (m_UpdateBindingUIEvent == null)
                m_UpdateBindingUIEvent = new UpdateBindingUIEvent();
            return m_UpdateBindingUIEvent;
        }
    }

    /// <summary>
    /// Event that is triggered when an interactive rebind is started on the action.
    /// </summary>
    public InteractiveRebindEvent startRebindEvent
    {
        get
        {
            if (m_RebindStartEvent == null)
                m_RebindStartEvent = new InteractiveRebindEvent();
            return m_RebindStartEvent;
        }
    }

    /// <summary>
    /// Event that is triggered when an interactive rebind has been completed or canceled.
    /// </summary>
    public InteractiveRebindEvent stopRebindEvent
    {
        get
        {
            if (m_RebindStopEvent == null)
                m_RebindStopEvent = new InteractiveRebindEvent();
            return m_RebindStopEvent;
        }
    }

    // We want the label for the action name to update in edit mode, too, so
    // we kick that off from here.
#if UNITY_EDITOR
    protected void OnValidate()
    {
        UpdateActionLabel();
        UpdateBindingDisplay();
    }

#endif

    private void UpdateActionLabel()
    {
        if (m_ActionLabel != null)
        {
            var action = m_Action?.action;
            m_ActionLabel.text = action != null ? action.name : string.Empty;
        }
    }

    [Serializable]
    public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, RebindActionUI.InfoToPass>
    {
    }

    [Serializable]
    public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
    {
    }

    [Serializable]
    public class CompositeRebindingEvent : UnityEvent<bool, int>
    {
    }
}
