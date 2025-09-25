using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class Adrenaline : MonoBehaviour
{
    [SerializeField] private int maxAdrenaline = 150;
    [SerializeField] private int startAdrenaline = 75;
    [SerializeField] private float decreaseRate = 5f; // units per second when idle
    [SerializeField] private float increaseRate = 5f; // units per second when moving 
    [SerializeField] private bool isMoving = true; 

    private PlayerController playerController; //SPEED MODIFICATIOn
    public int CurrentAdrenaline { get; private set; }

    public UnityEvent<int, int> adrenalineChanged;

    private Animator animator;
    private int isMovingHash;

    private void Awake()
    {
        CurrentAdrenaline = startAdrenaline;
        animator = GetComponent<Animator>();
        isMovingHash = Animator.StringToHash(AnimationStrings.isMoving);
        playerController = GetComponent<PlayerController>(); //SPEEDMODIF
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
        }
        // speed boost check
        if (playerController != null)
        {
            if (CurrentAdrenaline > 2000)
            {
                playerController.walkSpeed = 8f;     // +2
                playerController.airWalkSpeed = 5f; // 
            }
            else if (CurrentAdrenaline <= 1000)
            {
                playerController.walkSpeed = 4f;     // -2
                playerController.airWalkSpeed = 5f; // 

            }
            else
            {
                playerController.walkSpeed = 6f;     // normal
                playerController.airWalkSpeed = 5f; // normal
            }
        }

    }
    public void GainAdrenalineOnAttack()
    {
        AddAdrenaline(50);                 //ADJUST ADRENALINE ON ATTACK HERE
    }
}
