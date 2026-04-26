using System;
using UnityEngine;

public class CallElevator : MonoBehaviour
{
    public event Action<float> OnPlayerEntered;

    private const string PlayerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
            OnPlayerEntered?.Invoke(other.transform.position.y);
    }

}