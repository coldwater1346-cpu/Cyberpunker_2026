using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float _mouseXSpeed = 200f;
    [SerializeField] private float _mouseYSpeed = 80f;

    [SerializeField] private float _maxUpAngle = 20f;
    [SerializeField] private float _maxDownAngle = 0f;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _cam;

    private float _leftRight;
    private float _upDown;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _leftRight = _player.eulerAngles.y;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseXSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseYSpeed * Time.deltaTime;

        _leftRight += mouseX;
        _upDown -= mouseY;

        _upDown = Mathf.Clamp(_upDown, _maxDownAngle, _maxUpAngle);

        _player.rotation = Quaternion.Euler(0f, _leftRight, 0f);
        _cam.localRotation = Quaternion.Euler(_upDown, 0f, 0f);
    }
}