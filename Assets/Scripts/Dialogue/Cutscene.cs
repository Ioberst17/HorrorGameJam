using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cutscene.CutsceneActivity;
using Ink.Parsed;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class Cutscene : MonoBehaviour
{
    // outside references
    private GameController gameController;
    private PlayerController playerController;
    private PlayerAnimator playerAnimator;
    private DialogueManager dialogueManager;
    private ScreenFade screenFade;
    private ObjectiveUI objectiveUI;

    // the cutscenes activities
    public List<CutsceneActivity> activities = new List<CutsceneActivity>();

    // internal states
    [SerializeField] private int currentActivityIndex = 0;
    [SerializeField] public bool hasPlayed;
    private IEnumerator currentCoroutine;
    public int minimumObjectiveID;

    // supporting classes
    [System.Serializable]
    public class MoveCutsceneActivity
    {
        [Tooltip("Add a target object for the player to run towards")] public GameObject targetObject;
        [Tooltip("Use this to adjust left (negative) or right (positive) in 'X' position units from the target object if needed")] public float XDirectionAdjustment;
    }

    [System.Serializable]
    public class InteractCutsceneActivity { public float optionalPreInteractWaitTime; }

    [System.Serializable]
    public class WaitCutsceneActivity { public float waitTime; }

    [System.Serializable]
    public class TriggerDialogueCutsceneActivity { public bool useFile; public TextAsset fileToUse; }


    [System.Serializable]
    public class TransportCutsceneActivity { public GameObject targetObject; }


    [System.Serializable]
    public class CutsceneActivity
    {
        public enum ActivityType { Move, Interact, Wait, TriggerDialogue, Transport }

        public ActivityType activityType;
        public InteractCutsceneActivity interactActivity;
        public MoveCutsceneActivity moveActivity;
        public WaitCutsceneActivity waitActivity;
        public TriggerDialogueCutsceneActivity triggerDialogueActivity;
        public TransportCutsceneActivity transportActivity;
    }

    // get references
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        playerController = FindObjectOfType<PlayerController>();
        playerAnimator = FindObjectOfType<PlayerAnimator>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        GetComponent<BoxCollider2D>().isTrigger = true;
        screenFade = FindObjectOfType<ScreenFade>();
        objectiveUI = FindObjectOfType<ObjectiveUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision) // trigger cutscene when player enters trigger box
    {
        if (CutsceneConditions() && collision.gameObject.tag == "Player" && !hasPlayed) { Play(); }
    }

    bool CutsceneConditions() { if (minimumObjectiveID >= objectiveUI.AreaNum) { return true; } else { return false; } }

    // Start playing the cutscene
    public void Play()
    {
        gameController.IsCutscene = true; // to disable all non-UI player input
        currentActivityIndex = 0;
        PlayCurrentActivity();
    }

    // Stop playing the cutscene
    public void Finished()
    {
        hasPlayed = true;
        gameController.IsCutscene = false;
    }

    // Move to the next activity in the sequence
    private void PlayNextActivity()
    {
        // currentCoroutine.MoveNext() check to see if the coroutine is running, while != null checks to see it is assigned
        if (currentCoroutine != null && currentCoroutine.MoveNext())
            return;

        if (currentActivityIndex < activities.Count)
        {
            PlayCurrentActivity();
            currentActivityIndex++;
        }
        else { Finished(); }
    }

    // Play the current activity in the sequence
    private void PlayCurrentActivity()
    {
        if (currentActivityIndex < activities.Count)
        {
            CutsceneActivity currentActivity = activities[currentActivityIndex];

            switch (currentActivity.activityType)
            {
                case ActivityType.Move:
                    // Move the player character
                    MoveCutsceneActivity moveActivity = currentActivity.moveActivity;
                    MovePlayer(moveActivity.targetObject, moveActivity.XDirectionAdjustment);
                    break;
                case ActivityType.Interact:
                    // Interact with an object
                    InteractCutsceneActivity interactActivity = currentActivity.interactActivity;
                    InteractWithObject(interactActivity.optionalPreInteractWaitTime);
                    break;
                case ActivityType.TriggerDialogue:
                    // Trigger a dialogue sequence
                    TriggerDialogueCutsceneActivity dialogueActivity = currentActivity.triggerDialogueActivity;
                    TriggerDialogue(dialogueActivity.useFile, dialogueActivity.fileToUse);
                    break;
                case ActivityType.Transport:
                    // Transport the player to another location
                    TransportCutsceneActivity transportActivity = currentActivity.transportActivity;
                    TransportPlayer(transportActivity.targetObject);
                    break;
                case ActivityType.Wait:
                    // Wait a certain amount of time
                    WaitCutsceneActivity waitActivity = currentActivity.waitActivity;
                    Wait(waitActivity.waitTime);
                    break;
                default:
                    // Unknown activity type
                    Debug.LogWarning("Unknown activity type: " + currentActivity.activityType);
                    PlayNextActivity();
                    break;
            }
        }
        else
        {
            Finished();
        }
    }

    // ACTIVITY COROUTINES
    // Move the player character to a target position
    private void MovePlayer(GameObject targetObject, float adjustmentInXDirection)
    {
        StartCoroutine(MovePlayerCoroutine(targetObject, adjustmentInXDirection));
    }

    private IEnumerator MovePlayerCoroutine(GameObject targetObject, float adjustmentInXDirection)
    {
        yield return StartCoroutine(MovePlayerToPositionCoroutine(targetObject.transform.position, adjustmentInXDirection));

        Debug.Log("Just reached target object: " + targetObject);

        currentActivityIndex++;
        PlayCurrentActivity();
    }

    private IEnumerator MovePlayerToPositionCoroutine(Vector3 targetPosition, float adjustmentInXDirection)
    {
        while (playerController.MovePlayerToPosition(targetPosition, adjustmentInXDirection)) { yield return null; }
    }


    // Interact with an object
    private void InteractWithObject(float waitTime) { StartCoroutine(InteractWithObjectCoroutine(waitTime)); }

    private IEnumerator InteractWithObjectCoroutine(float waitTime)
    {
        gameController.TriggerInteractButton();
        Debug.Log("Interacted with object");

        yield return null;

        currentActivityIndex++;
        PlayCurrentActivity();
    }

    // Wait for a certain amount of time
    private void Wait(float timeToWait) { StartCoroutine(WaitCoroutine(timeToWait)); }

    private IEnumerator WaitCoroutine(float timeToWait)
    {
        float timeElapsed = 0;

        while (timeElapsed <= timeToWait)
        {
            playerController.SetVelocity();
            gameController.XInput = 0;
            gameController.YInput = 0;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Just waited for a block of seconds: " + timeToWait);

        currentActivityIndex++;
        PlayCurrentActivity();
    }

    // Trigger a dialogue sequence
    private void TriggerDialogue(bool useFile, TextAsset fileToUse) { StartCoroutine(TriggerDialogueCoroutine(useFile, fileToUse)); }

    private IEnumerator TriggerDialogueCoroutine(bool useFile, TextAsset fileToUse)
    {
        StartCoroutine(IdlePlayer());
        if (useFile)
        {
            DialogueManager.GetInstance().EnterDialogueMode(fileToUse, this.gameObject);
        }
        else // assume that player is meant to trigger an interaction using interact
        {
            if (!dialogueManager.DialogueIsPlaying) { gameController.TriggerInteractButton(); }
        }
        
        while (dialogueManager.DialogueIsPlaying) { yield return null; }

        currentActivityIndex++;
        PlayCurrentActivity();
    }

    IEnumerator IdlePlayer()
    {
        // while any of these are not true
        while (!gameController.PlayerInputIdle() ||  // check if XInput and YInput are 0
                    !playerAnimator.CheckIfAnimationIsPlaying("PlayerIdle") || // Check if player idle animation is playing
                                    playerController.RB.velocity.x != 0)  // check if X motion is 0
        { gameController.ResetPlayerMotionAndInput(); yield return null; } // attempt to reset motion
    }

    // Transport the player to a target position
    private void TransportPlayer(GameObject targetObject) { StartCoroutine(TransportPlayerCoroutine(targetObject.transform.position)); }

    private IEnumerator TransportPlayerCoroutine(Vector3 targetPosition)
    {
        Debug.Log("Attempting to transport player");

        screenFade.FadeToBlack();
        yield return new WaitForSeconds(1f); // Wait for the screen to fade to black
        playerController.transform.position = targetPosition;

        screenFade.FadeFromBlack();
        yield return new WaitForSeconds(1f); // Wait for the screen to fade from black

        currentActivityIndex++;
        PlayCurrentActivity();
    }

#if UNITY_EDITOR
    /// <summary>
    /// EDITOR CODE FOR EASE OF USE, STRICTLY GUI RELATED BELOW
    /// </summary>

    [CustomEditor(typeof(Cutscene))]
    public class CutsceneSequenceEditor : Editor
    {
        SerializedProperty activities;
        ReorderableList reorderableList;
        private Cutscene cutscene;

        private InputActionAsset editorActions;
        private InputAction rightClickAction;

        private void OnEnable()
        {
            cutscene = (Cutscene)target;
            activities = serializedObject.FindProperty("activities");

            reorderableList = new ReorderableList(serializedObject, activities, true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Activities");
                },
                drawElementCallback = OnListElementGUI,
                elementHeightCallback = index =>
                {
                    float offset = 4f; // Add this offset value to add space between elements for padding and visibility
                    return GetElementHeight(index) + offset * 2; // Add offset * 2 to the element's height to have space at the top and bottom
                },

                elementHeight = 0
            };
        }

        private float GetElementHeight(int index)
        {
            SerializedProperty activity = activities.GetArrayElementAtIndex(index);
            SerializedProperty activityType = activity.FindPropertyRelative("activityType");

            float propertyHeight = EditorGUI.GetPropertyHeight(activityType);

            switch ((ActivityType)activityType.enumValueIndex)
            {
                case ActivityType.Interact:
                    SerializedProperty interactActivity = activity.FindPropertyRelative("interactActivity");
                    propertyHeight += EditorGUI.GetPropertyHeight(interactActivity, GUIContent.none, true);
                    break;
                case ActivityType.Move:
                    SerializedProperty moveActivity = activity.FindPropertyRelative("moveActivity");
                    propertyHeight += EditorGUI.GetPropertyHeight(moveActivity, GUIContent.none, true);
                    break;
                case ActivityType.Wait:
                    SerializedProperty waitActivity = activity.FindPropertyRelative("waitActivity");
                    propertyHeight += EditorGUI.GetPropertyHeight(waitActivity, GUIContent.none, true);
                    break;
                case ActivityType.TriggerDialogue:
                    SerializedProperty triggerDialogueActivity = activity.FindPropertyRelative("triggerDialogueActivity");
                    propertyHeight += EditorGUI.GetPropertyHeight(triggerDialogueActivity, GUIContent.none, true);
                    break;
                case ActivityType.Transport:
                    SerializedProperty transportActivity = activity.FindPropertyRelative("transportActivity");
                    propertyHeight += EditorGUI.GetPropertyHeight(transportActivity, GUIContent.none, true);
                    break;
            }

            return propertyHeight;
        }

        private void OnListElementGUI(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty activity = activities.GetArrayElementAtIndex(index);
            SerializedProperty activityType = activity.FindPropertyRelative("activityType");

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), activityType, GUIContent.none);

            rect.y += EditorGUIUtility.singleLineHeight;

            float offset = 2f; // Add this offset value to add space between elements for padding and visibility
            rect.y += offset; // Add the offset to the current y position

            switch ((ActivityType)activityType.enumValueIndex)
            {
                case ActivityType.Interact:
                    SerializedProperty interactActivity = activity.FindPropertyRelative("interactActivity");
                    EditorGUI.PropertyField(rect, interactActivity, GUIContent.none, true);
                    break;
                case ActivityType.Wait:
                    SerializedProperty waitActivity = activity.FindPropertyRelative("waitActivity");
                    EditorGUI.PropertyField(rect, waitActivity, GUIContent.none, true);
                    break;
                case ActivityType.Move:
                    SerializedProperty moveActivity = activity.FindPropertyRelative("moveActivity");
                    EditorGUI.PropertyField(rect, moveActivity, GUIContent.none, true);
                    break;
                case ActivityType.TriggerDialogue:
                    SerializedProperty triggerDialogueActivity = activity.FindPropertyRelative("triggerDialogueActivity");
                    EditorGUI.PropertyField(rect, triggerDialogueActivity, GUIContent.none, true);
                    break;
                case ActivityType.Transport:
                    SerializedProperty transportActivity = activity.FindPropertyRelative("transportActivity");
                    EditorGUI.PropertyField(rect, transportActivity, GUIContent.none, true);
                    break;
            }
        }

        // Store the current activities index when the serialized object is applied
        private void OnDisable()
        {
            cutscene.currentActivityIndex = reorderableList.index;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasPlayed"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumObjectiveID"));

            reorderableList.DoLayoutList();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentActivityIndex"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}