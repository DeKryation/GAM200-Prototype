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
        if (player != null)
            adrenaline = player.GetComponent<Adrenaline>();
    }

    private void Start()
    {
        if (adrenaline != null)
        {
            adrenalineSlider.value = Calculate(adrenaline.CurrentAdrenaline, 150);
            adrenalineText.text = "ADREN " + adrenaline.CurrentAdrenaline + " / 150";
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

    private float Calculate(float current, float max) => current / max;

    private void OnAdrenalineChanged(int newVal, int maxVal)
    {
        adrenalineSlider.value = Calculate(newVal, maxVal);
        adrenalineText.text = "ADREN " + newVal + " / " + maxVal;
    }
}
