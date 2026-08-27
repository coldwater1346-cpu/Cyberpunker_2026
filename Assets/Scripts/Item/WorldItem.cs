using UnityEngine;

public class WorldItem : MonoBehaviour
{ 

    public enum ItemType
{
    CardKey,
    Sandevistan,
    Scouter
}


    [SerializeField] private ItemType _itemType;

    private bool _canPick = false;
    private PlayerItemController _playerItem;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _canPick = true;
        _playerItem = other.GetComponent<PlayerItemController>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _canPick = false;
        _playerItem = null;
    }

    private void Update()
    {
        if (!_canPick) return;
        if (_playerItem == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickItem();
        }
    }

    private void PickItem()
    {
        switch (_itemType)
        {
            case ItemType.CardKey:
                _playerItem.GetCardKey();
                break;

            case ItemType.Sandevistan:
                _playerItem.GetSlowTimer();
                break;

            case ItemType.Scouter:
                _playerItem.GetScouter();
                break;
        }

        gameObject.SetActive(false);
    }
}