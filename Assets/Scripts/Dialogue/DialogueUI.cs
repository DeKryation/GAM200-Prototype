using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;

    private TypewriterEffect typewriterEffect;

    private void Start()
    {
        // GetComponent<TypewriterEffect>().Run(textToType:"Go Stream ODYSSEY by RIIZE!\nIt's sure fire and addictive!!", textLabel);     // Start the typewriter effect with the specified text
        typewriterEffect = GetComponent<TypewriterEffect>();        // Get the TypewriterEffect component attached to the same GameObject
        CloseDialogueBox();
        ShowDialogue(testDialogue);     // Show the test dialogue at the start
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        // yield return new WaitForSeconds(2);  // Optional delay before starting the effect

        foreach (string dialogue in dialogueObject.Dialogue)
        {
            yield return typewriterEffect.Run(dialogue, textLabel);     // Start the typewriter effect for the current dialogue line to end
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));  // Wait until the player presses the space key to continue
        }

        CloseDialogueBox();
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;  // Clear the text label when closing the dialogue box
    }
}
