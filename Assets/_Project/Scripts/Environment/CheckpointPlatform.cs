using UnityEngine;

public class CheckpointPlatform : MonoBehaviour
{
    [SerializeField] private Material _floorMaterial;

    private MeshRenderer _meshRenderer;
    
    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        PlayerMovement.Instance.OnMoveEnd += CheckpointRayCast;
    }

    private void OnDestroy()
    {
        PlayerMovement.Instance.OnMoveEnd -= CheckpointRayCast;
    }

    private void CheckpointRayCast()
    {
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfo, 1f))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                ChangeMaterial();
            }
        }
    }

    private void ChangeMaterial()
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.material = _floorMaterial;
        }
    }
}
