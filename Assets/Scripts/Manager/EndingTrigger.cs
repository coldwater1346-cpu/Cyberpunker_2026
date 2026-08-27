using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _endingCam;
    [SerializeField] private int _endingPriority = 20;
    [SerializeField] private float _endDelay = 3f;

    private bool _isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_isTriggered) return;
        if (!other.CompareTag("Player")) return;

        _isTriggered = true;
        StartCoroutine(PlayEnding());
    }

    private IEnumerator PlayEnding()
    {
        if (_endingCam != null)
            _endingCam.Priority = _endingPriority;

        yield return new WaitForSeconds(_endDelay);

        Debug.Log("게임 종료");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}