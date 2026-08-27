using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    [SerializeField] private EnemyAI _enemyAI;

    private void Awake()
    {
        if (_enemyAI == null)
        {
            _enemyAI = GetComponentInParent<EnemyAI>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_enemyAI != null)
            {
                _enemyAI.StartSelfChase(other.gameObject);
            }
        }
    }
}