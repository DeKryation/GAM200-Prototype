using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrenzyBar : MonoBehaviour
{
    public Slider frenzySlider;
    public TMP_Text frenzyText;

    private float maxFrenzyTime = 5f;
    private float currentFrenzyTime = 0f;
    private bool active = false;

    private void Start()
    {
        frenzySlider.minValue = 0f;
        frenzySlider.maxValue = 1f;
        frenzySlider.value = 0f;
        frenzyText.text = "";
    }

    public void ActivateFrenzy(float duration)
    {
        maxFrenzyTime = duration;
        currentFrenzyTime = duration;
        active = true;
        frenzySlider.value = 1f;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!active) return;

        currentFrenzyTime -= Time.deltaTime;
        frenzySlider.value = Mathf.Clamp01(currentFrenzyTime / maxFrenzyTime);

        if (currentFrenzyTime <= 0f)
        {
            active = false;
            gameObject.SetActive(false);
        }
    }
    public void UpdateRemainingFrenzy(float currentTime, float maxTime)
    {
        currentFrenzyTime = Mathf.Clamp(currentTime, 0f, maxTime);
        maxFrenzyTime = maxTime;
        frenzySlider.value = Mathf.Clamp01(currentFrenzyTime / maxFrenzyTime);
        gameObject.SetActive(true);
    }
}
