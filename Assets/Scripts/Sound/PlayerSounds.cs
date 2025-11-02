using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private PlayerController playerController;
    private TouchingDirections touching;
    private float stepTimer = 0f;
    private float baseStepInterval = 0.4f; // base at normal walk speed

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        touching = GetComponent<TouchingDirections>();
    }

    private void Update()
    {
        if (playerController.IsMoving && touching.IsGrounded)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstepSFX();
                stepTimer = baseStepInterval; // fixed 0.4s, ignore speed scaling for now
            }
        }
    }

    public void PlayAttackSFX()
    {
        SoundManager.Instance.PlaySFX("PlayerAttackBasic");
    }

    public void PlayFootstepSFX()
    {
        // randomize between multiple footstep clips if you want variety
        SoundManager.Instance.PlaySFX("PlayerFootstep");
    }

    public void PlayDashSFX()
    {
        SoundManager.Instance.PlaySFX("PlayerDash");
    }

    public void PlayParrySFX()
    {
        SoundManager.Instance.PlaySFX("PlayerParry");
    }
    public void PlayJumpSFX()
    {
        SoundManager.Instance.PlaySFX("PlayerJump");
    }

    public void PlayKenaHitSFX()
    {
        SoundManager.Instance.PlaySFX("PlayerHitSuccess");
    }
}
