using System.Collections;
using UnityEngine;


/*
 * 파티클 실행, 반환을 수행
 */

public class PooledParticle : MonoBehaviour
{
    private ParticleSystem _particle;
    private Coroutine _DoneRoutine;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (_DoneRoutine != null)
        {
            StopCoroutine(_DoneRoutine);
        }
        _particle.Clear();
        _particle.Play();

        _DoneRoutine = StartCoroutine(DoneRoutine());
    }

    IEnumerator DoneRoutine()
    {
        while (_particle != null && _particle.IsAlive(true))
        {
            yield return null;
        }

        PoolManager.Instance.Return(gameObject, PoolKey.EFFECT);
    }

    public void RestartAt(Transform firePosition)
    {
        transform.SetPositionAndRotation(firePosition.position, firePosition.rotation);
    }
}
