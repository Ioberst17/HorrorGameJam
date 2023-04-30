using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using Ink.Parsed;
using static AreaHistory;
using static SiblingComponentUtils;


public class DialogueManager : MonoBehaviour
{
    public DataManager dataManager;

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

    public bool dialogueIsPlaying;

    public bool choicesDisplayed = false;

    private static DialogueManager instance;

    private List<Ink.Runtime.Choice> currentChoices;
    [SerializeField] List<string> currentTags;

    [SerializeField] Morality playerMorality;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }



    private void Start()
    {
        if (gameObject.GetSiblingComponent<SpriteGlowSupport>() != null)
        {
            Debug.Log("The name of Sprite Glow Support Effect's gameObject is: " + gameObject.GetSiblingComponent<SpriteGlowSupport>().gameObject);
        }
        
        dataManager = DataManager.Instance;
        dialogueIsPlaying = false;
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



    private void Update()
    {
        //return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        //handle continuing to the next line in the dialogue when return/enter is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ContinueStory();
        }
    }



    public void EnterDialogueMode(TextAsset inkJSON, GameObject triggerObject)
    {
        currentStory = new Ink.Runtime.Story(inkJSON.text);
        CheckIfSavePoint(triggerObject);
        CheckIfNewExperience(triggerObject);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

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
        }
    }

    void CheckIfNewExperience(GameObject triggerObject)
    {
        if (triggerObject.GetComponent<DialogueTrigger>().isNewExperience) 
        {
            destroyDialogueTriggerObject = true; dialogueTriggerObject = triggerObject;
            if(triggerObject.GetComponent<PickupableItem>().itemType == PickupableItem.ItemTypeOptions.Weapons)
            {
                int weaponID = triggerObject.GetComponent<PickupableItem>().staticID;
                currentStory.variablesState["weaponDescription"] = FindObjectOfType<WeaponDatabase>().ReturnItemFromID(weaponID).description;
            }
        }
    }

    public void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
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
                foreach (string tag in choice.tags)
                {
                    Debug.Log(("Choice at index " + index + " has a" + tag));
                }
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
