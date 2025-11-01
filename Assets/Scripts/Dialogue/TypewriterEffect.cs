using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 50f;  // To edit in unity for the Speed of the typewriter effect (characters per second)
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource blipAudioSource;                    // Plays blip sound
    [SerializeField] private AudioClip blipSound;                           // The sound to play per letter
    [SerializeField][Range(0f, 1f)] private float blipVolume = 0.4f;        // Volume of the blip sound
    [SerializeField] private int blipFrequency = 2;                         // Play sound every X letters (like an example of every 2nd letter)

    public bool IsRunning { private set; get; }

    private readonly List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.6f),      // Pause duration after sentence-ending punctuation)
        new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.3f)      // Pause duration after shorter pauses
    };

    private Coroutine typingCoroutine;

    public void Run(string textToType, TMP_Text textLabel)      // Responsible for running the couroutine of starting the typing effect.
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    public void OnParticleSystemStopped()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)      // Coroutine for typing the text out.
    {
        IsRunning = true;
        textLabel.text = string.Empty;  // Clear the text label at the start
     // yield return new WaitForSeconds(2);  // (THIS HAD BEEN INPUT IN THE DialogueUI SCRIPT INSTEAD) Optional delay before starting the effect
     
        float t = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;  // Store the last character index to check for punctuation

            t += Time.deltaTime * typewriterSpeed;  // Adjust the multiplier to change typing speed (the duration of the effect)

            charIndex = Mathf.FloorToInt(t);  // value of characters per second (value of the timer)
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for(int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1; // Check if it's the last character

                textLabel.text = textToType.Substring(0, i + 1);  // Update the text to include the new character


                // --- Play blip sound every few letters ---
                if (blipAudioSource != null && blipSound != null && (i % blipFrequency == 0))
                {
                    blipAudioSource.pitch = Random.Range(0.95f, 1.05f); // optional: slight pitch variation
                    blipAudioSource.PlayOneShot(blipSound, blipVolume);
                }


                // --- Check for punctuation and apply wait time ---
                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);  // Wait for the specified duration if it's a punctuation character
                }
            }

            yield return null;  // waits for one frame
        }

        IsRunning = false;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach(Punctuation punctuationCategory in punctuations) // Loop through each punctuation category
        {
            if (punctuationCategory.Punctuations.Contains(character))    // Check if the character is in the current category
            {
                waitTime = punctuationCategory.WaitTime;   // Set the wait time based on the category
                return true;    // Return true if it's a punctuation character
            }
        }
        waitTime = default;
        return false;
    }

    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;

        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}
