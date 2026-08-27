using UnityEngine;

public class SecurityCameraController : MonoBehaviour
{
    [Header("Detect Enemies")]
    [SerializeField] private EnemyAI[] _enemies;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 감시 카메라 범위에 들어왔습니다.");

            for (int i = 0; i < _enemies.Length; i++)
            {
                if (_enemies[i] != null)
                {
                    _enemies[i].StartAlertChase(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 감시 카메라 범위에서 벗어났습니다.");
        }
    }
}