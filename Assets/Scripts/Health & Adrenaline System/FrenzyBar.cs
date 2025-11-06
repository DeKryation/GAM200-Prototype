using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class FrenzyBar : MonoBehaviour
{
    [Header("UI")]
    public Slider frenzySlider;
    public TMP_Text frenzyText;

    [Header("Refs (optional)")]
    [SerializeField] private PlayerController playerController; // assign in inspector or auto-find

    private float maxFrenzyTime = 5f;
    private float currentFrenzyTime = 0f;
    private bool active = false;

    private void Awake()
    {
        // Auto-find player/controller if not wired
        if (playerController == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerController = player.GetComponent<PlayerController>();
        }
    }

    private void Start()
    {
        if (frenzySlider != null)
        {
            frenzySlider.minValue = 0f;
            frenzySlider.maxValue = 1f;
            frenzySlider.value = 0f;
        }
        if (frenzyText != null) frenzyText.text = "";
        gameObject.SetActive(false); // hidden until activated
    }

    // Helper: is dialogue open?
    private bool DialogueIsOpen()
    {
        return playerController != null
            && playerController.DialogueUI != null
            && playerController.DialogueUI.IsOpen;
    }

    public void ActivateFrenzy(float duration)
    {
        maxFrenzyTime = duration;
        currentFrenzyTime = duration;
        active = true;

        if (frenzySlider) frenzySlider.value = 1f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!active) return;

        // Pause countdown while dialogue is open
        if (DialogueIsOpen()) return;

        currentFrenzyTime -= Time.deltaTime;
        if (frenzySlider) frenzySlider.value = Mathf.Clamp01(currentFrenzyTime / maxFrenzyTime);

        if (currentFrenzyTime <= 0f)
        {
            active = false;
            gameObject.SetActive(false);
        }
    }

    // External updater (e.g., ExtendFrenzy)
    public void UpdateRemainingFrenzy(float currentTime, float maxTime)
    {
        currentFrenzyTime = Mathf.Clamp(currentTime, 0f, maxTime);
        maxFrenzyTime = maxTime;
        if (frenzySlider) frenzySlider.value = Mathf.Clamp01(currentFrenzyTime / maxFrenzyTime);
        gameObject.SetActive(true);
    }
}
