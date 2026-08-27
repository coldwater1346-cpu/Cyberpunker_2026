using UnityEngine;

public class TimeControlManager : MonoBehaviour
{
    public static TimeControlManager Instance { get; private set; }

    [Header("World Time")]
    [SerializeField] private float _worldScale = 1f;
    [SerializeField] private bool _isSlowMode = false;

    public float WorldScale => _worldScale;
    public bool IsSlowMode => _isSlowMode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartSlowMode(float slowScale)
    {
        _isSlowMode = true;
        _worldScale = Mathf.Clamp(slowScale, 0.025f, 1f);
    }

    public void EndSlowMode()
    {
        _isSlowMode = false;
        _worldScale = 1f;
    }

    public float GetWorldScale()
    {
        return _worldScale;
    }
}