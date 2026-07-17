using System.Collections;
using UnityEngine;

public class MineController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject redAuraObject;
    [SerializeField] private float hitRevealDuration = 2f;

    [Header("Runtime Set")]
    [SerializeField] private MineRuntimeSet mineSet;

    private const string PlayerTag = "Player";

    private ParticleSystem gasParticles;
    private ParticleSystem.EmissionModule gasEmission;
    private bool superSniffActive;
    private Coroutine hitRevealCoroutine;

    private void Awake()
    {
        if (redAuraObject != null)
        {
            gasParticles = redAuraObject.GetComponent<ParticleSystem>();

            if (gasParticles != null)
            {
                gasEmission = gasParticles.emission;

                gasEmission.enabled = false;
                gasParticles.Play();
            }
        }
    }

    private void OnEnable()
    {
        if (mineSet != null) mineSet.AddToList(this);
        PlayerSniff.OnSuperSniff += SetSmellVisible;
    }

    private void OnDisable()
    {
        if (mineSet != null) mineSet.RemoveFromList(this);
        PlayerSniff.OnSuperSniff -= SetSmellVisible;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(PlayerTag)) return;
        RevealHit();
    }

    private void RevealHit()
    {
        if (gasParticles == null) return;

        if (hitRevealCoroutine != null) StopCoroutine(hitRevealCoroutine);
        hitRevealCoroutine = StartCoroutine(HitRevealRoutine());
    }

    private IEnumerator HitRevealRoutine()
    {
        gasEmission.enabled = true;
        yield return new WaitForSeconds(hitRevealDuration);
        gasEmission.enabled = superSniffActive;
        hitRevealCoroutine = null;
    }

    public void SetSmellVisible(bool canSmell)
    {
        superSniffActive = canSmell;

        if (gasParticles != null && hitRevealCoroutine == null)
        {
            gasEmission.enabled = canSmell;
        }
    }
}