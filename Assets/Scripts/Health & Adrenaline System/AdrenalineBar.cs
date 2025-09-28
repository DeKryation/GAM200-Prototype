using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdrenalineBar : MonoBehaviour
{
    public Slider adrenalineSlider;
    public TMP_Text adrenalineText;

    private Adrenaline adrenaline;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        adrenaline = player ? player.GetComponent<Adrenaline>() : null;

        if (adrenaline == null)
        {
            gameObject.SetActive(false); // no adrenaline source? hide bar
            return;
        }

        adrenalineSlider.minValue = 0f;
        adrenalineSlider.maxValue = 1f;
        adrenalineSlider.value = Calculate(adrenaline.CurrentAdrenaline, adrenaline.MaxAdrenaline);
        adrenalineText.text = "ADREN " + adrenaline.CurrentAdrenaline + " / " + adrenaline.MaxAdrenaline;
    }

    private void Start()
    {
        if (adrenaline != null)
        {
            // use runtime max instead of hardcoded 150
            adrenalineSlider.value = Calculate(adrenaline.CurrentAdrenaline, adrenaline.MaxAdrenaline);
            adrenalineText.text = "ADREN " + adrenaline.CurrentAdrenaline + " / " + adrenaline.MaxAdrenaline;
        }
    }

    private void OnEnable()
    {
        if (adrenaline != null)
            adrenaline.adrenalineChanged.AddListener(OnAdrenalineChanged);
    }

    private void OnDisable()
    {
        if (adrenaline != null)
            adrenaline.adrenalineChanged.RemoveListener(OnAdrenalineChanged);
    }

    private float Calculate(float current, float max) => max <= 0 ? 0 : current / max;

    private void OnAdrenalineChanged(int newVal, int maxVal)
    {
        adrenalineSlider.value = Calculate(newVal, maxVal);
        adrenalineText.text = "HEARTRATE " + newVal + " / " + maxVal;

        // Change bar color based on thresholds
        Image fillImage = adrenalineSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            if (newVal >= 2000)
            {
                fillImage.color = Color.red;  // high adrenaline
            }
            else if (newVal <= 1000)
            {
                fillImage.color = new Color(0.5f, 0.8f, 1f); // light blue
            }
            else
            {
                fillImage.color = Color.yellow; // neutral
            }
        }
    }
}
