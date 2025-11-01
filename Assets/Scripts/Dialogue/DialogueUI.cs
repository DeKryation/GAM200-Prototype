using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static event Action OnDialogueClosed;

    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;

    [Header("Portraits")]
    [SerializeField] private UnityEngine.UI.Image leftPortrait;
    [SerializeField] private UnityEngine.UI.Image rightPortrait;
    [SerializeField] private TMP_Text leftCharacterNameLabel;
    [SerializeField] private TMP_Text rightCharacterNameLabel;
    [SerializeField] private float inactiveAlpha = 0.5f;
    [SerializeField] private float activeAlpha = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;        // Reference to the ResponseHandler component
    private TypewriterEffect typewriterEffect;      // Reference to the TypewriterEffect component
    private DialogueObject currentDialogueObject;       // To hold the current dialogue object for audio reference

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Start with portraits invisible
        SetPortraitAlpha(leftPortrait, 0f);
        SetPortraitAlpha(rightPortrait, 0f);

        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject)     // Main method to start showing dialogue
    {
        currentDialogueObject = dialogueObject;     // Store the current dialogue object
        
        IsOpen = true;
        dialogueBox.SetActive(true);

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)       // Method to add response events to the response handler
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)      // Coroutine to step through each line of dialogue
    {
        for (int i = 0; i < dialogueObject.DialogueLines.Length; i++)
        {
            var line = dialogueObject.DialogueLines[i];

            // Assign both portraits immediately
            if (leftPortrait != null)
            {
                leftPortrait.gameObject.SetActive(true);
                leftPortrait.sprite = line.isLeftSide ? line.speakerPortrait : line.otherPortrait;
                leftPortrait.color = new Color(1f, 1f, 1f, line.isLeftSide ? activeAlpha : inactiveAlpha);
            }

            if (rightPortrait != null)
            {
                rightPortrait.gameObject.SetActive(true);
                rightPortrait.sprite = line.isLeftSide ? line.otherPortrait : line.speakerPortrait;
                rightPortrait.color = new Color(1f, 1f, 1f, line.isLeftSide ? inactiveAlpha : activeAlpha);
            }

            // Update character names
            if (leftCharacterNameLabel != null)
                leftCharacterNameLabel.text = line.isLeftSide ? line.speakerName : "";
            if (rightCharacterNameLabel != null)
                rightCharacterNameLabel.text = !line.isLeftSide ? line.speakerName : "";

            // Start typing dialogue
            yield return RunTypingEffect(line.text);
            textLabel.text = line.text;

            // Wait for player input to continue
            yield return new WaitUntil(() =>
                Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)
            );
        }

        // Show responses or close dialogue
        if (dialogueObject.HasResponses)
            responseHandler.ShowResponses(dialogueObject.Responses);
        else
            CloseDialogueBox();
    }

    private IEnumerator RunTypingEffect(string dialogue)        // Coroutine to run the typewriter effect
    {
        typewriterEffect.Run(dialogue, textLabel);


        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                typewriterEffect.OnParticleSystemStopped();
        }
    }

    private void SetPortraitAlpha(UnityEngine.UI.Image portrait, float alpha)       // Helper method to set portrait alpha
    {
        if (portrait == null) return;
        var c = portrait.color;
        c.a = alpha;
        portrait.color = c;
    }

    public void CloseDialogueBox()      // Method to close the dialogue box and reset UI elements
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        // Reset portraits
        if (leftPortrait != null)
        {
            leftPortrait.sprite = null;
            leftPortrait.gameObject.SetActive(false);
        }
        if (rightPortrait != null)
        {
            rightPortrait.sprite = null;
            rightPortrait.gameObject.SetActive(false);
        }

        // Reset names
        if (leftCharacterNameLabel != null)
            leftCharacterNameLabel.text = string.Empty;
        if (rightCharacterNameLabel != null)
            rightCharacterNameLabel.text = string.Empty;

        // Reset alpha
        SetPortraitAlpha(leftPortrait, 0f);
        SetPortraitAlpha(rightPortrait, 0f);

        OnDialogueClosed?.Invoke();
    }
}





