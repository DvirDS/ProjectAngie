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

    [Header("Rectangle Detection Settings")]
    public Vector2 sightBoxSize = new Vector2(10f, 4f);
    public Vector2 stopChaseBoxSize = new Vector2(14f, 6f);

    [Header("Chase Settings")]
    [Header("Chase Boundaries")]
    [Tooltip("Max distance for flying enemies (Radius)")]
    public float maxChaseDistance = 15f;

    [Tooltip("Max boundary for ground enemies (Width, Height)")]
    public Vector2 maxChaseBoxSize = new Vector2(20f, 5f);

    [Header("Combat (Ranged)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public float shootingRange = 6f; 
    public float fireRate = 1.5f;

    [Header("Melee Settings")]
    public float contactDamage = 10f;
    public float damageInterval = 1.0f; 

    [Header("Darkness Settings")]
    [Tooltip("Multiplier for sight range when player is in a dark zone")]
    public float darkZoneDetectionMultiplier = 0.5f;

    [Header("Alert")]
    [Tooltip("How long the enemy pauses in Alert state before chasing (seconds)")]
    public float alertDuration = 0.8f;

    [Header("Waypoint Behaviour")]
    [Tooltip("How long the enemy pauses at each waypoint before moving on")]
    public float waypointPauseTime = 0.5f;
}