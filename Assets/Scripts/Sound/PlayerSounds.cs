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
        if (playerController.IsMoving && playerController.GetComponent<TouchingDirections>().IsGrounded)
        {
            stepTimer -= Time.deltaTime;

            // Adjust interval based on speed (higher speed = shorter interval)
            float speed = Mathf.Abs(playerController.CurrentMoveSpeed);
            float interval = baseStepInterval * (playerController.walkSpeed / Mathf.Max(speed, 0.1f));

            if (stepTimer <= 0f)
            {
                PlayFootstepSFX();
                stepTimer = interval;
            }
        }
        else
        {
            stepTimer = 0.1f;  // reset when idle
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
}
