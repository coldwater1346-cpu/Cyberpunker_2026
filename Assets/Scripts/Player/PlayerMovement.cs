using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 6f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float _jumpPower = 7f;

    [Header("Roll")]
    [SerializeField] private float _rollDistance = 3f;
    [SerializeField] private float _rollDuration = 0.25f;

    [Header("Camera")]
    [SerializeField] private Transform _cameraTransform;

    [Header("Mode")]
    [SerializeField] private KeyCode _pistolKey = KeyCode.Alpha1;

    private Rigidbody _rb;
    private Animator _anim;
    private PlayerHealth _playerHealth;

    private bool _isGround;
    private bool _isPistol;
    private bool _isRolling;

    public bool IsPistol
    {
        get { return _isPistol; }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _playerHealth = GetComponent<PlayerHealth>();

        if (_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (_cameraTransform == null) return;
        if (_anim == null) return;

        if (_playerHealth != null && _playerHealth.IsDead)
        {
            _isRolling = false;

            if (_rb != null)
            {
                _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
                _rb.angularVelocity = Vector3.zero;
            }

            _anim.SetFloat("Forward", 0f);
            _anim.SetFloat("Side", 0f);
            _anim.SetBool("IsMove", false);
            _anim.SetBool("IsRun", false);
            _anim.SetBool("IsGround", _isGround);

            return;
        }

        if (Input.GetKeyDown(_pistolKey))
        {
            _isPistol = !_isPistol;
        }

        _anim.SetBool("IsPistol", _isPistol);

        if (_isRolling) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isRun = Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W);
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool roll = Input.GetKeyDown(KeyCode.C);

        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        if (camForward != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(camForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        float currentSpeed = isRun ? _runSpeed : _walkSpeed;
        transform.position += moveDir * currentSpeed * Time.deltaTime;

        bool isMove = moveDir != Vector3.zero;

        _anim.SetFloat("Forward", v);
        _anim.SetFloat("Side", h);
        _anim.SetBool("IsMove", isMove);
        _anim.SetBool("IsRun", isRun);
        _anim.SetBool("IsGround", _isGround);

        if (jump && _isGround)
        {
            _anim.SetTrigger("Jump");
            _isGround = false;

            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
        }

        if (roll && _isGround)
        {
            _anim.SetTrigger("Roll");
            StartCoroutine(RollCoroutine(moveDir, camForward));
        }
    }

    private IEnumerator RollCoroutine(Vector3 moveDir, Vector3 camForward)
    {
        _isRolling = true;

        Vector3 rollDir;

        if (moveDir != Vector3.zero)
        {
            rollDir = moveDir;
        }
        else
        {
            rollDir = camForward;
        }

        float elapsed = 0f;
        float rollSpeed = _rollDistance / _rollDuration;

        while (elapsed < _rollDuration)
        {
            if (_playerHealth != null && _playerHealth.IsDead)
            {
                _isRolling = false;
                yield break;
            }

            transform.position += rollDir * rollSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isRolling = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = false;
        }
    }
}