using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Ink.Runtime;
using UnityEngine.EventSystems;
using Ink.Parsed;
using static AreaHistory;
using static SiblingComponentUtils;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public DataManager dataManager;
    private QuestUpdater questUpdater;
    private GameController gameController;
    private DialogueItemDisplay DialogueItemDisplay { get; set; }
    [SerializeField] private WeaponDatabase weaponDatabase;
    private NarrativeItemsDatabase narrativeItemsDatabase;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;

    private TextMeshProUGUI[] choicesText;

    private string[] choicesTags;

    private Ink.Runtime.Story currentStory;
    private StoryHistory storyHistory;

    private bool destroyDialogueTriggerObject;
    private GameObject dialogueTriggerObject;

    public bool DialogueIsPlaying { get; set; }

    public bool choicesDisplayed = false;

    private static DialogueManager Instance { get; set; }

    private List<Ink.Runtime.Choice> currentChoices;
    [SerializeField] List<string> currentTags;

    [SerializeField] Morality playerMorality;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        Instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return Instance;
    }



    private void Start()
    {
        if (gameObject.GetSiblingComponent<SpriteGlowSupport>() != null)
        {
            Debug.Log("The name of Sprite Glow Support Effect's gameObject is: " + gameObject.GetSiblingComponent<SpriteGlowSupport>().gameObject);
        }
        
        dataManager = DataManager.Instance;
        
        DialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        //get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        playerMorality = GameObject.Find("Morality").GetComponent<Morality>();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoad; }

    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoad; }

    void OnSceneLoad(Scene scene, LoadSceneMode loadScene) // get references to objects in the new scene
    {
        gameController = FindObjectOfType<GameController>();
        questUpdater = FindObjectOfType<QuestUpdater>();
        weaponDatabase = FindObjectOfType<WeaponDatabase>();
        narrativeItemsDatabase = FindObjectOfType<NarrativeItemsDatabase>();
        DialogueItemDisplay = FindObjectOfType<DialogueItemDisplay>();
    }

    //return right away if dialogue isn't playing
    private void Update() { if (!DialogueIsPlaying) { return; } }

    public void EnterDialogueMode(TextAsset inkJSON, GameObject triggerObject, bool openAnInkStitch = false)
    {
        currentStory = new Ink.Runtime.Story(inkJSON.text);
        if (openAnInkStitch) { currentStory.ChoosePathString("path/to/your/inkScript.json"); }
        EnableQuestTracking(triggerObject);
        CheckIfSavePoint(triggerObject);
        CheckIfNewExperience(triggerObject);
        DialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }
    // used to update quest-related tracking objects by enabling functions from Ink
    private void EnableQuestTracking(GameObject triggerObject)
    {
        DialogueTrigger trigger = triggerObject.GetComponent<DialogueTrigger>();

        if (trigger.isPartOfAQuestActivity)
        {
            // when / if UpdateQuest is called from the Ink file
            currentStory.BindExternalFunction("UpdateQuest", () =>
            {
                Debug.Log("Checking quests!");
                // attempt to update the dialogue tracker in quest updater with relevant quest info
                for (int i = 0; i < trigger.questInfo.Length; i++)
                {
                    questUpdater.UpdateDialogueTracker(trigger.questInfo[i].questName,
                                                       trigger.questInfo[i].subQuestIndex,
                                                       trigger.questInfo[i].npcName);
                }
            }
            );  
        }
    }

    // used at save point stations, makes functions usable from Ink
    public void CheckIfSavePoint(GameObject triggerObject)
    {
        if (triggerObject.transform.parent != null && triggerObject.transform.parent.name.Contains("SaveGamePoint"))
        {
            currentStory.BindExternalFunction("SaveCurrent", () => dataManager.SaveData());
            currentStory.BindExternalFunction("SaveNew", (int fileNumber) => dataManager.SaveData(fileNumber));

            currentStory.BindExternalFunction("SeeIfFileHasBeenSavedBefore", () => dataManager.SeeIfFileHasBeenSavedBefore());
            currentStory.BindExternalFunction("GetSpecificFilePlayTime", (int fileNumber) => dataManager.GetFilePlayTimePrettyPrint(fileNumber));

            currentStory.BindExternalFunction("PlaySaveSound", () => FindObjectOfType<AudioManager>().PlaySFX("ItemPickup")); // PLACEHOLDER, Swap later for more apt sounds
            currentStory.BindExternalFunction("PlaySaveVFX", () => triggerObject.GetSiblingComponent<SpriteGlowSupport>().PlayGlow());
            currentStory.BindExternalFunction("PlaySaveVFX2", () => FindObjectOfType<PlayerVisualEffectsController>().PlayParticleSystem("PlayerSave"));
        }
    }

    // used when acquiring items for the first time
    void CheckIfNewExperience(GameObject triggerObject)
    {
        if(triggerObject.GetComponent<DialogueTrigger>() != null)
        {
            if (triggerObject.GetComponent<DialogueTrigger>().isNewExperience) // if it is a new experience
            {
                destroyDialogueTriggerObject = true; dialogueTriggerObject = triggerObject; // then store the object

                if (triggerObject.GetComponent<PickupableItem>().itemType == PickupableItem.ItemTypeOptions.Weapons) // if it's a weapon
                {
                    int weaponID = triggerObject.GetComponent<PickupableItem>().staticID; // get the weapon ID
                    currentStory.variablesState["itemDescription"] = weaponDatabase.ReturnItemFromID(weaponID).description; // and show the weapon description
                    DialogueItemDisplay.Display(true, weaponDatabase.ReturnItemFromID(weaponID).sprite, weaponDatabase.ReturnItemFromID(weaponID).name );
                }

                else if (triggerObject.GetComponent<PickupableItem>().itemType == PickupableItem.ItemTypeOptions.NarrativeItems)
                {
                    int narrativeItemID = triggerObject.GetComponent<PickupableItem>().staticID; // get the item ID
                    currentStory.variablesState["itemDescription"] = narrativeItemsDatabase.ReturnItemFromID(narrativeItemID).description; // and show the weapon description
                    DialogueItemDisplay.Display(true, narrativeItemsDatabase.ReturnItemFromID(narrativeItemID).sprite, narrativeItemsDatabase.ReturnItemFromID(narrativeItemID).name);
                }
            }
        }
    }


    public void ExitDialogueMode()
    {
        // if there is an issue with jumping do to jump / submit being the same button, turn this into an IEnumerator and have it wait for 0.2 seconds before continuing

        DialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        DialogueItemDisplay.Display(false, null, "");
        if (destroyDialogueTriggerObject) { Destroy(dialogueTriggerObject); destroyDialogueTriggerObject = false; }
    }


    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            //set text for current dialogue line
            dialogueText.text = currentStory.Continue();

            //display choices, if any, for this line
            DisplayChoices();
        }
        else if (!currentStory.canContinue && currentChoices.Count > 0)
        {
            Debug.Log("Do nothing here. Inside of else if.\n");
            Debug.Log("number of choices: " + currentChoices.Count);
        }
        else
        {
            ExitDialogueMode();
        }
    }


    private void DisplayChoices()
    {
        currentChoices = currentStory.currentChoices;

        //checks to make sure our UI can support the number of choices coming in.
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can Support. Number of choices given: " + currentChoices.Count);
        }

        int index = 0;
        //enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach (Ink.Runtime.Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            if(choice.tags != null)
            {
                foreach (string tag in choice.tags) { Debug.Log(("Choice at index " + index + " has a" + tag)); }
            }

            if(choice.tags != null)
            {
                foreach (object tagObj in choice.tags)
                {
                    string tag = tagObj.ToString();
                    currentTags.Add(tag);
                }
            }
            //
            index++;
        }

        // select the first choice if gamepad is the input method
        if(gameController.CurrentControlScheme == "Gamepad") { choices[0].GetComponent<Button>().Select(); }

        //go through the remaining choices the UI supports and make sure they're hidden
        for(int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        //StartCoroutine(SelectFirstChoice());
    }

    /*private IEnumerator SelectFirstChoice()
    {
        //event systems requires we clear it first,
        //then wait for at least one frame before we set the current selected object.   
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }*/

    public void MakeChoice(int choiceIndex)
    {
        //choicesDisplayed = true;
        Debug.Log(choicesText[choiceIndex].text);
        if (currentChoices[choiceIndex].tags != null)
        {
            if (currentChoices[choiceIndex].tags.Contains("positive")) { Debug.Log("Added morality"); playerMorality.AddToMoralityLevel(1); }
            else if (currentChoices[choiceIndex].tags.Contains("negative")) { Debug.Log("Subtracted morality"); playerMorality.AddToMoralityLevel(-1); }
        }
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
}
