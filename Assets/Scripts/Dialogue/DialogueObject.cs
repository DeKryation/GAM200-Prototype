using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]     // Create a DialogueObject asset via the Unity Editor menu
public class DialogueObject : ScriptableObject
{
    [SerializeField] private DialogueLine[] dialogueLines;      // Array of dialogue lines for the conversation
    [SerializeField] private Response[] responses;      // Array of possible player responses
    [SerializeField] private AudioClip voiceSound;      // Audio clip for voice lines
    [SerializeField] private float voicePitch = 1f;     // Pitch adjustment for the voice audio

    public DialogueLine[] DialogueLines => dialogueLines;   // Property to access the dialogue lines
    public Response[] Responses => responses;       // Property to access the responses
    public bool HasResponses => Responses != null && Responses.Length > 0;  // Property to check if there are any responses available.

    public AudioClip VoiceSound => voiceSound;  // Expose the audio clip
    public float VoicePitch => voicePitch;  // Expose the pitch adjustment
}

// Put DialogueLine **after or above** this class
[System.Serializable]
public struct DialogueLine
{
    [TextArea] public string text;
    public string speakerName;
    public Sprite speakerPortrait;
    public Sprite otherPortrait;
    public bool isLeftSide;
}




// DialgoueObject original version: For Single Character dialogue
/* using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField][TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;
    // For Audio Source to play voice lines, can be added.
    [SerializeField] private AudioClip voiceSound;
    [SerializeField] private float voicePitch = 1f;

    public string[] Dialogue => dialogue;

    public bool HasResponses => Responses != null && Responses.Length > 0;      // Property to check if there are any responses available.

    public Response[] Responses => responses;

    // Expose the audio fields
    public AudioClip VoiceSound => voiceSound;
    public float VoicePitch => voicePitch;

    [Header("Voice Settings")]
    public string voiceSFXName;   // The name of the SFX to play (matches SoundLibrary name)
    public bool loopVoice = true; // Whether the voice should loop during typing

    public string CharacterName;
    public Sprite CharacterPortrait;
} */
