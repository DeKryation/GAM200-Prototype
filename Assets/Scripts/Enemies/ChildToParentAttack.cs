using UnityEngine;

public class ChildToParentAttack : MonoBehaviour
{
    // Option A: drag specific Attack(s) in the Inspector
    [SerializeField] private Attack[] attacks;

    // Option B: auto-find all Attack components under this Animator
    [SerializeField] private bool autoFindChildren = true;

    private void Awake()
    {
        if (autoFindChildren || attacks == null || attacks.Length == 0)
        {
            // includeInactive = true so disabled weapon objects are found too
            attacks = GetComponentsInChildren<Attack>(true);
        }
    }

    // ===== Animation Events call these =====
    // Put these names in your attack animation clip events
    public void AE_ActivateAttackHitbox()
    {
        if (attacks == null) return;
        foreach (var a in attacks)
            if (a) a.ActivateHitbox();
    }

    public void AE_DeactivateAttackHitbox()
    {
        if (attacks == null) return;
        foreach (var a in attacks)
            if (a) a.DeactivateHitbox();
    }

    // Optional: if you use different clips with different weapons,
    // you can pass an int/string parameter from the event and filter here.
    public void AE_ActivateAttackHitboxByIndex(int attackIndex)
    {
        if (attacks == null || attackIndex < 0 || attackIndex >= attacks.Length) return;
        attacks[attackIndex]?.ActivateHitbox();
    }
}
