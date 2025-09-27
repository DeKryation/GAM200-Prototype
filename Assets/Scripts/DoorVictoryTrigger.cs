using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DoorVictoryTrigger : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool oneShot = true;

    [Header("Events")]
    public UnityEvent onVictoryTriggered;

    private bool _consumed;

    private void Reset()
    {
        // Make sure collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed && oneShot) return;
        if (!other.CompareTag(playerTag)) return;

        _consumed = true;
        onVictoryTriggered?.Invoke();
    }
}
