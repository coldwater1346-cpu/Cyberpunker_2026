using UnityEngine;

public class ValveRayInteractor : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rayDistance = 10f;
    [SerializeField] private LayerMask _layerMask;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _rayDistance, _layerMask))
        {
            ValveSwitch valve = hit.collider.GetComponent<ValveSwitch>();

            if (valve != null)
            {
                valve.Interact();
            }
        }
    }
}