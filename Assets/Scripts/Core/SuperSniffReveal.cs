using UnityEngine;

public class SuperSniffReveal : MonoBehaviour
{
    [Header("References (Auto-filled)")]
    [Tooltip("הסקריפט ימצא אוטומטית את כל הילדים אם תשאיר את השדות ריקים")]
    [SerializeField] private Renderer[] objectRenderers;
    [SerializeField] private Collider2D[] objectColliders;
    [SerializeField] private ParticleSystem highlightParticles;

    [Header("Settings")]
    [Tooltip("If true, the object cannot be touched/stood on when invisible.")]
    [SerializeField] private bool disableCollisionWhenHidden = true;

    private bool isCurrentlySniffing = false;

    private void Awake()
    {
        if (objectRenderers == null || objectRenderers.Length == 0)
            objectRenderers = GetComponentsInChildren<Renderer>(true);

        if (objectColliders == null || objectColliders.Length == 0)
            objectColliders = GetComponentsInChildren<Collider2D>(true);

        if (highlightParticles == null)
            highlightParticles = GetComponentInChildren<ParticleSystem>(true);
    }

    private void OnEnable()
    {
        PlayerSniff.OnSuperSniff += HandleSuperSniff;
    }

    private void OnDisable()
    {
        PlayerSniff.OnSuperSniff -= HandleSuperSniff;
    }

    private void Start()
    {
        HandleSuperSniff(false);
    }

    private void Update()
    {
        if (isCurrentlySniffing && highlightParticles != null && highlightParticles.isPlaying)
        {
            if (!HasExistingObjects())
            {
                highlightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private void HandleSuperSniff(bool isSniffing)
    {
        isCurrentlySniffing = isSniffing;
        bool hasObjects = HasExistingObjects();

        foreach (var rnd in objectRenderers)
        {
            if (rnd != null && !(rnd is ParticleSystemRenderer))
            {
                rnd.enabled = isSniffing;
            }
        }

        if (disableCollisionWhenHidden)
        {
            foreach (var col in objectColliders)
            {
                if (col != null) col.enabled = isSniffing;
            }
        }

        if (!hasObjects)
        {
            if (highlightParticles != null) highlightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return;
        }

        if (highlightParticles != null)
        {
            if (isSniffing)
            {
                highlightParticles.Play();
            }
            else
            {
                highlightParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private bool HasExistingObjects()
    {
        foreach (var rnd in objectRenderers)
        {
            if (rnd != null && !(rnd is ParticleSystemRenderer))
            {
                if (rnd.gameObject.activeInHierarchy)
                {
                    return true;
                }
            }
        }
        return false;
    }
}