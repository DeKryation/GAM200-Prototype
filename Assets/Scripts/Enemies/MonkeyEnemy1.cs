using Assets.Scripts;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class MonkeyEnemy1 : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;


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
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove)
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
}
