using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents; // Array to hold response events, if needed in future.

    private List<GameObject> tempResponseButtons = new List<GameObject>();  // List to keep track of temporary response buttons for cleanup.

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        responseButtonTemplate.gameObject.SetActive(false); // Ensure template is hidden
    }

    public void AddResponseEvents(ResponseEvent[] events)
    {
        this.responseEvents = events;   // Use the parameter, not the field
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;    // Variable to keep track of the total height of the response box.

        for (int i = 0; i < responses.Length; i++)   // Loop through each response to create a button.
        {
            Response response = responses[i];
            int responseIndex = i;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);  // Instantiate a new button from the template.
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;   // Set the button text to the response text.
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));    // Add listener for button click event, don't have to manually input in the inspector "On click ()" part because it's done in code.

            tempResponseButtons.Add(responseButton); // Add the button to the list of temporary buttons for later cleanup.

            responseBoxHeight += responseButtonTemplate.sizeDelta.y; // Increment the total height by the height of the button template.
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight); // Set the height of the response box to fit all buttons.
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex)    // Method to handle the event when a response is picked.
    {
        responseBox.gameObject.SetActive(false);   // Hide the response box.

        foreach (GameObject button in tempResponseButtons)   // Clean up all temporary buttons.
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        // Trigger the UnityEvent for the picked response, if available
        if (responseEvents != null && responseIndex >= 0 && responseIndex < responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        responseEvents = null; // Clear the response events after one use to avoid unintended reuse.

        if(response.DialogueObject)
        {
            dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            dialogueUI.CloseDialogueBox(); // Close the dialogue box if there's no follow-up dialogue.
        }
    }
}
