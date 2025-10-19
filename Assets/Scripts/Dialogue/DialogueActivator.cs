using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private GameObject interactIcon; // assign the icon prefab or object in Inspector
    [SerializeField] private AudioSource sfxSource;     // AudioSource to play SFX
    [SerializeField] private AudioClip sfxClip;     // SFX clip to play on interaction

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
               this.dialogueObject = dialogueObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.Interactable = this;

            // Show the icon when in range
            if (interactIcon != null)
                interactIcon.SetActive(true);

            // --- Play SFX here ---
            if (sfxSource != null && sfxClip != null)
            {
                sfxSource.PlayOneShot(sfxClip);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;

                // Hide the icon when leaving range
                if (interactIcon != null)
                    interactIcon.SetActive(false);
            }
        }
    }

    public void Interact(PlayerController player)
    {
        /* if (TryGetComponent(out DialogueResponseEvents responseEvents) && responseEvents.DialogueObject == dialogueObject)
        {
            player.DialogueUI.AddResponseEvents(responseEvents.Events);
        } */
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }
            player.DialogueUI.ShowDialogue(dialogueObject);
    }
}
