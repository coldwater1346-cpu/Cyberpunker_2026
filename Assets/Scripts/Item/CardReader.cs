using UnityEngine;

public class CardReader : MonoBehaviour
{
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private DoorController _doorController;

    private bool _canUse = false;
    private PlayerItemController _playerItem;

    private void Update()
    {
        if (_canUse && Input.GetKeyDown(KeyCode.E))
        {
            if (_playerItem == null) return;

            if (_playerItem.HasCardKey)
            {
                _playerItem.UseCardKey();

                if (_doorController != null)
                {
                    _doorController.OpenDoor();
                }

                Debug.Log("카드키 인증 성공");
            }
            else
            {
                Debug.Log("카드키가 없습니다.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _canUse = true;
            _playerItem = other.GetComponent<PlayerItemController>();

            Debug.Log("E 키를 눌러 카드리더기를 사용하세요.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _canUse = false;
            _playerItem = null;
        }
    }
}