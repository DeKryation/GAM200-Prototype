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

    private List<GameObject> tempResponseButtons = new List<GameObject>();  // List to keep track of temporary response buttons for cleanup.

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        responseButtonTemplate.gameObject.SetActive(false); // Ensure template is hidden
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;    // Variable to keep track of the total height of the response box.

        foreach (Response response in responses)
        {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);  // Instantiate a new button from the template.
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;   // Set the button text to the response text.
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));    // Add listener for button click event, don't have to manually input in the inspector "On click ()" part because it's done in code.

            tempResponseButtons.Add(responseButton); // Add the button to the list of temporary buttons for later cleanup.

            responseBoxHeight += responseButtonTemplate.sizeDelta.y; // Increment the total height by the height of the button template.
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight); // Set the height of the response box to fit all buttons.
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response)    // Method to handle the event when a response is picked.
    {
        responseBox.gameObject.SetActive(false);   // Hide the response box.

        foreach (GameObject button in tempResponseButtons)   // Clean up all temporary buttons.
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        dialogueUI.ShowDialogue(response.DialogueObject);   // Show the dialogue associated with the picked response.
    }
}
