using Assets.Scripts;
using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class MonkeyEnemy1 : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    private Coroutine stunRoutine;
    private bool _isStunned;

    [SerializeField]
    private float postStunAttackDelay = 2f;

    private Attack[] _attacks;                // NEW: cache attacks to disable during stun

    public bool IsStunned => _isStunned;


    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkFaceDirection { Right, Left }

    public WalkFaceDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkFaceDirection WalkDirection
    {
        get => _walkDirection;
        set
        {
            if (_walkDirection == value) return;

            _walkDirection = value;  // <-- IMPORTANT: persist the new value

            // update move vector
            walkDirectionVector = (_walkDirection == WalkFaceDirection.Right)
                ? Vector2.right
                : Vector2.left;

            // set scale explicitly (no cumulative *= -1)
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (_walkDirection == WalkFaceDirection.Right ? 1 : -1);
            transform.localScale = s;
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();

        _attacks = GetComponentsInChildren<Attack>(true);   // NEW
        animator.SetBool(AnimationStrings.canAttack, true); // NEW: allow attacks by default
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove && !_isStunned)
                rb.linearVelocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.linearVelocity.y);
            else
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, walkStopRate), rb.linearVelocity.y);
        }

    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkFaceDirection.Right)
        {
            WalkDirection = WalkFaceDirection.Left;
        }
        else if (WalkDirection == WalkFaceDirection.Left)
        {
            WalkDirection = WalkFaceDirection.Right;
        }
        else
        {
            Debug.LogError("no go left or right");
        }
    }
    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

        if (_isStunned) animator.ResetTrigger(AnimationStrings.attackTrigger);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    public void OnCliffEdgeDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

    // --- STUN API -------------------------------------------------------- // 
    public void OnStunned(float seconds)
    {
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunCo(seconds));
    }

    private IEnumerator StunCo(float seconds)
    {
        _isStunned = true;

        _isStunned = true;
        animator.SetBool(AnimationStrings.canMove, false);
        animator.SetBool(AnimationStrings.canAttack, false);   // <-- blocks transition
        animator.ResetTrigger(AnimationStrings.attackTrigger);  // clear pending attack
                                                                // disable Attack scripts/colliders if you added that too

        // stop motion & locomotion
        if (rb) rb.linearVelocity = Vector2.zero;
        animator.SetBool(AnimationStrings.canMove, false);

        // **block animation transitions into attack**
        animator.SetBool(AnimationStrings.canAttack, false);              // gate transitions
        animator.ResetTrigger(AnimationStrings.attackTrigger);            // clear any pending trigger

        // **disable all attack scripts & their colliders**
        if (_attacks == null || _attacks.Length == 0)
            _attacks = GetComponentsInChildren<Attack>(true);

        foreach (var a in _attacks)
        {
            if (!a) continue;
            a.DeactivateHitbox();
            var col = a.GetComponent<Collider2D>();
            if (col) col.enabled = false;
            a.enabled = false;
        }

        // also push back any internal cooldown
        AttackCooldown = Mathf.Max(AttackCooldown, seconds + postStunAttackDelay);

        yield return new WaitForSeconds(seconds);

        animator.SetBool(AnimationStrings.canMove, true);
        animator.SetBool(AnimationStrings.canAttack, true);

        // unfreeze locomotion
        animator.SetBool(AnimationStrings.canMove, true);

        // allow attack animations again (after the post-stun delay)
        animator.SetBool(AnimationStrings.canAttack, true);

        // re-enable attack scripts (hitboxes will be reopened later by their animation events)
        foreach (var a in _attacks)
            if (a) a.enabled = true;

        // optional extra delay before next attack
        if (postStunAttackDelay > 0f)
            AttackCooldown = Mathf.Max(AttackCooldown, postStunAttackDelay);

        _isStunned = false;
    }
}
