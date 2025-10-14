using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    // [SerializeField] private DialogueObject testDialogue;

    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        // GetComponent<TypewriterEffect>().Run(textToType:"Go Stream ODYSSEY by RIIZE!\nIt's sure fire and addictive!!", textLabel);     // Start the typewriter effect with the specified text
        typewriterEffect = GetComponent<TypewriterEffect>();        // Get the TypewriterEffect component attached to the same GameObject
        responseHandler = GetComponent<ResponseHandler>();          // Get the ResponseHandler component attached to the same GameObject

        CloseDialogueBox();
       // ShowDialogue(testDialogue);     // Show the test dialogue at the start
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        // yield return new WaitForSeconds(2);  // Optional delay before starting the effect

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, textLabel);     // Start the typewriter effect for the current dialogue line to end

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.Responses != null && dialogueObject.HasResponses) break; // If it's the last line and there are responses, break to show responses

            yield return new WaitUntil(() =>
            Input.GetKeyDown(KeyCode.Space) ||  // Wait until the player presses the 'Space' key to continue
            Input.GetMouseButtonDown(0) // or player presses the left mouse button to continue
            );
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);    // Show responses if available
        }
        else
        {
            CloseDialogueBox();     // Close the dialogue box if there are no responses
        }
    }

    private void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;  // Clear the text label when closing the dialogue box
    }
}
