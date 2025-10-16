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
    [SerializeField] private string canParryBool = "canParry";

    [SerializeField] private GameObject parryFxPrefab;
    [SerializeField] private Vector2 fxLocalOffset = new Vector2(0.3f, 0.6f);
    [SerializeField] private float fxLifetime = 0.35f;

    public bool IsParrying { get; private set; }

    private bool _parryConsumed = false;          // play once per window
    private int _lastAttackId = 0;

    private Animator animator;
    private int _canParryHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (parryHitbox != null) parryHitbox.enabled = false;

        _canParryHash = Animator.StringToHash(canParryBool);

        if (_canParryHash != 0)
            animator.SetBool(_canParryHash, true);
    }

    private void Update()
    {
        if (_canParryHash != 0)
            animator.SetBool(_canParryHash, !IsOnCooldown);
    }

    private void SpawnParryFX()
    {
        if (!parryFxPrefab) return;

        float dir = Mathf.Sign(transform.localScale.x == 0 ? 1 : transform.localScale.x);
        Vector3 worldPos = transform.position + new Vector3(fxLocalOffset.x * dir, fxLocalOffset.y, 0f);

        var go = Instantiate(parryFxPrefab, worldPos, Quaternion.identity);
        Destroy(go, fxLifetime);
    }

    public void StartParry()
    {
        if (IsParrying) return;

        if (IsOnCooldown)
        {
            Debug.Log($"Parry on cooldown: {CooldownRemaining:0.00}s left");
            return;
        }

        _lastParryTime = Time.time;
        _parryConsumed = false;
        _lastAttackId = 0;
        IsParrying = true;
        if (parryHitbox) parryHitbox.enabled = true;

        animator.SetTrigger(AnimationStrings.parryTrigger);
        StartCoroutine(CloseWindowAfter(windowDuration));
    }

    private void EndParryEarly()
    {
        IsParrying = false;
        if (parryHitbox) parryHitbox.enabled = false;
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsParrying || _parryConsumed) return;

        var attack = other.GetComponent<Attack>() ?? other.GetComponentInParent<Attack>();
        if (attack == null) return;

        int id = attack.GetInstanceID();
        if (_lastAttackId == id) return;

        var enemyRoot = attack.transform.root;
        if (!enemyRoot.CompareTag(enemyTag)) return;

        _parryConsumed = true;
        _lastAttackId = id;
        attack.Neutralize();

        SpawnParryFX();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("ParrySuccess");

        var monkey = enemyRoot.GetComponent<MonkeyEnemy1>();
        if (monkey != null) monkey.OnStunned(stunSeconds);
        else
        {
            var enemyAnimator = enemyRoot.GetComponent<Animator>();
            if (enemyAnimator != null)
                StartCoroutine(ImmobilizeEnemy(enemyAnimator, stunSeconds));
        }

        // Frenzy extension here
        var adrenaline = GetComponentInParent<Adrenaline>();
        if (adrenaline != null && adrenaline.IsInFrenzy)
        {
            adrenaline.ExtendFrenzy(6f); // add +6 seconds
            Debug.Log("Frenzy extended by +6s from Parry!");
        }

        EndParryEarly();
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
        if (!IsParrying || _parryConsumed || attack == null) return;

        _parryConsumed = true;
        _lastAttackId = attack.GetInstanceID();
        attack.Neutralize();
        SpawnParryFX();
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySFX("ParrySuccess");

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

        // Frenzy extension here too (in case some enemies trigger via OnSuccessfulParry)
        var adrenaline = GetComponentInParent<Adrenaline>();
        if (adrenaline != null && adrenaline.IsInFrenzy)
        {
            adrenaline.ExtendFrenzy(6f);
            Debug.Log("Frenzy extended by +6s from Parry!");
        }

        EndParryEarly();
    }
}
