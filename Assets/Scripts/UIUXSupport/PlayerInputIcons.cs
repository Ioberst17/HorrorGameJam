using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

////TODO: have updateBindingUIEvent receive a control path string, too (in addition to the device layout name)

    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RebindActionUI.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class PlayerInputIcons : MonoBehaviour
    {
        // Icon Data
        public KeyboardAndMouseIcons keyboardandmouse;
        public GamepadIcons xbox;
        public GamepadIcons ps4;

        // External References for Data
        private PlayerInput playerInput;

        // External References to Change
        GameObject controlsMenu;

        // Internal Variables for Tracking
        string currentControlScheme;
        [SerializeField] bool workingOnAComposite;
        [SerializeField] int compositeIndex;

        [SerializeField] GameObject pauseMenuUI;
        [SerializeField] Transform nonCompositeIcon;
        [SerializeField] Transform compositeIcon;
        [SerializeField] ActionBindingIconComposite[] compositeIconArray = new ActionBindingIconComposite[4]; // assumes there are 4 objects in a composite - stored under a iconCompositeParent
        [SerializeField] GameObject iconCompositeParent;
        [SerializeField] string compositeImageGameObjectName;
        [SerializeField] Image iconToChange;
        

        protected void Start()
        {
            playerInput = FindObjectOfType<PlayerInput>();
            pauseMenuUI = GameObject.Find("PauseMenu");
            controlsMenu = ComponentFinder.GetComponentInChildrenByNameAndType<Image>("ControlsMenu", pauseMenuUI, true).gameObject;

            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = controlsMenu.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                //component.UpdateBindingDisplay();
            }
        }

        // called from RebindActionUI when a display changes
        protected void OnUpdateBindingDisplay(RebindActionUI component, RebindActionUI.InfoToPass eventInfo)
        {
            if (string.IsNullOrEmpty(eventInfo.deviceLayoutName) || string.IsNullOrEmpty(eventInfo.controlPath))
                return;

            // get the current control scheme
            currentControlScheme = playerInput.currentControlScheme;
            workingOnAComposite = eventInfo.isCompositeHeadOrPart;
            compositeIndex = eventInfo.bindingIndex + eventInfo.compositeCounter;

            // get the relevant image
            var icon = GetSprite(eventInfo.deviceLayoutName, eventInfo.controlPath);


            // get the relevant text component
            var textComponent = component.bindingText;

            // Grab the right image component
            // if composite, get a reference to the composite object
            if (eventInfo.isCompositeHeadOrPart)
            {
                if(component == null) { Debug.Log("RebindActionUI that was passed in is null");  }
                if(component.gameObject == null) { Debug.Log("RebindActionUI's gameobject is null");  }
                iconCompositeParent = ComponentFinder.GetComponentInChildrenByNameAndType<ActionBindingIconCompositeParent>("ActionBindingIconComposite", component.gameObject, true).gameObject;
                iconCompositeParent.gameObject.SetActive(true);
                if (eventInfo.isCompositePart)
                {
                    compositeIconArray = iconCompositeParent.GetComponentsInChildren<ActionBindingIconComposite>(true);
                    int intToUse = (int)eventInfo.compositeCounter;
                    compositeImageGameObjectName = "Image" + intToUse.ToString();
                    compositeIcon = compositeIconArray.FirstOrDefault(c => c.gameObject.name == compositeImageGameObjectName).transform;
                }                
            }
            // always will need a reference to the non-composite icon
            nonCompositeIcon = component.GetComponentInChildren<ActionBindingIcon>(true).transform; 

            // image component should be a reference to the correct object
            if (eventInfo.isCompositePart) { iconToChange = compositeIcon.GetComponent<Image>(); }
            else if (eventInfo.isCompositeHead) { iconToChange = compositeIcon.GetComponent<Image>(); }
            else { iconToChange = nonCompositeIcon.GetComponent<Image>(); }

            // decide whether to use icons or text
            if("Keyboard and Mouse"  == playerInput.currentControlScheme)
            {
                IconVsTextUpdate(icon, textComponent, iconToChange, component.KeyboardAndMouseSchemeName, eventInfo);
            }
            else if("Gamepad" == playerInput.currentControlScheme)
            {
                IconVsTextUpdate(icon, textComponent, iconToChange, component.GamepadSchemeName, eventInfo);
            }
            else
            {
                Debug.Log("Attempting to update icons vs text in rebinding UI; however, the player's current control scheme name does not match those on value of this script");
            }
        }

        // UPDATE REBINDACTIONUI SCRIPTS WITH IMAGE OR TEXT

        private void IconVsTextUpdate(Sprite icon, Text textToUpdate, Image imageToUpdate, string controlSchemeMatch, RebindActionUI.InfoToPass eventInfo)
        {
            // set everything that can be displayed to false, then turn on what's relevant
            textToUpdate.gameObject.SetActive(false);
            nonCompositeIcon.gameObject.SetActive(false);
            if (iconCompositeParent != null && compositeIcon != null) // if there are composite icon slots, make sure they're false;
            {
                iconCompositeParent.gameObject.SetActive(false);
                compositeIcon.gameObject.SetActive(false);
            }

            // turn relevant objects on
            if (icon != null && playerInput.currentControlScheme == controlSchemeMatch) // if images are used, then use this branch of logic
            {
                if (eventInfo.isCompositePart)
                {
                    iconCompositeParent.gameObject.SetActive(true); // parent needs to be one for a composite part to be displayed
                }
                else if (eventInfo.isCompositeHead)
                {
                    iconCompositeParent.gameObject.SetActive(true);
                }
                else // working on a non-composite actions
                {
                    nonCompositeIcon.gameObject.SetActive(true);
                }

                imageToUpdate.gameObject.SetActive(true);


                // scaling of buttons
                if ("Keyboard and Mouse" == playerInput.currentControlScheme) { imageToUpdate.rectTransform.localScale = new Vector3(.9f, .9f, 1); }
                else if ("Gamepad" == playerInput.currentControlScheme) { imageToUpdate.rectTransform.localScale = new Vector3(.65f, .85f, 1); }

                // update image
                imageToUpdate.sprite = icon;
            }
            else // else, use text
            {
                textToUpdate.gameObject.SetActive(true);
            }
        }

        // used by other scripts: rebinding panel to display error messages / other external scripts that need an icon; i.e. actions that will not change
        // assumes you're working with a RebindActionUI script

        public Sprite UpdateStaticIconImages(RebindActionUI component, string deviceLayoutName, string controlPath, bool usedInRebindActionUIPrefab = true, string altObjectName = "")
        {
            // get the current control scheme
            currentControlScheme = playerInput.currentControlScheme;

            // get the relevant image
            var icon = GetSprite(deviceLayoutName, controlPath);

            return icon;
        }

        private Sprite GetSprite(string deviceLayoutName, string controlPath)
        {
            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard"))
                icon = keyboardandmouse.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = keyboardandmouse.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = xbox.GetSprite(controlPath);

            return icon;
        }

        // STRUCTS

        [Serializable]
        public struct KeyboardAndMouseIcons
        {
            [Header("Alpha Keys")]
            // alpha
            public Sprite A_Key_Dark;
            public Sprite B_Key_Dark;
            public Sprite C_Key_Dark;
            public Sprite D_Key_Dark;
            public Sprite E_Key_Dark;
            public Sprite F_Key_Dark;
            public Sprite G_Key_Dark;
            public Sprite H_Key_Dark;
            public Sprite I_Key_Dark;
            public Sprite J_Key_Dark;
            public Sprite K_Key_Dark;
            public Sprite L_Key_Dark;
            public Sprite M_Key_Dark;
            public Sprite N_Key_Dark;
            public Sprite O_Key_Dark;
            public Sprite P_Key_Dark;
            public Sprite Q_Key_Dark;
            public Sprite R_Key_Dark;
            public Sprite S_Key_Dark;
            public Sprite T_Key_Dark;
            public Sprite U_Key_Dark;
            public Sprite V_Key_Dark;
            public Sprite W_Key_Dark;
            public Sprite X_Key_Dark;
            public Sprite Y_Key_Dark;
            public Sprite Z_Key_Dark;

            // movement composite
            public Sprite WASD;

            [Header("Numeric Keys")]
            // numeric
            public Sprite Num0_Key_Dark;
            public Sprite Num1_Key_Dark;
            public Sprite Num2_Key_Dark;
            public Sprite Num3_Key_Dark;
            public Sprite Num4_Key_Dark;
            public Sprite Num5_Key_Dark;
            public Sprite Num6_Key_Dark;
            public Sprite Num7_Key_Dark;
            public Sprite Num8_Key_Dark;
            public Sprite Num9_Key_Dark;

            [Header("Other: Traditional Keys")]
            // other: traditional
            public Sprite Esc_Key_Dark;
            public Sprite Space_Key_Dark;
            public Sprite Slash_Key_Dark;
            public Sprite Tab_Key_Dark;
            public Sprite Backslash_Key_Dark;
            public Sprite Tilda_Key_Dark;
            public Sprite Backspace_Key_Dark;
            public Sprite Caps_Lock_Key_Dark;

            [Header("Other: Traditional Keys - SHIFT")]
            // Shifts
            public Sprite Shift_Key_Dark;
            public Sprite Right_Shift_Key_Dark;
            public Sprite Left_Shift_Key_Dark;

            [Header("Other: Traditional Keys - CTRL")]
            // Controls
            public Sprite Ctrl_Key_Dark;
            public Sprite Right_Ctrl_Key_Dark;
            public Sprite Left_Ctrl_Key_Dark;

            [Header("Other: Traditional Keys - ALT")]
            // Alts
            public Sprite Alt_Key_Dark;
            public Sprite Left_Alt_Key_Dark;
            public Sprite Right_Alt_Key_Dark;

            [Header("Other: Bracket Keys")]
            // other: brackets
            public Sprite Mark_Left_Key;
            public Sprite Mark_Right_Key;
            public Sprite Minus_Key_Dark;
            public Sprite Plus_Key_Dark;

            [Header("Other: Punctuation Keys")]
            // other: punctations
            public Sprite Colon_Key_Dark;
            public Sprite Semicolon_Key_Dark;
            public Sprite Question_Key_Dark;

            [Header("Arrow Keys")]
            // other: arrows
            public Sprite Up_Arrow_Dark;
            public Sprite Down_Arrow_Dark;
            public Sprite Left_Arrow_Dark;
            public Sprite Right_Arrow_Dark;

            [Header("Mouse Keys")]
            // mouse
            public Sprite Mouse_Left_Key_Dark;
            public Sprite Mouse_Right_Key_Dark;

            //public Sprite selectButton; // not used in game
            //public Sprite leftStickPress;
            //public Sprite rightStickPress;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    // alpha
                    case "a": return A_Key_Dark;
                    case "b": return B_Key_Dark;
                    case "c": return C_Key_Dark;
                    case "d": return D_Key_Dark;
                    case "e": return E_Key_Dark;
                    case "f": return F_Key_Dark;
                    case "g": return G_Key_Dark;
                    case "h": return H_Key_Dark;
                    case "i": return I_Key_Dark;
                    case "j": return J_Key_Dark;
                    case "k": return K_Key_Dark;
                    case "l": return L_Key_Dark;
                    case "m": return M_Key_Dark;
                    case "n": return N_Key_Dark;
                    case "o": return O_Key_Dark;
                    case "p": return P_Key_Dark;
                    case "q": return Q_Key_Dark;
                    case "r": return R_Key_Dark;
                    case "s": return S_Key_Dark;
                    case "t": return T_Key_Dark;
                    case "u": return U_Key_Dark;
                    case "v": return V_Key_Dark;
                    case "w": return W_Key_Dark;
                    case "x": return X_Key_Dark;
                    case "y": return Y_Key_Dark;
                    case "z": return Z_Key_Dark;

                    // numeric
                    case "0": return Num0_Key_Dark;
                    case "1": return Num1_Key_Dark;
                    case "2": return Num2_Key_Dark;
                    case "3": return Num3_Key_Dark;
                    case "4": return Num4_Key_Dark;
                    case "5": return Num5_Key_Dark;
                    case "6": return Num6_Key_Dark;
                    case "7": return Num7_Key_Dark;
                    case "8": return Num8_Key_Dark;
                    case "9": return Num9_Key_Dark;

                    // other
                    case "escape": return Esc_Key_Dark;
                    case "space": return Space_Key_Dark;
                    case "slash": return Slash_Key_Dark;
                    case "capsLock": return Caps_Lock_Key_Dark;
                    case "backspace": return Backspace_Key_Dark;
                    case "tab": return Tab_Key_Dark;

                    case "shift": return Shift_Key_Dark;
                    case "rightShift": return Right_Shift_Key_Dark;
                    case "leftShift": return Left_Shift_Key_Dark;

                    case "ctrl": return Ctrl_Key_Dark;
                    case "rightCtrl": return Right_Ctrl_Key_Dark;
                    case "leftCtrl": return Left_Ctrl_Key_Dark;

                    case "alt": return Alt_Key_Dark;
                    case "rightAlt": return Right_Alt_Key_Dark;
                    case "leftAlt": return Left_Alt_Key_Dark;                    
                    

                    case "leftButton": return Mouse_Left_Key_Dark;
                    case "rightButton": return Mouse_Right_Key_Dark;
                }
                return null;
            }
        }

        [Serializable]
        public struct GamepadIcons
        {
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite startButton;
            public Sprite selectButton;
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            public Sprite leftStick;
            public Sprite rightStick;
            public Sprite leftStickPress;
            public Sprite rightStickPress;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    case "buttonSouth": return buttonSouth;
                    case "buttonNorth": return buttonNorth;
                    case "buttonEast": return buttonEast;
                    case "buttonWest": return buttonWest;
                    case "start": return startButton;
                    case "select": return selectButton;
                    case "leftTrigger": return leftTrigger;
                    case "rightTrigger": return rightTrigger;
                    case "leftShoulder": return leftShoulder;
                    case "rightShoulder": return rightShoulder;
                    case "dpad": return dpad;
                    case "dpad/up": return dpadUp;
                    case "dpad/down": return dpadDown;
                    case "dpad/left": return dpadLeft;
                    case "dpad/right": return dpadRight;
                    case "leftStick": return leftStick;
                    case "rightStick": return rightStick;
                    case "leftStickPress": return leftStickPress;
                    case "rightStickPress": return rightStickPress;
                }
                return null;
            }
        }

        public GameObject FindInactiveGameObject(Transform startingObject, string name)
        {
            if (startingObject == null)
            {
                Debug.LogError("Starting object is null");
                return null;
            }

            GameObject foundObject = null;

            // Check if the startingObject is a Transform or RectTransform
            if (startingObject is Transform || startingObject is RectTransform)
            {
                foreach (Transform child in startingObject.GetComponentsInChildren<Transform>(true))
                {
                    // Check if the child is either a Transform or RectTransform
                    if ((child is Transform || child is RectTransform) && child.gameObject.name == name && !child.gameObject.activeSelf)
                    {
                        foundObject = child.gameObject;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("Invalid starting object type");
            }

            if (foundObject != null)
            {
                Debug.Log("Inactive object found: " + foundObject.name);
            }
            else
            {
                Debug.Log("Inactive object not found");
            }

            return foundObject;
        }

    }
