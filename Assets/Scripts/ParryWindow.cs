using Assets.Scripts;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ParryWindow : MonoBehaviour
{
    [Header("Parry Tuning")]
    [SerializeField] private float windowDuration = 0.18f;   // active parry window
    [SerializeField] private float stunSeconds = 3f;         // enemy immobilize time

    [Header("Cooldown")]
    [SerializeField] private float parryCooldown = 3f;       // cooldown seconds
    private float _lastParryTime = -999f;
    public bool IsOnCooldown => Time.time < _lastParryTime + parryCooldown;
    public float CooldownRemaining =>
        Mathf.Max(0f, (_lastParryTime + parryCooldown) - Time.time);

    [Header("Refs")]
    [SerializeField] private Collider2D parryHitbox;         // disabled by default
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Animator Params")]
    // Trigger is already in AnimationStrings.parryTrigger
    // This bool is used to reflect "can parry now?" for transitions/UI.
    [SerializeField] private string canParryBool = "canParry";

    public bool IsParrying { get; private set; }

    private Animator animator;
    private int _canParryHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (parryHitbox != null) parryHitbox.enabled = false;

        _canParryHash = Animator.StringToHash(canParryBool);

        // start allowed (unless you want to begin on cooldown)
        if (_canParryHash != 0)
            animator.SetBool(_canParryHash, true);
    }

    private void Update()
    {
        // Keep animator in sync: true only when NOT on cooldown and NOT already parrying
        if (_canParryHash != 0)
            animator.SetBool(_canParryHash, !IsOnCooldown);
    }

    public void StartParry()
    {
        if (IsParrying) return;

        if (IsOnCooldown)
        {
            // Do NOT set the trigger while on cooldown
            Debug.Log($"Parry on cooldown: {CooldownRemaining:0.00}s left");
            return;
        }

        _lastParryTime = Time.time; // start cooldown now

        IsParrying = true;
        if (parryHitbox) parryHitbox.enabled = true;

        // fire the parry animation state
        animator.SetTrigger(AnimationStrings.parryTrigger);

        StartCoroutine(CloseWindowAfter(windowDuration));
    }

    private IEnumerator CloseWindowAfter(float t)
    {
        yield return new WaitForSeconds(t);

        IsParrying = false;

        if (parryHitbox)
        {
            parryHitbox.enabled = false;
            Debug.Log("Parry OFF: hitbox disabled");
        }

        // We don't flip canParry true here; Update() will set it to true
        // automatically once the cooldown has expired.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsParrying) return;

        var attack = other.GetComponent<Attack>() ?? other.GetComponentInParent<Attack>();
        if (attack == null) return;

        var enemyRoot = attack.transform.root;
        if (!enemyRoot.CompareTag(enemyTag)) return;

        attack.Neutralize();

        var monkey = enemyRoot.GetComponent<MonkeyEnemy1>();
        if (monkey != null)
        {
            monkey.OnStunned(stunSeconds);
        }
        else
        {
            var enemyAnimator = enemyRoot.GetComponent<Animator>();
            if (enemyAnimator != null)
                StartCoroutine(ImmobilizeEnemy(enemyAnimator, stunSeconds));
        }
    }

    private IEnumerator ImmobilizeEnemy(Animator enemyAnimator, float seconds)
    {
        enemyAnimator.SetBool(AnimationStrings.canMove, false);
        var rb = enemyAnimator.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(seconds);
        enemyAnimator.SetBool(AnimationStrings.canMove, true);
    }

    public void OnSuccessfulParry(Attack attack)
    {
        if (!IsParrying) return;

        attack.Neutralize();

        var enemyRoot = attack.transform.root;
        var monkey = enemyRoot.GetComponent<MonkeyEnemy1>();
        if (monkey != null)
        {
            monkey.OnStunned(stunSeconds);
        }
        else
        {
            var enemyAnimator = enemyRoot.GetComponent<Animator>();
            if (enemyAnimator != null)
                StartCoroutine(ImmobilizeEnemy(enemyAnimator, stunSeconds));
        }
    }
}
