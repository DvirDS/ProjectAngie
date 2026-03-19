using System.Linq;
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
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        roomBoundCollider.enabled = false;
        wall.enabled = false;
    }

    void OnEnable()
    {
        Events.onUnloadCreateBounds += turnColliderOn;
    }

    private void turnColliderOn(string sceneName)
    {
        if (sceneName != gameObject.scene.name)
            return;
        confiner.BoundingShape2D = roomBoundCollider;
        confiner.InvalidateBoundingShapeCache();
        wall.enabled = true;
    }

    private void OnDisable()
    {
        Events.onUnloadCreateBounds -= turnColliderOn;
    }
}
