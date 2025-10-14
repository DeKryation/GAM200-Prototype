using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f;  // To edit in unity for the Speed of the typewriter effect (characters per second)
    public Coroutine Run(string textToType, TMP_Text textLabel)      // Responsible for running the couroutine of starting the typing effect.
    {
        return StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)      // Coroutine for typing the text out.
    {
        textLabel.text = string.Empty;  // Clear the text label at the start
     // yield return new WaitForSeconds(2);  // (THIS HAD BEEN INPUT IN THE DialogueUI SCRIPT INSTEAD) Optional delay before starting the effect
     
        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            t += Time.deltaTime * typewriterSpeed;  // Adjust the multiplier to change typing speed (the duration of the effect)
            charIndex = Mathf.FloorToInt(t);  // value of characters per second (value of the timer)
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            textLabel.text = textToType.Substring(0, charIndex);  // Sets the text to be displayed up to the current character index

            yield return null;  // waits for one frame
        }

        textLabel.text = textToType;  // Ensures the full text is displayed at the end of the effect
    }
}
