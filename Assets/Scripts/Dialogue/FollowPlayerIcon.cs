using UnityEngine;

public class FollowPlayerIcon : MonoBehaviour
{
    public Transform player;       // drag the player GameObject here
    public Vector3 offset = new Vector3(0, 2, 0); // height above the player

    private void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}
