using System;
using UnityEngine;

public class CallElevator : MonoBehaviour
{
    private const float DefaultTransformOffset = 0.8f;
    private const string PlayerTag = "Player";

    public event Action<float> OnPlayerEntered;

    [SerializeField] private float transformOffset = DefaultTransformOffset;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
            OnPlayerEntered?.Invoke(other.transform.position.y - transformOffset);
    }
}