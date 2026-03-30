using UnityEngine;

public class MineController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private GameObject redAuraObject;

    [Header("Runtime Set")]
    [SerializeField] private MineRuntimeSet mineSet;

    private ParticleSystem gasParticles;
    private ParticleSystem.EmissionModule gasEmission; // גישה ל"ברז" החלקיקים

    private void Awake()
    {
        if (redAuraObject != null)
        {
            gasParticles = redAuraObject.GetComponent<ParticleSystem>();

            if (gasParticles != null)
            {
                // שומרים את הרכיב שאחראי על ייצור החלקיקים
                gasEmission = gasParticles.emission;

                // מתחילים את המשחק כשה"ברז" סגור, אבל המערכת עצמה רצה ברקע
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

    public void SetSmellVisible(bool canSmell)
    {
        if (gasParticles != null)
        {
            // פשוט פותחים או סוגרים את הברז - מגיב באופן מיידי וללא השהיה!
            gasEmission.enabled = canSmell;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthDrainSystem playerHealth = other.GetComponent<HealthDrainSystem>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}