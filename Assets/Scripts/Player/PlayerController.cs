using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    // START: This is to set active for the interaction for Dialogue.
    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    public IInteractable Interactable { get; set; }
    // END: 


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

    [Header("Attack")]
    [SerializeField] private float attackCooldownSeconds = 0.40f;
    private float _attackCooldownRemaining = 0f;
    public bool CanAttack => _attackCooldownRemaining <= 0f;

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
    // === Helpers (place inside the class) ===

    // Exact state names from your Animator. Change if yours differ.
    private const string State_Attack1 = "player_attack";
    private const string State_Attack2 = "player_attack_2";
    private const string State_Attack3 = "player_attack_3";
    [SerializeField] private bool _comboActive = false;
    [SerializeField] private int _comboStep = 0;
    [SerializeField] private bool _queuedNextAttack = false;
    [SerializeField] private float comboAdvanceThreshold = 0.98f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }
    private bool IsInAttackState()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName(State_Attack1) || info.IsName(State_Attack2) || info.IsName(State_Attack3) || info.IsTag("Attack");
    }
    private void Update()
    {
        // START: To Pause the game when the dialogue box is open.
        if (dialogueUI.IsOpen) return;
        // END:

        // START: This is to set active for the interaction for Dialogue.
        if (Input.GetKeyDown(KeyCode.E))
        {
                Interactable?.Interact(player: this);
        }
        // END:


        // existing cooldown ticking
        if (_attackCooldownRemaining > 0f)
        {
            _attackCooldownRemaining -= Time.unscaledDeltaTime;
            if (_attackCooldownRemaining < 0f) _attackCooldownRemaining = 0f;
        }

        animator.SetFloat(AnimationStrings.attackCooldown, _attackCooldownRemaining);

        // NEW: if we just left the combo, start the cooldown now
        HandleComboEndCooldown();
        TryAdvanceQueuedCombo();
    }
    private void TryAdvanceQueuedCombo()
    {
        if (!_queuedNextAttack) return;

        var st = animator.GetCurrentAnimatorStateInfo(0);
        // wait until the clip is basically done and not mid-blend
        if (st.normalizedTime < comboAdvanceThreshold || animator.IsInTransition(0)) return;

        if (st.IsName(State_Attack1))
        {
            animator.CrossFadeInFixedTime(State_Attack2, 0.05f, 0);
            _queuedNextAttack = false;
        }
        else if (st.IsName(State_Attack2))
        {
            animator.CrossFadeInFixedTime(State_Attack3, 0.05f, 0);
            _queuedNextAttack = false;
        }
        else
        {
            // left the combo path
            _queuedNextAttack = false;
        }
    }
    private void HandleComboEndCooldown()
    {
        if (!_comboActive) return;

        // We consider the combo "ended" when we're no longer in any attack state,
        // and we're not mid-transition between attack states.
        bool inTransition = animator.IsInTransition(0);
        if (!IsInAttackState() && !inTransition)
        {
            _attackCooldownRemaining = attackCooldownSeconds; // start post-combo cooldown
            _comboActive = false;
            _comboStep = 0;
        }
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

        // START: Prevent movement if dialogue is open
        if (dialogueUI != null && dialogueUI.IsOpen) 
        {
            moveInput = Vector2.zero;
            IsMoving = false;
            return;
        }
        // END:

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

        //START: Prevent jumping if dialogue is open
        if (dialogueUI != null && dialogueUI.IsOpen) return;
        //END:

        if (context.started && touchingDirections.IsGrounded && canMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (PauseMenu.GameIsPaused) return;

        if (context.started && IsAlive && canMove && (CanAttack || IsInAttackState()))
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);

            // --- CHAINING: queue next attack; we’ll hop when this clip finishes ---
            if (st.IsName(State_Attack1) || st.IsName(State_Attack2))
            {
                _queuedNextAttack = true;   // <<-- was CrossFade; now we queue
                return;                     // don't touch cooldown while chaining
            }

            // --- NEW COMBO START (your existing intent + cooldown) ---
            bool upIntent =
                moveInput.y > 0.5f
#if ENABLE_INPUT_SYSTEM
                || (Keyboard.current != null && Keyboard.current.wKey.isPressed)
#endif
                ;

            bool downIntent =
                moveInput.y < -0.5f
#if ENABLE_INPUT_SYSTEM
                || (Keyboard.current != null && Keyboard.current.sKey.isPressed)
#endif
                ;

            if (downIntent)
                animator.SetTrigger(Assets.Scripts.AnimationStrings.attackDownTrigger);
            else if (upIntent)
                animator.SetTrigger(Assets.Scripts.AnimationStrings.attackUpTrigger);
            else
                animator.SetTrigger(Assets.Scripts.AnimationStrings.attackTrigger);

            _attackCooldownRemaining = attackCooldownSeconds; // keep your current cooldown behavior
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
            GetComponent<Adrenaline>()?.AddAdrenaline(35);
            Debug.Log("Adrenaline +50 from dash");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
        GetComponent<Adrenaline>()?.AddAdrenaline(0);
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (PauseMenu.GameIsPaused) return;

        // START: Prevent parrying if dialogue is open
        if (dialogueUI != null && dialogueUI.IsOpen) return;
        // END:

        if (context.started && IsAlive)
        {
            // Do NOT set any animator trigger here.
            // Just ask ParryWindow to parry — it already checks cooldown.
            var parry = GetComponent<ParryWindow>();
            if (parry != null) parry.StartParry();
        }
    }
}