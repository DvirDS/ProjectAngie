using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [SerializeField] private SceneFade fadeScreen;
    [SerializeField] private float holdDuration = 0.3f;
    [SerializeField] private float fadeDuration = 0.5f;

    public Transform RespawnPoint { get; set; }

    private bool isRespawning = false;
    private string sceneNameToReload;
    private Vector3 respawnPosition;
    private bool hasRespawnPoint = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Respawn()
    {
        if (isRespawning) return;
        if (!hasRespawnPoint && RespawnPoint == null)
        {
            Debug.LogWarning("RespawnManager: no respawn point set — cannot respawn.");
            return;
        }
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (RespawnPoint != null)
        {
            respawnPosition = RespawnPoint.position;
            sceneNameToReload = RespawnPoint.gameObject.scene.name;
            RespawnPoint = null;
            hasRespawnPoint = true;
        }

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
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            player.transform.position = respawnPosition;
            if (rb != null)
                rb.position = respawnPosition;

            var health = player.GetComponent<HealthDrainSystem>();
            if (health != null) health.ResetAfterRespawn();
        }

        yield return StartCoroutine(fadeScreen.HoldColorDuration(Color.black, holdDuration));
        yield return StartCoroutine(fadeScreen.FadeInCoroutine(fadeDuration));

        isRespawning = false;
    }
}