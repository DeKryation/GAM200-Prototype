using Assets.Scripts;
using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;
    public UnityEvent<int, int> healthChanged;
    [SerializeField] private string deathFloorTag = "DeathFloor";

    Animator animator;

    [SerializeField]
    private int _maxHealth;
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool IsInvincible = false;

    private float timeSinceHit = 0;

    [SerializeField]
    private float invincibilityTime = 0.25f;

    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            if (!value) damageableDeath?.Invoke();
        }
    }

    //IMPT: DONT CHANGE VELOCITY WHILE VELOCITY IS LOCKED, TAKEN FROM PLAYER CONTROLLER
    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                IsInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
    }



    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !IsInvincible)
        {
            Health -= damage;
            IsInvincible = true;


            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);


            return true;
        }

        return false;
    }

    public void Heal(int healthRestore)
    {
        if (IsAlive)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            CharacterEvents.characterHealed(gameObject, healthRestore);
        }
    }

    public void Kill()
    {
        if (!IsAlive) return;
        Health = 0;                // setter will flip IsAlive and fire events
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag(deathFloorTag)) Kill();
    }

}
