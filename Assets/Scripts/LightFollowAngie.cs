using UnityEngine;

public class LightFollowAngie : MonoBehaviour
{
    private Rigidbody2D player;
    private bool isPlayerUnderground = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.GetComponent<Rigidbody2D>();
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        isPlayerUnderground = true;
    }

    void Update()
    {
        if (isPlayerUnderground) 
            transform.position = player.position;
    }
}
