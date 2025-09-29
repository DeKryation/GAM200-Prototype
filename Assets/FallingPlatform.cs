using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallWait = 2f;     // Time before platform falls
    public float respawnWait = 3f;  // Time before platform respawns
    public float destroyWait = 1f;

    bool isFalling;
    Rigidbody2D rb;
    Vector3 originalPosition;
    Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Wait for the platform to fall and then respawn it
        yield return new WaitForSeconds(destroyWait);
        StartCoroutine(RespawnPlatform());
    }

    private IEnumerator RespawnPlatform()
    {
        // Wait for respawn delay
        yield return new WaitForSeconds(respawnWait);

        // Rset platform properties
        ResetPlatform();
    }

    private void ResetPlatform()
    {
        // Stop any movement
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Reset position and rotation
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // Reset falling state
        isFalling = false;
    }
}
