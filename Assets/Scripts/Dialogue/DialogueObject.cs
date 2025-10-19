using UnityEngine;

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

    public string CharacterName;
    public Sprite CharacterPortrait;
}
