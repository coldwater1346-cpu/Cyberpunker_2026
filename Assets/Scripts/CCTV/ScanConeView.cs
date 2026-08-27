using UnityEngine;

public class ScanConeView : MonoBehaviour
{
    [SerializeField] private GameObject _normalCone;
    [SerializeField] private GameObject _scanCone;
    [SerializeField] private PlayerItemController _playerItem;

    private void Start()
    {
        if (_playerItem == null)
            _playerItem = FindFirstObjectByType<PlayerItemController>();

        ApplyConeView(false);
    }

    private void Update()
    {
        if (_playerItem == null) return;

        ApplyConeView(_playerItem.IsScanMode());
    }

    private void ApplyConeView(bool isScanMode)
    {
       // if (_normalCone != null)
           // _normalCone.SetActive(!isScanMode);

        if (_scanCone != null)
            _scanCone.SetActive(isScanMode);
    }
}