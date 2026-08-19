using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy Data")]
public class EnemySO : ScriptableObject
{
    private const float DefaultPatrolSpeed = 2f;
    private const float DefaultChaseSpeed = 5f;
    private const float DefaultSightRange = 8f;
    private const float DefaultStopChaseRange = 12f;
    private const float DefaultStealthDetectionMultiplier = 0.3f;
    private const float DefaultSightBoxWidth = 10f;
    private const float DefaultSightBoxHeight = 4f;
    private const float DefaultStopChaseBoxWidth = 14f;
    private const float DefaultStopChaseBoxHeight = 6f;
    private const float DefaultMaxChaseDistance = 15f;
    private const float DefaultMaxChaseBoxWidth = 20f;
    private const float DefaultMaxChaseBoxHeight = 5f;
    private const float DefaultProjectileSpeed = 15f;
    private const float DefaultShootingRange = 6f;
    private const float DefaultFireRate = 1.5f;
    private const float DefaultContactDamage = 10f;
    private const float DefaultDamageInterval = 1.0f;
    private const float DefaultDarkZoneMultiplier = 0.5f;
    private const float DefaultAlertDuration = 0.8f;
    private const float DefaultWaypointPauseTime = 0.5f;

    [Header("Movement")]
    public float patrolSpeed = DefaultPatrolSpeed;
    public float chaseSpeed = DefaultChaseSpeed;

    [Header("Behaviour")]
    public bool patrolOnly = false;
    public bool canFly = false;

    [Header("Detection")]
    public float sightRange = DefaultSightRange;
    public float stopChaseRange = DefaultStopChaseRange;
    public float stealthDetectionMultiplier = DefaultStealthDetectionMultiplier;

    [Header("Rectangle Detection Settings")]
    public Vector2 sightBoxSize = new Vector2(DefaultSightBoxWidth, DefaultSightBoxHeight);
    public Vector2 stopChaseBoxSize = new Vector2(DefaultStopChaseBoxWidth, DefaultStopChaseBoxHeight);

    [Header("Chase Settings")]
    [Header("Chase Boundaries")]
    [Tooltip("Max distance for flying enemies (Radius)")]
    public float maxChaseDistance = DefaultMaxChaseDistance;

    [Tooltip("Max boundary for ground enemies (Width, Height)")]
    public Vector2 maxChaseBoxSize = new Vector2(DefaultMaxChaseBoxWidth, DefaultMaxChaseBoxHeight);

    [Header("Combat (Ranged)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = DefaultProjectileSpeed;
    public float shootingRange = DefaultShootingRange;
    public float fireRate = DefaultFireRate;

    [Header("Melee Settings")]
    public float contactDamage = DefaultContactDamage;
    public float damageInterval = DefaultDamageInterval;

    [Header("Darkness Settings")]
    [Tooltip("Multiplier for sight range when player is in a dark zone")]
    public float darkZoneDetectionMultiplier = DefaultDarkZoneMultiplier;

    [Header("Alert")]
    [Tooltip("How long the enemy pauses in Alert state before chasing (seconds)")]
    public float alertDuration = DefaultAlertDuration;

    [Header("Waypoint Behaviour")]
    [Tooltip("How long the enemy pauses at each waypoint before moving on")]
    public float waypointPauseTime = DefaultWaypointPauseTime;
}