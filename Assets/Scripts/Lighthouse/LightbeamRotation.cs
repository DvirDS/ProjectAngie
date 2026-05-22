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

        if(angle > switchAngle && lightBackwards)
        {
            foreach (SpriteRenderer sprite in spritesToUnlit)
                sprite.sortingLayerName = unlitLayer;
            lightBackwards = false;
        }
        else if(angle < -switchAngle && !lightBackwards)
        {
            foreach (SpriteRenderer sprite in spritesToUnlit)
                sprite.sortingLayerName = defaultLayer;
            player.sortingLayerName = defaultLayer;
            lightBackwards = true;
        }
    }
}
