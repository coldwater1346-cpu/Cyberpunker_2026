using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform _leftDoor;
    [SerializeField] private Transform _rightDoor;
    [SerializeField] private Vector3 _leftOpenOffset = new Vector3(0, 0f, -6f);
    [SerializeField] private Vector3 _rightOpenOffset = new Vector3(0, 0f, 6f);
    [SerializeField] private float _openDuration = 2f;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera _cmPlayer;
    [SerializeField] private CinemachineCamera _cmDoor;
    [SerializeField] private float _cameraShowTime = 2f;

    private bool _isOpen = false;

    private Vector3 _leftClosedPos;
    private Vector3 _rightClosedPos;
    private Vector3 _leftOpenPos;
    private Vector3 _rightOpenPos;

    private void Start()
    {
        if (_leftDoor != null)
        {
            _leftClosedPos = _leftDoor.position;
            _leftOpenPos = _leftClosedPos + _leftOpenOffset;
        }

        if (_rightDoor != null)
        {
            _rightClosedPos = _rightDoor.position;
            _rightOpenPos = _rightClosedPos + _rightOpenOffset;
        }
    }

    // 외부에서 호출하는 함수
    public void SetOpen(bool value)
    {
        if (value)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (_isOpen) return;

        _isOpen = true;
        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        if (_cmDoor != null) _cmDoor.Priority = 20;
        if (_cmPlayer != null) _cmPlayer.Priority = 10;

        Vector3 leftStartPos = _leftDoor != null ? _leftDoor.position : Vector3.zero;
        Vector3 rightStartPos = _rightDoor != null ? _rightDoor.position : Vector3.zero;

        float elapsed = 0f;

        while (elapsed < _openDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _openDuration;

            if (_leftDoor != null)
                _leftDoor.position = Vector3.Lerp(leftStartPos, _leftOpenPos, t);

            if (_rightDoor != null)
                _rightDoor.position = Vector3.Lerp(rightStartPos, _rightOpenPos, t);

            yield return null;
        }

        if (_leftDoor != null)
            _leftDoor.position = _leftOpenPos;

        if (_rightDoor != null)
            _rightDoor.position = _rightOpenPos;

        yield return new WaitForSeconds(_cameraShowTime);

        if (_cmPlayer != null) _cmPlayer.Priority = 20;
        if (_cmDoor != null) _cmDoor.Priority = 10;
    }
}