using UnityEngine;

public class EnemyBackZone : MonoBehaviour
{
    [SerializeField] private EnemyUnit _enemy;

    private void Awake()
    {
        if (_enemy == null)
            _enemy = GetComponentInParent<EnemyUnit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerAttack player = other.GetComponentInParent<PlayerAttack>();
        if (player == null) return;

        if (_enemy == null || _enemy.IsDead) return;

        player.SetBackStabTarget(_enemy);

        Debug.Log("F키로 백어택 가능");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerAttack player = other.GetComponentInParent<PlayerAttack>();
        if (player == null) return;

        player.ClearBackStabTarget(_enemy);
    }
}