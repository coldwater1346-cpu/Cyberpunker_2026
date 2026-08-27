//using UnityEngine;

//public class CardKeyItem : MonoBehaviour
//{
//    [Header("Pick Up")]
//    [SerializeField] private string _playerTag = "Player";

//    [Header("Item Effect")]
//    [SerializeField] private float _rotateSpeed = 90f;   // 회전 속도

//    private bool _canPickUp = false;
//    private PlayerInventory _playerInventory;

//    private void Update()
//    {
//        // 아이템 회전
//        transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);

//        // 획득 입력
//        if (_canPickUp && Input.GetKeyDown(KeyCode.E))
//        {
//            if (_playerInventory != null)
//            {
//                _playerInventory.GetCardKey();
//                gameObject.SetActive(false);
//            }
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag(_playerTag))
//        {
//            _canPickUp = true;
//            _playerInventory = other.GetComponent<PlayerInventory>();

//            Debug.Log("E 키를 눌러 카드키를 획득하세요.");
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag(_playerTag))
//        {
//            _canPickUp = false;
//            _playerInventory = null;
//        }
//    }
//}