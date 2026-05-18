using UnityEngine;

public class CheesePickUp : MonoBehaviour
{
    private GameObject snoutAnchor;
    public bool IsCheesePickUp {get; set;}
    public bool CanPickUp { get; set; }

    private void Start()
    {
        IsCheesePickUp = false;
        CanPickUp = true;
        snoutAnchor = GameObject.FindGameObjectWithTag("Snout");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsCheesePickUp = CanPickUp;
    }

    void Update()
    {
        if (IsCheesePickUp && CanPickUp) 
            transform.position = snoutAnchor.transform.position;
    }
}
