using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    private MonkeyEnemy1 playerController;
    private TouchingDirections touching;

    private void Awake()
    {
        playerController = GetComponent<MonkeyEnemy1>();
        touching = GetComponent<TouchingDirections>();
    }

    private void Update()
    {

    }

    public void PlayEnemyHitSFX()
    {
        SoundManager.Instance.PlaySFX("PlayerHitSuccess");
    }

    public void PlayEnemyAttackSFX()
    {
        SoundManager.Instance.PlaySFX("PlayerAttackBasic");
    }



}
