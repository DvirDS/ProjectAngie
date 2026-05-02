using System.Collections;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;
    [SerializeField] private SceneFade fadeScreen;

    public Transform RespawnPoint { get; set; }

    [SerializeField] private float holdDuration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Transform player;

    private void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        instance = this;
    }

    public void Respawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return StartCoroutine(fadeScreen.FadeOutCoroutine(fadeDuration));
        player.position = RespawnPoint.position;
        yield return StartCoroutine(fadeScreen.HoldColorDuration(Color.black, holdDuration));
        yield return StartCoroutine(fadeScreen.FadeInCoroutine(fadeDuration));
    }
}