using System.Collections.Generic;
using UnityEngine;

public class LightbeamRotation : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] float maxLightbeamAngle = 65.0f;
    [SerializeField] float speed = 1.0f;

    [Header("Sprites to Unlit")]
    [SerializeField] List<SpriteRenderer> spritesToUnlit;
    [SerializeField] float switchAngle = 64f;

    [Header("Player Scoping")]
    [Tooltip("The player's sprite is only toggled while they're actually inside this room's bounds. " +
             "Without this, the player (a persistent object shared across rooms) keeps flickering between " +
             "layers here even while still in an adjacent room, because the previous room's scene can still " +
             "be loaded (and this script already running) for a moment during the additive scene-load handoff.")]
    [SerializeField] private Collider2D roomBounds;

    private SpriteRenderer player;

    private bool lightBackwards = true;
    private string unlitLayer = "Unlit";
    private string defaultLayer = "Default";

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<SpriteRenderer>();
        spritesToUnlit.Add(player);
    }

    void FixedUpdate()
    {
        float angle = maxLightbeamAngle * Mathf.Sin(Time.time * speed);
        transform.rotation = Quaternion.Euler(0, 0, -180 + angle);

        bool playerInRoom = roomBounds == null || (player != null && roomBounds.bounds.Contains(player.transform.position));

        if(angle > switchAngle && lightBackwards)
        {
            foreach (SpriteRenderer sprite in spritesToUnlit)
            {
                if (sprite == player && !playerInRoom) continue;
                sprite.sortingLayerName = unlitLayer;
            }
            lightBackwards = false;
        }
        else if(angle < -switchAngle && !lightBackwards)
        {
            foreach (SpriteRenderer sprite in spritesToUnlit)
            {
                if (sprite == player && !playerInRoom) continue;
                sprite.sortingLayerName = defaultLayer;
            }
            lightBackwards = true;
        }

        // Safety net: if the player left the room mid-cycle while still switched to Unlit,
        // restore them immediately instead of leaving them stuck on the wrong layer.
        if (!playerInRoom && player != null && player.sortingLayerName == unlitLayer)
        {
            player.sortingLayerName = defaultLayer;
        }
    }
}
