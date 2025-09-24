using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealthCollectible : MonoBehaviour
{
    public int healthRestore = 20;


    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            damageable.Heal(healthRestore);
            SoundManager.Instance.PlaySFX("HealthPickup");
            Destroy(gameObject);
        }
    }
}
