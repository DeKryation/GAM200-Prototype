using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[RequireComponent(typeof(Collider2D))]
public class DoorVictoryTrigger : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool oneShot = true;

    [Header("Cutscene")]
    [SerializeField] private VideoClip cutsceneClip;      // <- assign your MP4 (imported as VideoClip)
    [SerializeField] private float extraDelayAfter = 0.0f; // seconds to wait after it ends

    [Header("Events")]
    public UnityEvent onVictoryTriggered; // already points to VictoryScreenController.ShowVictoryScreen

    private bool _consumed;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed && oneShot) return;
        if (!other.CompareTag(playerTag)) return;

        _consumed = true;

        // Play cutscene overlay, then fire victory UI
        if (CutscenePlayer.Instance != null && cutsceneClip != null)
        {
            CutscenePlayer.Instance.Play(cutsceneClip, extraDelayAfter, () =>
            {
                onVictoryTriggered?.Invoke();
            });
        }
        else
        {
            // Fallback: no clip or player in scene -> just show victory
            onVictoryTriggered?.Invoke();
        }
    }
}
