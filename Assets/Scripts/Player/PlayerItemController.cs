using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerItemController : MonoBehaviour
{
    [Header("Card Key")]
    [SerializeField] private GameObject _cardKeyView;

    private bool _hasCardKey = false;
    public bool HasCardKey
    {
        get { return _hasCardKey; }
    }

    [Header("SlowTimer")]
    [SerializeField] private GameObject _slowTimerObject;

    private bool _hasSlowTimer = false;
    private bool _isSlowTimerEquip = false;
    private bool _isSlowTimerUsing = false;

    [Header("SlowTimer Skill")]
    [SerializeField] private float _slowScale = 0.2f;
    [SerializeField] private float _duration = 10f;

    private Coroutine _slowRoutine;

    [Header("Scouter")]
    [SerializeField] private GameObject _scouterObject;

    private bool _hasScouter = false;
    private bool _isScouterEquip = false;
    private bool _isScanMode = false;

    [Header("Global Volume")]
    [SerializeField] private Volume _globalVolume;

    private MotionBlur _motionBlur;
    private ColorAdjustments _colorAdjustments;

    private float _defaultPostExposure;
    private float _defaultContrast;
    private float _defaultSaturation;
    private Color _defaultColorFilter;

    private void Start()
    {
        if (_cardKeyView != null)
            _cardKeyView.SetActive(false);

        if (_slowTimerObject != null)
            _slowTimerObject.SetActive(false);

        if (_scouterObject != null)
            _scouterObject.SetActive(false);

        if (_globalVolume != null && _globalVolume.profile != null)
        {
            _globalVolume.profile.TryGet(out _motionBlur);
            _globalVolume.profile.TryGet(out _colorAdjustments);

            if (_motionBlur != null)
                _motionBlur.active = false;

            if (_colorAdjustments != null)
            {
                _defaultPostExposure = _colorAdjustments.postExposure.value;
                _defaultContrast = _colorAdjustments.contrast.value;
                _defaultSaturation = _colorAdjustments.saturation.value;
                _defaultColorFilter = _colorAdjustments.colorFilter.value;
            }
        }
    }

    private void Update()
    {
        // 슬로우 타이머 장착 ON/OFF
        if (_hasSlowTimer && Input.GetKeyDown(KeyCode.E))
        {
            ToggleSlowTimer();
        }

        // 슬로우 타이머 사용
        if (_isSlowTimerEquip && Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (!_isSlowTimerUsing)
            {
                _slowRoutine = StartCoroutine(UseSlowTimer());
            }
            else
            {
                StopSlowTimer();
            }
        }

        // 스카우터 장착 ON/OFF
        if (_hasScouter && Input.GetKeyDown(KeyCode.E))
        {
            ToggleScouter();
        }

        // 스캔모드 ON/OFF
        if (_isScouterEquip && Input.GetKeyDown(KeyCode.Alpha2))
        {
            ToggleScanMode();
        }
    }

    // -------------------------
    // 카드키
    // -------------------------
    public void GetCardKey()
    {
        _hasCardKey = true;

        if (_cardKeyView != null)
            _cardKeyView.SetActive(true);

        Debug.Log("카드키를 획득했습니다.");
    }

    public void UseCardKey()
    {
        _hasCardKey = false;

        if (_cardKeyView != null)
            _cardKeyView.SetActive(false);

        Debug.Log("카드키를 사용했습니다.");
    }

    // -------------------------
    // 슬로우 타이머
    // -------------------------
    public void GetSlowTimer()
    {
        _hasSlowTimer = true;

        if (_slowTimerObject != null)
            _slowTimerObject.SetActive(false);

        Debug.Log("슬로우 타이머를 획득했습니다.");
    }

    private void ToggleSlowTimer()
    {
        _isSlowTimerEquip = !_isSlowTimerEquip;

        if (_slowTimerObject != null)
            _slowTimerObject.SetActive(_isSlowTimerEquip);

        if (!_isSlowTimerEquip && _isSlowTimerUsing)
        {
            StopSlowTimer();
        }

        Debug.Log(_isSlowTimerEquip ? "슬로우 타이머 장착" : "슬로우 타이머 해제");
    }

    private IEnumerator UseSlowTimer()
    {
        _isSlowTimerUsing = true;

        if (TimeControlManager.Instance != null)
            TimeControlManager.Instance.StartSlowMode(_slowScale);

        SetSlowTimerVolume(true);

        Debug.Log("슬로우 타이머 발동");

        yield return new WaitForSecondsRealtime(_duration);

        StopSlowTimer();
    }

    private void StopSlowTimer()
    {
        _isSlowTimerUsing = false;

        if (_slowRoutine != null)
        {
            StopCoroutine(_slowRoutine);
            _slowRoutine = null;
        }

        if (TimeControlManager.Instance != null)
            TimeControlManager.Instance.EndSlowMode();

        SetSlowTimerVolume(false);

        Debug.Log("슬로우 타이머 종료");
    }

    private void SetSlowTimerVolume(bool isActive)
    {
        if (_motionBlur != null)
        {
            _motionBlur.active = isActive;
        }

        if (_colorAdjustments != null)
        {
            if (isActive)
            {
                _colorAdjustments.postExposure.value = -0.15f;
                _colorAdjustments.contrast.value = 18f;
                _colorAdjustments.saturation.value = -65f;
                _colorAdjustments.colorFilter.value = new Color(0.92f, 0.95f, 1f, 1f);
            }
            else
            {
                _colorAdjustments.postExposure.value = _defaultPostExposure;
                _colorAdjustments.contrast.value = _defaultContrast;
                _colorAdjustments.saturation.value = _defaultSaturation;
                _colorAdjustments.colorFilter.value = _defaultColorFilter;
            }
        }
    }

    // -------------------------
    // 스카우터
    // -------------------------
    public void GetScouter()
    {
        _hasScouter = true;

        if (_scouterObject != null)
            _scouterObject.SetActive(false);

        Debug.Log("스카우터를 획득했습니다.");
    }

    private void ToggleScouter()
    {
        _isScouterEquip = !_isScouterEquip;

        if (_scouterObject != null)
            _scouterObject.SetActive(_isScouterEquip);

        if (!_isScouterEquip && _isScanMode)
        {
            _isScanMode = false;
            Debug.Log("스캔모드 종료");
        }

        Debug.Log(_isScouterEquip ? "스카우터 장착" : "스카우터 해제");
    }

    private void ToggleScanMode()
    {
        _isScanMode = !_isScanMode;

        Debug.Log(_isScanMode ? "스캔모드 시작" : "스캔모드 종료");
    }

    public bool IsScanMode()
    {
        return _isScanMode;
    }

    public bool HasScouter
    {
        get { return _hasScouter; }
    }
}