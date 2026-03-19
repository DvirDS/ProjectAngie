using UnityEngine;

public class SoldierEnemy : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 6f;

    [Header("Platform Boundary")] // where the soldier should stop chasing and disappear
    [SerializeField] private float endX = 20f;

    private SpriteRenderer playerSprite;
    private bool isChasing = false;
    private bool hasFinished = false;
    private SpriteRenderer soldierSprite;
    private Rigidbody2D soldier;

    // so we can reset after respawn
    private Vector3 startPosition;

    private void Awake()
    {
        soldierSprite = GetComponent<SpriteRenderer>();
        soldier = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerSprite = playerObj.GetComponent<SpriteRenderer>();
        }

    }

    private void Update()
    {
        if (!isChasing || hasFinished) return;

        soldier.MovePosition(soldier.position + new Vector2(chaseSpeed * Time.deltaTime, 0f));

        if (transform.position.x >= endX)
        {
            hasFinished = true;
            gameObject.SetActive(false);
        }
    }

    public void Alert()
    {
        if (hasFinished)
            return;
        isChasing = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isChasing || hasFinished) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player caught by soldier!");
            transform.position = startPosition;
            gameObject.SetActive(false);
        }
    }
}