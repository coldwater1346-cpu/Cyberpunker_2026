using System.Collections;
using UnityEngine;

public class ValveSwitch : MonoBehaviour
{
    [Header("Valve")]
    [SerializeField] private Transform _valveModel;
    [SerializeField] private Vector3 _rotateAxis = Vector3.forward;
    [SerializeField] private float _rotateAngle = 540f;
    [SerializeField] private float _rotateTime = 0.3f;

    [Header("Light")]
    [SerializeField] private GameObject _redLight;
    [SerializeField] private GameObject _greenLight;

    [Header("Puzzle Manager")]
    [SerializeField] private ValvePuzzleManager _puzzleManager;

    [Header("State")]
    [SerializeField] private bool _isGreen = false;

    private bool _isRotating = false;

    public bool IsGreen
    {
        get { return _isGreen; }
    }

    private void Start()
    {
        UpdateLight();
    }

    public void Interact()
    {
        if (_isRotating) return;
        StartCoroutine(RotateValveRoutine());
    }

    private IEnumerator RotateValveRoutine()
    {
        _isRotating = true;

        float rotated = 0f;
        float speed = _rotateAngle / _rotateTime;

        while (rotated < _rotateAngle)
        {
            float step = speed * Time.deltaTime;

            if (rotated + step > _rotateAngle)
            {
                step = _rotateAngle - rotated;
            }

            if (_valveModel != null)
            {
                _valveModel.Rotate(_rotateAxis.normalized, step, Space.Self);
            }

            rotated += step;
            yield return null;
        }

        _isGreen = !_isGreen;
        UpdateLight();

        Debug.Log($"{gameObject.name} 상태 변경 : {_isGreen}");

        if (_puzzleManager != null)
        {
            _puzzleManager.CheckPuzzle();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}의 PuzzleManager 참조가 비어있음");
        }

        _isRotating = false;
    }

    private void UpdateLight()
    {
        if (_redLight != null)
        {
            _redLight.SetActive(!_isGreen);
        }

        if (_greenLight != null)
        {
            _greenLight.SetActive(_isGreen);
        }
    }
}