using UnityEngine;

public class CheeseDrop : MonoBehaviour
{
    [SerializeField] private GameObject cheese;
    [SerializeField] private GameObject stick;
    [SerializeField] private GameObject rat;

    private const string defaultLayer = "Default";
    private SpriteRenderer cheeseSprite;

    private void Start()
    {
        rat.GetComponent<EnemyAI>().Freeze();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        bool wasCheesePickedUp = cheese.GetComponentInChildren<CheesePickUp>().IsCheesePickUp;
        if (wasCheesePickedUp)
        {
            DropCheese();
            DropStick();
            gameObject.SetActive(false);
            
            rat.GetComponent<EnemyAI>().Unfreeze();
        }
    }

    private void DropCheese()
    {
        cheese.GetComponentInChildren<CheesePickUp>().IsCheesePickUp = false;
        MakeRigidBodyDynamic(cheese);
        cheese.GetComponentInChildren<CheesePickUp>().CanPickUp = false;
        
        SpriteRenderer cheeseSprite = cheese.GetComponent<SpriteRenderer>();
        cheeseSprite.sortingLayerName = defaultLayer;
    }

    private void DropStick()
    {
        MakeRigidBodyDynamic(stick);
    }

    private void MakeRigidBodyDynamic(GameObject obj)
    {
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1.0f;
            rb.mass = 1.0f;
        }
    }
}
