using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private Collider2D _col;
    private bool _neutralized;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    // If your Attack component or its GameObject gets toggled on/off per swing,
    // this auto-resets when it turns back on.
    private void OnEnable()
    {
        ResetForNewSwing();
    }

    public void ResetForNewSwing()
    {
        _neutralized = false;
        if (_col) _col.enabled = false;   // keep off until active frames
    }

    public void ActivateHitbox()          // call via animation event on first active frame
    {
        _neutralized = false;
        if (_col) _col.enabled = true;
    }

    public void DeactivateHitbox()        // call via animation event on last active frame
    {
        if (_col) _col.enabled = false;
    }

    public void Neutralize()              // called by parry
    {
        _neutralized = true;
        if (_col) _col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_neutralized) return;

        // --- Parry handling ---
        var parry = collision.GetComponent<ParryWindow>()
                    ?? collision.GetComponentInParent<ParryWindow>();
        if (parry != null && parry.IsParrying)
        {
            parry.OnSuccessfulParry(this);
            return;
        }

        // --- Damage handling ---
        var damageable = collision.GetComponent<Damageable>()
                         ?? collision.GetComponentInParent<Damageable>();
        if (damageable != null)
        {
            // Apply adrenaline multiplier if present
            int finalDamage = attackDamage;
            var adrenaline = GetComponentInParent<Adrenaline>();
            if (adrenaline != null)
            {
                if (adrenaline.CurrentAdrenaline > 2000)
                {
                    finalDamage = Mathf.RoundToInt(attackDamage * 2f); // boosted
                }
                else if (adrenaline.CurrentAdrenaline <= 1000)
                {
                    finalDamage = Mathf.RoundToInt(attackDamage * 0.5f); // reduced
                }
            }

            Vector2 deliveredKnockback =
                transform.parent.localScale.x > 0
                    ? knockback
                    : new Vector2(-knockback.x, -knockback.y);

            // Check if the target actually took damage
            bool gotHit = damageable.Hit(finalDamage, deliveredKnockback);

            // Extend Frenzy if hit landed during frenzy
            if (gotHit && adrenaline != null && adrenaline.IsInFrenzy)
            {
                float extraTime = 1.5f; // how much Frenzy time to add per successful hit
                adrenaline.ExtendFrenzy(extraTime);
            }
        }
    }
}