using UnityEngine;
using System.Collections;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private Collider2D hitbox;
    [SerializeField] private float activeTime = 0.12f; // fallback if you don't use animation events

    private void Reset()
    {
        hitbox = GetComponent<Collider2D>();
        if (hitbox) hitbox.enabled = false;
    }

    public void EnableHitbox() { if (hitbox) hitbox.enabled = true; }
    public void DisableHitbox() { if (hitbox) hitbox.enabled = false; }

    // Optional fallback if you don’t wire up animation events yet
    public IEnumerator Pulse()
    {
        EnableHitbox();
        yield return new WaitForSeconds(activeTime);
        DisableHitbox();
    }
}
