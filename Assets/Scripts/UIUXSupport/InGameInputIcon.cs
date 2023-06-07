using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InGameInputIcon : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerInputIcons playerInputIcons;
    SpriteRenderer spriteRenderer;
    [SerializeField] RebindActionUI actionToBindTo;
    [SerializeField] int bindingIndex;
    [SerializeField] string currentControlSchemeTracker;
    [SerializeField] bool isVisible = false;
    [SerializeField] private string actionName = "Interact";
    [SerializeField] bool initialized = false;

    // other hierarchy references; used to speed up access to getting right RebindUIObject
    GameObject pauseMenuUI;
    GameObject controlsMenu;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerInputIcons = FindObjectOfType<PlayerInputIcons>(); // stores player icons

        spriteRenderer = GetComponent<SpriteRenderer>(); // sprite to change

        // makes finding references faster later
        pauseMenuUI = GameObject.Find("PauseMenuUI");
        controlsMenu = ComponentFinder.GetComponentInChildrenByNameAndType<Image>("ControlsMenu", pauseMenuUI, true).gameObject;
    }

    private void Update()
    {
        if(playerInput.currentControlScheme != currentControlSchemeTracker) 
        {
            currentControlSchemeTracker = playerInput.currentControlScheme;
            OnControlsChanged();
        }
    }

    private void OnBecameVisible() // a default Unity callback when an object is in frame
    {
        isVisible = true;
        GetIconReference();
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    private void GetIconReference()
    {
        if (!initialized)
        {
            var rebindUIComponents = controlsMenu.GetComponentsInChildren<RebindActionUI>(true);
            foreach (var component in rebindUIComponents)
            {
                if (component.gameObject.name == actionName) // make sure we only do this for the action we want and that it matches the gameobject name that the RebindActionUI is attached to e.g. "Move" gameobject in the control menu
                {
                    component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                    actionToBindTo = component;
                    initialized = true;
                }
            }
        }
    }

    void OnUpdateBindingDisplay(RebindActionUI component, RebindActionUI.InfoToPass eventInfo)
    {
        spriteRenderer.sprite = playerInputIcons.UpdateStaticIconImages(component, eventInfo.deviceLayoutName, eventInfo.controlPath);
        spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
    }

    void OnControlsChanged() // used if Controls Menu is not up e.g. if in-game
    {
        if (initialized)
        {
            bindingIndex = actionToBindTo.actionReference.action.GetBindingIndex(group: playerInput.currentControlScheme);
            (string displayString, string deviceLayoutName, string controlPath) = actionToBindTo.GetDisplayStringInfo(bindingIndex);
            spriteRenderer.sprite = playerInputIcons.UpdateStaticIconImages(actionToBindTo, deviceLayoutName, controlPath);
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
