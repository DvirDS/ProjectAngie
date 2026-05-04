using System.Collections;
using UnityEngine;

public class CheeseDrop : MonoBehaviour
{
    [SerializeField] private GameObject cheese;
    [SerializeField] private GameObject stick;
    [SerializeField] private GameObject rat;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        bool wasCheesePickedUp = cheese.GetComponentInChildren<CheesePickUp>().IsCheesePickUp;
        if (wasCheesePickedUp)
        {
            DropCheese();
            DropStick();
            gameObject.SetActive(false);
        }
    }

    private void DropCheese()
    {
        cheese.GetComponentInChildren<CheesePickUp>().IsCheesePickUp = false;
        MakeRigidBodyDynamic(cheese);
    }

    private void DropStick()
    {
        MakeRigidBodyDynamic(stick);
        StartCoroutine(ObjectDisappear(stick));
    }

    private IEnumerator ObjectDisappear(GameObject obj)
    {
        yield return new WaitForSeconds(10f);
        obj.SetActive(false);
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
