using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    // [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private UnityEngine.UI.Image characterPortrait;
    [SerializeField] private TMP_Text characterNameLabel;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // assign on the DialogueUI GameObject (no clip needed)

    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        // GetComponent<TypewriterEffect>().Run(textToType:"Go Stream ODYSSEY by RIIZE!\nIt's sure fire and addictive!!", textLabel);     // Start the typewriter effect with the specified text
        typewriterEffect = GetComponent<TypewriterEffect>();        // Get the TypewriterEffect component attached to the same GameObject
        responseHandler = GetComponent<ResponseHandler>();          // Get the ResponseHandler component attached to the same GameObject

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>(); // fallback

        CloseDialogueBox();
       // ShowDialogue(testDialogue);     // Show the test dialogue at the start
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);

        // NEW — Set the portrait and name from DialogueObject (if your DialogueObject stores them)
        if (characterPortrait != null && dialogueObject.CharacterPortrait != null)      // Check if the characterPortrait and CharacterPortrait are not null
            characterPortrait.sprite = dialogueObject.CharacterPortrait;        // Set the character portrait sprite

        if (characterNameLabel != null)     // Check if the characterNameLabel is not null
            characterNameLabel.text = dialogueObject.CharacterName;     // Set the character name text

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)   // Method to add response events to the response handler
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        // yield return new WaitForSeconds(2);  // Optional delay before starting the effect

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            // Start looping the voice clip for this DialogueObject
            if (audioSource != null && dialogueObject.VoiceSound != null)
            {
                audioSource.clip = dialogueObject.VoiceSound;
                audioSource.pitch = dialogueObject.VoicePitch;
                audioSource.loop = true; // loop during the line
                audioSource.Play();
            }

            yield return RunTypingEffect(dialogue); // Wait for the typing effect to complete

            // Stop looping once line is done typing
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.loop = false;
                audioSource.Stop();
            }

            textLabel.text = dialogue; // Ensure the full dialogue is displayed after typing effect

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.Responses != null && dialogueObject.HasResponses) break; // If it's the last line and there are responses, break to show responses

            yield return null; // Wait one frame before checking for input
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

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.IsRunning)
        {
            yield return null; // Wait until the typing effect is complete

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) // Allow skipping the typing effect
            {
                typewriterEffect.OnParticleSystemStopped(); // Stop the typing effect
            }
        }
    }

    public void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;  // Clear the text label when closing the dialogue box

        // Stop voice playback when dialogue closes
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        // NEW — Reset portrait & name
        if (characterNameLabel != null)
            characterNameLabel.text = string.Empty;

        if (characterPortrait != null)
            characterPortrait.sprite = null;
    }
}
