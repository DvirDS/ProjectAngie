using UnityEngine;

public class Elevator : MonoBehaviour
{
    private const float DefaultSpeed = 2f;
    private const float ArrivalThreshold = 0.05f;
    private const string PlayerTag = "Player";

    [Header("Floor Targets")]
    [SerializeField] private Transform lastFloorTransform;

    [Header("Settings")]
    [SerializeField] private float speed = DefaultSpeed;

    [Header("Call Zone")]
    [SerializeField] private CallElevator callZone;

    private enum ElevatorState { Idle, GoingToPlayer, WaitingForPlayer, GoingToLastFloor }
    private ElevatorState state = ElevatorState.Idle;

    private float callY;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (callZone != null)
            callZone.OnPlayerEntered += OnPlayerCalledElevator;
    }

    private void OnDisable()
    {
        if (callZone != null)
            callZone.OnPlayerEntered -= OnPlayerCalledElevator;
    }

    private void OnPlayerCalledElevator(float playerY)
    {
        if (state != ElevatorState.Idle) return;
        callY = playerY;
        state = ElevatorState.GoingToPlayer;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(PlayerTag)) return;

        state = ElevatorState.GoingToLastFloor;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case ElevatorState.GoingToPlayer:
                MoveTo(callY, ElevatorState.WaitingForPlayer);
                break;

            case ElevatorState.GoingToLastFloor:
                MoveTo(lastFloorTransform.position.y, ElevatorState.Idle);
                break;
        }
    }

    private void MoveTo(float floor, ElevatorState nextState)
    {
        float newY = Mathf.MoveTowards(
                rb.position.y,
                floor,
                speed * Time.fixedDeltaTime
            );

        rb.MovePosition(new Vector2(rb.position.x, newY));

        if (Mathf.Abs(rb.position.y - floor) < ArrivalThreshold)
        {
            rb.MovePosition(new Vector2(rb.position.x, floor));
            rb.linearVelocity = Vector2.zero;
            state = nextState;
        }
    }
}