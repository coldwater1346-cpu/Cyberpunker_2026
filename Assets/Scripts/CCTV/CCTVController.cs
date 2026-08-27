using UnityEngine;

public class CCTVController : MonoBehaviour
{
    [SerializeField] private float _minAngle = -90f;
    [SerializeField] private float _maxAngle = 90f;

    [SerializeField] private float _rotateDuration = 7f; // 5~10초 사이 설정

    private float _range;

    private void Start()
    {
        _range = _maxAngle - _minAngle;
    }

    private void Update()
    {
        // 0 ~ range 값을 왕복
        float angle = Mathf.PingPong(Time.time * (_range / _rotateDuration), _range) + _minAngle;

        transform.localRotation = Quaternion.Euler(0f, angle, 0f);
    }
}