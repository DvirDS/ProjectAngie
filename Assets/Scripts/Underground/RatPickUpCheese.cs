using System;
using UnityEngine;

public class DropCheese : MonoBehaviour
{
    [SerializeField] private GameObject cheese;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != cheese.name) return;
        cheese.SetActive(false);
    }
}
