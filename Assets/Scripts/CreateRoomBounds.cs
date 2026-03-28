using Unity.Cinemachine;
using UnityEngine;

public class CreateRoomBounds : MonoBehaviour
{

    private Collider2D roomBoundCollider;
    private CinemachineConfiner2D confiner;
    [SerializeField] private BoxCollider2D wall;

    void Awake()
    {
        roomBoundCollider = GetComponent<Collider2D>();
        roomBoundCollider.enabled = false;
        if (wall != null)
        {
            wall.enabled = false;
        }
    }

    void Start()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner == null)
            Debug.LogWarning("No CinemachineConfiner2D found in scene.");
    }

    void OnEnable()
    {
        Events.onUnloadCreateBounds += turnRoomColliderOn;
    }

    private void OnDisable()
    {
        Events.onUnloadCreateBounds -= turnRoomColliderOn;
    }

    private void turnRoomColliderOn(string sceneName)
    {
        if (sceneName != gameObject.scene.name) return;
        confiner.BoundingShape2D = roomBoundCollider;
        confiner.InvalidateBoundingShapeCache();
        if (wall != null)
        {
            wall.enabled = true;
        }
    }
}
