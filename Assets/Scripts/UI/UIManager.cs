using Assets.Scripts.Events;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;

    public Canvas gameCanvas;

    private void Awake()
    {
        GameObject canvasObj = GameObject.Find("WorldCanvas");

        if (canvasObj != null)
        {
            gameCanvas = canvasObj.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogError("Canvas with the given name not found in the scene!");
        }
    }

        private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
    }


    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        // Spawn a little above the character's head
        Vector3 spawnPosition = character.transform.position + Vector3.up * 2f;

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();

        // Optional: Make the text face the camera
        tmpText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        // Offset spawn position slightly above the character's head
        Vector3 spawnPosition = character.transform.position + Vector3.up * 2f;

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        // Set the healing amount text
        tmpText.text = "+" + healthRestored.ToString();

        // Optional: make healing text green to distinguish it
        tmpText.color = Color.green;

        tmpText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

}
