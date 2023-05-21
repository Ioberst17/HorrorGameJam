using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

////TODO: localization support

////TODO: deal with composites that have parts bound in different control schemes

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// A reusable component with a self-contained UI for rebinding a single action.
    /// </summary>
    public class RebindActionUI : MonoBehaviour
    {
        // CUSTOM CODE TO HANDLE INPUT CHANGES
        PlayerInput playerInput;
        InputAction action;
        string currentControlScheme;
        string keyboardAndMouse = "Keyboard and Mouse";
        string gamepad = "Gamepad";

        private void Start() { playerInput = FindObjectOfType<PlayerInput>(); action = playerInput.actions.FindAction(m_Action.name); }

        void Update() 
        {
            // handles if the player changes input method, uses string to ensure not constant reassignment
            if (playerInput.currentControlScheme.Equals(keyboardAndMouse) && currentControlScheme != keyboardAndMouse) 
            { 
                currentControlScheme = keyboardAndMouse; 
                SetBindingToNewControlScheme();  
            }

            else if (playerInput.currentControlScheme.Equals(gamepad) && currentControlScheme != gamepad) 
            {
                currentControlScheme = gamepad;
                SetBindingToNewControlScheme(); 
            }
        }

        void SetBindingToNewControlScheme()
        {
            // if composite action / can't use binding index or need to specify
            int bindingIndex = action.GetBindingIndex(group: playerInput.currentControlScheme);


            if (action.bindings[bindingIndex].isPartOfComposite)
            {
                // hard coded logic - assumes that if you found a part of a composite, that it's the first one.
                // And that the one preceeding it, must be the 'Composite head' that contains the parts
                bindingId = action.bindings[bindingIndex-1].id.ToString();
            }
            else { bindingId = action.bindings[bindingIndex].id.ToString(); } // if not a composite, bindingId can just be updated
        }

        void FindCompositeBindingIndex(InputAction action)
        {
            var bindings = action.bindings;

            for (int i = 0; i < bindings.Count; i++)
            {
                bindings[i].GetNameOfComposite();
            }
        }

        // CUSTOM CODE ABOVE

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
        public void UpdateBindingDisplay() 
        {
            var displayString = string.Empty;
            var deviceLayoutName = default(string); 
            var controlPath = default(string);

            // Get display string from action.
            var action = m_Action?.action;
            if (action != null)
            {
                var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, displayStringOptions);
            }

            // Set on label (if any).
            if (m_BindingText != null)
                m_BindingText.text = displayString;

            // Give listeners a chance to configure UI in response.
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
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
                // It's a composite. Remove overrides from part bindings.
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                {
                    action.RemoveBindingOverride(i);
                } 
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }
            UpdateBindingDisplay();
        }

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        public void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex)) { return; }

            //Debug.Log("The current action: " + action + " has this many bindings: " + action.bindings.Count + 
            //    ", and the next binding for the action: " + action.bindings[bindingIndex + 1] + ", has a boolComposite value of: " + action.bindings[bindingIndex + 1].isPartOfComposite);
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

                        //CUSTOM CODE BELOW for handling duplicate bindings

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
                        // for the next part.
                        //Debug.Log("The currently rebinded action is a composite: " + allCompositeParts);
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            //Debug.Log("The next rebinded action is called " + action.bindings[nextBindingIndex]);
                            //Debug.Log("The next rebinded action is part of the composite: " + action.bindings[nextBindingIndex].isPartOfComposite);
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
            if (m_RebindText != null)
            {
                var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                    ? $"{partName}Waiting for {m_RebindOperation.expectedControlType} input..."
                    : $"{partName}Waiting for input...";
                m_RebindText.text = text;
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
                    Debug.Log("Duplicate binding found: " + newBinding.effectivePath);
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
                        Debug.Log("Duplicate binding found: " + newBinding.effectivePath);
                        return true;
                    }
                }
            }
            return false;
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

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        private static List<RebindActionUI> s_RebindActionUIs;

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
        public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
        {
        }

        [Serializable]
        public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
        {
        }
    }
}
