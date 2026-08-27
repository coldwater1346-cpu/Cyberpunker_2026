using UnityEngine;
using Unity.Cinemachine;

public class ScanMode : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _cmPlayer;
    [SerializeField] private CinemachineCamera _cmScan;

    [Header("Scan Effect")]
    [SerializeField] private GameObject _scanPanel;

    [Header("Player")]
    [SerializeField] private PlayerItemController _playerItem;

    private void Start()
    {
        if (_playerItem == null)
            _playerItem = FindFirstObjectByType<PlayerItemController>();

        ApplyScanView(false);
    }

    private void Update()
    {
        if (_playerItem == null) return;

        ApplyScanView(_playerItem.IsScanMode());
    }

    private void ApplyScanView(bool isScanMode)
    {
        if (_cmPlayer != null)
            _cmPlayer.Priority = isScanMode ? 0 : 10;

        if (_cmScan != null)
            _cmScan.Priority = isScanMode ? 10 : 0;

        if (_scanPanel != null)
            _scanPanel.SetActive(isScanMode);
    }
}