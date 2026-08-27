using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private Transform _firePosition;
    [SerializeField] private float _bulletDamage = 5f;
    [SerializeField] private float _spawnOffset = 0.3f;

    public void Fire(GameObject target)
    {
        if (_firePosition == null) return;
        if (target == null) return;
        if (PoolManager.Instance == null) return;

        GameObject bullet = PoolManager.Instance.Get(PoolKey.BULLET);
        if (bullet == null) return;

        Vector3 targetPos = target.transform.position + Vector3.up * 1.0f;
        Vector3 dir = (targetPos - _firePosition.position).normalized;

        bullet.transform.position = _firePosition.position + dir * _spawnOffset;
        bullet.transform.rotation = Quaternion.LookRotation(dir);

        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        if (bulletCtrl != null)
        {
            bulletCtrl.Init(_bulletDamage);
            bulletCtrl.Fire();
        }

        // πﬂªÁ »≠ø∞ ¿Ã∆Â∆Æ
        GameObject fireEffect = PoolManager.Instance.Get(PoolKey.EFFECT);
        if (fireEffect != null)
        {
            PooledParticle particle = fireEffect.GetComponent<PooledParticle>();
            if (particle != null)
            {
                particle.RestartAt(_firePosition);
            }
        }
    }
}