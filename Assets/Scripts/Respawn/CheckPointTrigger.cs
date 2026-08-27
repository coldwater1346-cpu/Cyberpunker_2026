using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    [SerializeField] private Transform _respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        if (respawn != null)
        {
            respawn.SetRespawnPoint(_respawnPoint);
        }
    }
}