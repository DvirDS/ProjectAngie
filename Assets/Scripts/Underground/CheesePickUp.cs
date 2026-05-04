using UnityEngine;

public class CheesePickUp : MonoBehaviour
{
    private GameObject snoutAnchor;
    public bool IsCheesePickUp {get; set;}

    private void Start()
    {
        IsCheesePickUp = false;
        snoutAnchor = GameObject.FindGameObjectWithTag("Snout");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        IsCheesePickUp = true;
    }

    void Update()
    {
        if (IsCheesePickUp) 
            transform.position = snoutAnchor.transform.position;
    }
}
