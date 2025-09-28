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
        int newVal = Mathf.Clamp(Mathf.RoundToInt(CurrentAdrenaline + amount), 0, maxAdrenaline);
        if (newVal != CurrentAdrenaline)
        {
            CurrentAdrenaline = newVal;
            adrenalineChanged?.Invoke(CurrentAdrenaline, maxAdrenaline);

            if (dieOnZero && CurrentAdrenaline <= 0)
                KillPlayer();
            else if (CurrentAdrenaline >= deathThreshold)
                KillPlayer();
        }


        // speed boost check
        if (playerController != null)
        {
            if (CurrentAdrenaline > 2000)
            {
                playerController.walkSpeed = 8f;
                playerController.airWalkSpeed = 6f;
                DamageMultiplier = 2f;  // double damage
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
                DamageMultiplier = 1f; // normal damage
            }
        }

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
