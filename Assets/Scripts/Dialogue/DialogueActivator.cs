using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private GameObject interactIcon; // assign the icon prefab or object in Inspector
    [SerializeField] private AudioSource sfxSource;     // AudioSource to play SFX
    [SerializeField] private AudioClip sfxClip;     // SFX clip to play on interaction

    private bool playerInRange = false; // track if player is nearby (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)

    // START: Subscribe to DialogueUI event (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)
    private void OnEnable()
    {
        
        DialogueUI.OnDialogueClosed += HandleDialogueClosed;    // Listen for when dialogue closes
    }

    private void OnDisable()
    {
        
        DialogueUI.OnDialogueClosed -= HandleDialogueClosed;    // Stop listening when this object is disabled/destroyed
    }

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }
    // END: (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)

    public void DialogueObject(DialogueObject dialogueObject)   // Alternative method to set dialogue object
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
        // START: Hide icon when dialogue starts (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)
        if (interactIcon != null)
            interactIcon.SetActive(false);
        // END: (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)

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
    // START: Called automatically when DialogueUI closes (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)
    private void HandleDialogueClosed()
    {
        if (playerInRange && interactIcon != null)
        {
            interactIcon.SetActive(true);

            // Optional: play pop SFX again when reappearing
            if (sfxSource != null && sfxClip != null)
                sfxSource.PlayOneShot(sfxClip);
        }
    }
    // END: (Making the InteractIcon Reappear - remove the code if this doesn't work well with the concept)
}
