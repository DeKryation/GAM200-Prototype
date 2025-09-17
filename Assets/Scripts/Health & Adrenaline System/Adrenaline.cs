using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class Adrenaline : MonoBehaviour
{
    [SerializeField] private int maxAdrenaline = 150;
    [SerializeField] private int startAdrenaline = 75;
    [SerializeField] private float decreaseRate = 5f; // units per second when idle
    [SerializeField] private float increaseRate = 5f; // units per second when moving 

    public int CurrentAdrenaline { get; private set; }

    public UnityEvent<int, int> adrenalineChanged;

    private Animator animator;
    private int isMovingHash;

    private void Awake()
    {
        CurrentAdrenaline = startAdrenaline;
        animator = GetComponent<Animator>();
        isMovingHash = Animator.StringToHash(AnimationStrings.isMoving);
    }

    private void Update()
    {
        if (animator == null) return;

        bool isMoving = animator.GetBool(isMovingHash);

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
    }
    public void GainAdrenalineOnAttack()
    {
        AddAdrenaline(50);                 //ADJUST ADRENALINE ON ATTACK HERE
    }
}
