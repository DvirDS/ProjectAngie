using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy Data")]

public class EnemySO : ScriptableObject
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Detection")]
    public float sightRange = 8f;
    public float stopChaseRange = 12f;
    public float stealthDetectionMultiplier = 0.3f;

    [Header("Alert")]
    [Tooltip("How long the enemy pauses in Alert state before chasing (seconds)")]
    public float alertDuration = 0.8f;

    [Header("Waypoint Behaviour")]
    [Tooltip("How long the enemy pauses at each waypoint before moving on")]
    public float waypointPauseTime = 0.5f;
}