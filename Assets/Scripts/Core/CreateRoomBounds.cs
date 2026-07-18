using Unity.Cinemachine;
using UnityEngine;

public class CreateRoomBounds : MonoBehaviour
{

    private Collider2D roomBoundCollider;
    private CinemachineConfiner2D confiner;
    private CinemachinePositionComposer composer;
    [SerializeField] private BoxCollider2D[] physicalBounds;
    [SerializeField] private float dampingX = 0.1f;
    [SerializeField] private float dampingY = 0.1f;

    void Awake()
    {
        roomBoundCollider = GetComponent<Collider2D>();
        roomBoundCollider.enabled = false;
        if (physicalBounds != null)
        {
            for (int i = 0; i < physicalBounds.Length; i++)
            {
                physicalBounds[i].isTrigger = true;
            }
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
        SetDamping();
        confiner.InvalidateBoundingShapeCache();
        
        if (physicalBounds != null)
        {
            for (int i = 0; i < physicalBounds.Length; i++)
            {
                physicalBounds[i].enabled = true;
                physicalBounds[i].isTrigger = false;
            }
        }
    }

    private void SetDamping()
    {
        CinemachinePositionComposer composer = confiner.GetComponent<CinemachinePositionComposer>();
        composer.Damping = new Vector2(dampingX, dampingY);
    }
}
