using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Damageable))]
public class Adrenaline : MonoBehaviour
{
    [SerializeField] private int maxAdrenaline = 3000;
    [SerializeField] private int startAdrenaline = 75;
    [SerializeField] private float decreaseRate = 5f; // units per second when idle
    [SerializeField] private float increaseRate = 5f; // units per second when moving 
    [SerializeField] private bool isMoving = true;

    [SerializeField] private FrenzyBar frenzyBar; // assign via Inspector
    private bool isInFrenzy = false;
    [SerializeField]
    private float frenzyDuration = 5f;
    private float frenzyTimer = 0f;

    [Header("Death Rules")]
    [SerializeField] private bool dieOnZero = true;
    [SerializeField] private int deathThreshold = 3000;
    public float DamageMultiplier { get; private set; } = 1f;

    private PlayerController playerController; //SPEED MODIFICATIOn
    public int CurrentAdrenaline { get; private set; }
    public int MaxAdrenaline => maxAdrenaline;

    public UnityEvent<int, int> adrenalineChanged;

    private Animator animator;
    private int isMovingHash;

    private Damageable damageable;

    private float _fractionCarry = 0f;

    private void Awake()
    {
        CurrentAdrenaline = startAdrenaline;
        animator = GetComponent<Animator>();
        isMovingHash = Animator.StringToHash(AnimationStrings.isMoving);
        playerController = GetComponent<PlayerController>(); //SPEEDMODIF
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        if (isInFrenzy)
        {
            frenzyTimer -= Time.deltaTime;
            if (frenzyTimer <= 0f)
            {
                EndFrenzy();
            }
            return; // skip normal adrenaline drain while frenzied
        }

        // START: Prevent Adrenaline logic if dialogue is open
        if (playerController != null && playerController.DialogueUI != null && playerController.DialogueUI.IsOpen)
            return;
        // END: 

        if (animator == null) return;

        isMoving = animator.GetBool(isMovingHash);

        if (isMoving)
            IncreaseOverTime();
        else
            DecreaseOverTime();
    }

    // --- Public API to replace HP ---
    public int TakeDamage(int amount)
    {
        if (amount <= 0) return 0;
        int before = CurrentAdrenaline;
        ModifyAdrenaline(-amount);
        return Mathf.Max(0, before - CurrentAdrenaline);
    }

    public int HealAdrenaline(int amount)
    {
        if (amount <= 0) return 0;
        int before = CurrentAdrenaline;
        ModifyAdrenaline(+amount);
        return Mathf.Max(0, CurrentAdrenaline - before);
    }

    public void AddAdrenaline(int amount)
    {
        ModifyAdrenaline(amount);
    }

    private void IncreaseOverTime()
    {
        ModifyAdrenaline(increaseRate * Time.deltaTime);
    }

    private void DecreaseOverTime()
    {
        ModifyAdrenaline(-decreaseRate * Time.deltaTime);
    }

    private void ModifyAdrenaline(float amount)
    {
        _fractionCarry += amount;

        // Convert ONLY the whole-number portion into an int delta, keep the fraction for later
        int wholeDelta = (int)_fractionCarry; // truncates toward 0 (works for pos & neg)
        if (wholeDelta != 0)
        {
            _fractionCarry -= wholeDelta;

            int newVal = Mathf.Clamp(CurrentAdrenaline + wholeDelta, 0, maxAdrenaline);
            if (newVal != CurrentAdrenaline)
            {
                CurrentAdrenaline = newVal;
                adrenalineChanged?.Invoke(CurrentAdrenaline, maxAdrenaline);

                if (CurrentAdrenaline >= deathThreshold)
                {
                    if (!isInFrenzy)       // avoid multiple triggers
                        TriggerFrenzy();
                    return;
                }

                // Death rules (only check on actual integer changes)
                //if ((dieOnZero && CurrentAdrenaline <= 0) || CurrentAdrenaline >= deathThreshold)
                    //KillPlayer();
            }

            // Speed & damage tiers (also only recompute when value actually changed)
            if (playerController != null)
            {
                if (CurrentAdrenaline > 2000)
                {
                    playerController.walkSpeed = 8f;
                    playerController.airWalkSpeed = 6f;
                    DamageMultiplier = 2f;   // double damage
                }
                else if (CurrentAdrenaline <= 1000)
                {
                    playerController.walkSpeed = 4f;
                    playerController.airWalkSpeed = 4f;
                    DamageMultiplier = 0.5f; // half damage
                }
                else
                {
                    playerController.walkSpeed = 6f;
                    playerController.airWalkSpeed = 5f;
                    DamageMultiplier = 1f;   // normal damage
                }
            }
        }
        // If wholeDelta == 0, we changed by <1 this frame keep accumulating quietly.
    }
    private void TriggerFrenzy()
    {
        isInFrenzy = true;
        frenzyTimer = frenzyDuration;
        frenzyBar?.ActivateFrenzy(frenzyDuration);

        // Optional buff effects
        playerController.walkSpeed *= 1.5f;
        playerController.airWalkSpeed *= 1.5f;
        DamageMultiplier = 3f;

        Debug.Log("Frenzy activated!");
    }

    private void EndFrenzy()
    {
        isInFrenzy = false;
        CurrentAdrenaline = maxAdrenaline - 1; // prevent instant re-trigger
        DamageMultiplier = 1f;
        playerController.walkSpeed = 6f;
        playerController.airWalkSpeed = 5f;

        Debug.Log("Frenzy ended!");
    }
    public bool IsInFrenzy => isInFrenzy;

    public void ExtendFrenzy(float extraSeconds)
    {
        // add to remaining time, but clamp to the configured max duration
        frenzyTimer = Mathf.Min(frenzyTimer + extraSeconds, frenzyDuration);

        // just visually update the bar without restarting it
        frenzyBar?.UpdateRemainingFrenzy(frenzyTimer, frenzyDuration);

        Debug.Log($"Frenzy extended by {extraSeconds:F1}s! Remaining: {frenzyTimer:F1}s");
    }

    private void KillPlayer()
    {
        if (damageable != null && damageable.IsAlive)
        {
            // Trigger existing death pipeline (animations, events, etc.)
            damageable.Health = 0;
        }
    }
    public void GainAdrenalineOnAttack()
    {
        AddAdrenaline(50);                 //ADJUST ADRENALINE ON ATTACK HERE
    }
}
