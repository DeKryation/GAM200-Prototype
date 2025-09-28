using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthCollectible : MonoBehaviour
{
    public int healthRestore = -200;


    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var adren = collision.GetComponent<Adrenaline>() ?? collision.GetComponentInParent<Adrenaline>();
        if (adren != null)
        {
            int healed = adren.HealAdrenaline(healthRestore);
            if (healed > 0)
                Assets.Scripts.Events.CharacterEvents.characterHealed?.Invoke(collision.gameObject, healed);
            Destroy(gameObject);
            return;
        }

        var damageable = collision.GetComponent<Damageable>() ?? collision.GetComponentInParent<Damageable>();
        if (damageable != null)
        {
            damageable.Heal(healthRestore); // enemies or non-adrenaline actors
            Destroy(gameObject);
        }
    }
}
