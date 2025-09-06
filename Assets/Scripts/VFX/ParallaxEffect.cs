using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    private Vector2 startingPosition;
    private float startingZ;

    private void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    private void Update()
    {
        Vector2 camMoveSinceStart = (Vector2)cam.transform.position - startingPosition;

        // Simple multiplier based on z-depth
        float parallaxFactor = Mathf.Abs(startingZ - followTarget.position.z) / 10f;

        Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
