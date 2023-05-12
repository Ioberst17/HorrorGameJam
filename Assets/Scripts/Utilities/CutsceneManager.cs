using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private float moveSpeed = 5.0f;

    private InputAction moveAction;

    private Coroutine moveCoroutine;

    private void Start()
    {
        //moveAction = playerControls.Player.Move;
    }

    private void CutsceneScripts()
    {

    }

    public void MovePlayer(Vector2 position)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveToPosition(position));
    }

    private IEnumerator MoveToPosition(Vector2 position)
    {
        // Calculate the direction and distance to the target position
        Vector2 direction = (position - (Vector2)playerCharacter.transform.position).normalized;
        float distance = Vector2.Distance(position, playerCharacter.transform.position);

        // Keep moving until the player reaches the target position
        while (distance > 0.01f)
        {
            // Use the input system to simulate input in the direction of the target position
            Vector2 input = direction * moveSpeed * Time.deltaTime;
            //PlayerInputSingleton.Instance.MoveInput = input;

            // Update the position and distance to the target position
            playerCharacter.transform.position += (Vector3)input;
            distance = Vector2.Distance(position, playerCharacter.transform.position);

            yield return null;
        }

        // Stop simulating input when the player reaches the target position
        //PlayerInputSingleton.Instance.MoveInput = Vector2.zero;
    }
}
