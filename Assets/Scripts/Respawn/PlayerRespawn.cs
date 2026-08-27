using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform _currentRespawnPoint;

    public void SetRespawnPoint(Transform point)
    {
        if (point == null) return;
        _currentRespawnPoint = point;
    }

    public void Respawn()
    {
        if (_currentRespawnPoint == null) return;

        transform.position = _currentRespawnPoint.position;
        transform.rotation = _currentRespawnPoint.rotation;
    }
}