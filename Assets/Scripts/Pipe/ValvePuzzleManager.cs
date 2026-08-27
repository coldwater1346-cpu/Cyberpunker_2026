using UnityEngine;

public class ValvePuzzleManager : MonoBehaviour
{
    [Header("Valve")]
    [SerializeField] private ValveSwitch[] _valves;

    [Header("Door")]
    [SerializeField] private DoorController _doorController;

    private bool _isClear = false;

    public void CheckPuzzle()
    {
        if (_isClear)
        {
            Debug.Log("이미 퍼즐 클리어 상태");
            return;
        }

        if (_valves == null || _valves.Length == 0)
        {
            Debug.LogWarning("Valve 배열이 비어있음");
            return;
        }

        for (int i = 0; i < _valves.Length; i++)
        {
            if (_valves[i] == null)
            {
                Debug.LogWarning($"Valve 배열 {i}번이 비어있음");
                return;
            }

            Debug.Log($"{_valves[i].gameObject.name} 현재 상태 : {_valves[i].IsGreen}");

            if (_valves[i].IsGreen == false)
            {
                Debug.Log("아직 모든 벨브가 초록불 상태가 아님");
                return;
            }
        }

        _isClear = true;
        Debug.Log("퍼즐 성공! 문 열기 실행");

        if (_doorController != null)
        {
            _doorController.SetOpen(true);
        }
        else
        {
            Debug.LogWarning("DoorController 참조가 비어있음");
        }
    }
}