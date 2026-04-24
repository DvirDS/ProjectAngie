using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Floor Targets")]
    [SerializeField] private Transform lastFloorTransform;

    [Header("Settings")]
    [SerializeField] private float speed = 2f;

    [Header("Call Zone")]
    [SerializeField] private CallElevator callZone;

    private enum ElevatorState { Idle, GoingToPlayer, WaitingForPlayer, GoingToLastFloor }
    private ElevatorState state = ElevatorState.Idle;

    private float callY;
    private const string PlayerTag = "Player";

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

    private void Update()
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
            transform.position.y,
            floor,
            speed * Time.deltaTime
        );

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Mathf.Abs(transform.position.y - floor) < 0.01f)
        {
            transform.position = new Vector3(transform.position.x, floor, transform.position.z);
            state = nextState;
        }
            
    }
}