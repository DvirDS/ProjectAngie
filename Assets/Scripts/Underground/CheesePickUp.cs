using UnityEngine;

public class CheesePickUp : MonoBehaviour
{
    private SpriteRenderer sprite;
    private const string noGroundLayer = "TransparentFX";
    private GameObject snoutAnchor;
    public bool IsCheesePickUp {get; set;}
    public bool CanPickUp { get; set; }
    

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        IsCheesePickUp = false;
        CanPickUp = true;
        snoutAnchor = GameObject.FindGameObjectWithTag("Snout");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsCheesePickUp = CanPickUp;
        sprite.sortingLayerName = noGroundLayer;
    }

    void Update()
    {
        if (IsCheesePickUp && CanPickUp) 
            transform.position = snoutAnchor.transform.position;
    }
}
