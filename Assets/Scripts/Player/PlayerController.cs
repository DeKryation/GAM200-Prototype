using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 7f;
    public float jumpImpulse = 8f;
    public float airWalkSpeed = 5f;

    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing;
    private float dashEndTime;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 2f; // cooldown in seconds
    private float lastDashTime = -999f;

    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    [SerializeField]
    private bool _isMoving = false;

    public float CurrentMoveSpeed
    {
        get
        {
            if (canMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        // Always use walkSpeed on ground
                        return walkSpeed;
                    }
                    else
                    {
                        // Air move speed
                        return airWalkSpeed;
                    }
                }
                else
                {
                    // Idle speed is 0
                    return 0f;
                }
            }
            else
            {
                return 0f;
            }
        }
    }

    public bool IsMoving
    {
        get => _isMoving;
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    public bool _isFacingRight = true;  // FLIP
    public bool IsFacingRight           // FLIP
    {
        get => _isFacingRight;
        private set
        {
            if (_isFacingRight != value)
            {
                Vector3 newScale = transform.localScale;
                newScale.x = Mathf.Abs(newScale.x) * (value ? 1 : -1);
                transform.localScale = newScale;
            }
            _isFacingRight = value;
        }
    }

    public bool canMove => animator.GetBool(AnimationStrings.canMove);
    public bool IsAlive => animator.GetBool(AnimationStrings.isAlive);

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            if (isDashing)
            {
                if (Time.time >= dashEndTime)
                    isDashing = false;
                // do not touch rb.linearVelocity at all keeps momentum
            }
            else
            {
                rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
            }
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (PauseMenu.GameIsPaused) return;

        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (PauseMenu.GameIsPaused) return;

        if (moveInput.x > 0 && !IsFacingRight)
        {
            // Right
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            // Left
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // TODO
        if (PauseMenu.GameIsPaused) return;

        if (context.started && touchingDirections.IsGrounded && canMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (PauseMenu.GameIsPaused) return;

        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && Time.time >= lastDashTime + dashCooldown && IsAlive)
        {
            float dashDir = IsFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDir * dashForce, rb.linearVelocity.y);

            isDashing = true;
            SoundManager.Instance.PlaySFX("PlayerDash");
            dashEndTime = Time.time + dashDuration;
            lastDashTime = Time.time;

            Debug.Log($"DASH PERFORMED at {Time.time}, direction: {(IsFacingRight ? "Right" : "Left")}");
            GetComponent<Adrenaline>()?.AddAdrenaline(50);
            Debug.Log("Adrenaline +50 from dash");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
        GetComponent<Adrenaline>()?.AddAdrenaline(-100);
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (PauseMenu.GameIsPaused) return;

        if (context.started && IsAlive)
        {
            // Do NOT set any animator trigger here.
            // Just ask ParryWindow to parry — it already checks cooldown.
            var parry = GetComponent<ParryWindow>();
            if (parry != null) parry.StartParry();
        }
    }
}