using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _distanceToPlayer = 10.0f;
    [SerializeField] private float _height = 10.0f;


    void Update()
    {
        transform.position = _player.transform.position + new Vector3(0, _height, _distanceToPlayer);
    }
}
