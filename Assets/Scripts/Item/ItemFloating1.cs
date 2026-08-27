using UnityEngine;

public class ItemFloating1 : MonoBehaviour
{
    [Header("Item Effect")]
    [SerializeField] private float _rotateSpeed = 90f;   // 회전 속도
   

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }
}
