using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    [SerializeField] private SceneFade fadeScreen;
    [SerializeField] private float holdDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.5f;

    public Transform RespawnPoint { get; set; }

    private bool isRespawning = false;
    private string sceneNameToReload;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Respawn()
    {
        if (isRespawning) return;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (RespawnPoint == null)
        {
            Debug.LogWarning("RespawnManager: no RespawnPoint set — cannot respawn.");
            isRespawning = false;
            yield break;
        }

        sceneNameToReload = RespawnPoint.gameObject.scene.name;

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        yield return StartCoroutine(fadeScreen.FadeOutCoroutine(fadeDuration));

        yield return SceneManager.UnloadSceneAsync(sceneNameToReload);
        SceneManager.LoadScene(sceneNameToReload, LoadSceneMode.Additive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isRespawning) return;
        if (scene.name != sceneNameToReload) return;
        StartCoroutine(AfterReload());
    }

    private IEnumerator AfterReload()
    {
        yield return null;
        yield return null;

        Events.onUnloadCreateBounds?.Invoke(sceneNameToReload);

        var player = GameObject.FindWithTag("Player");
        if (player != null && RespawnPoint != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            player.transform.position = RespawnPoint.position;
            if (rb != null)
                rb.position = RespawnPoint.position;
        }

        yield return StartCoroutine(fadeScreen.HoldColorDuration(Color.black, holdDuration));
        yield return StartCoroutine(fadeScreen.FadeInCoroutine(fadeDuration));

        isRespawning = false;
    }
}