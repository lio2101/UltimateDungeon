using System.Collections;
using UnityEngine;


public enum BossMode
{
    Idle,
    Follow,
    Attack
}

public class BossController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SwordBehavior _sword;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Life _life;

    [Header("Settings")]
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _attackSpeed;
    [SerializeField] float _minDistance = 2f;
    [SerializeField] private float _rotationSmoothTime = .1f;


    [Header("Log")]
    [SerializeField] BossMode _currentMode;

    private PlayerController _player;
    private float _distanceToPlayer;
    private Coroutine _followPlayerCoroutine;
    private float _rotationVelocity;
    private float _timeSinceAttack;

    public float Attackspeed => _attackSpeed;
    public float Maxspeed => _maxSpeed;
    public Life Life => _life;



    void Start()
    {
        _currentMode = BossMode.Idle;
    }

    void Update()
    {
        if (_player == null) return;

        _timeSinceAttack += Time.deltaTime;

        _distanceToPlayer = Vector3.Distance(_rb.position, _player.transform.position);

        if (_distanceToPlayer > _minDistance)
        {
            if (_followPlayerCoroutine == null)
            {
                FollowPlayer();
            }
        }
        else
        {
            if (_followPlayerCoroutine != null)
            {
                StopCoroutine(_followPlayerCoroutine);
                _followPlayerCoroutine = null;
            }

            _rb.linearVelocity = new Vector3(0, 0, 0);

            if (_timeSinceAttack >= 1)
            {
                _timeSinceAttack = 0;
                _sword.Swing();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Debug.Log("found player");
            _player = player;
            _currentMode = BossMode.Follow;
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        _followPlayerCoroutine = StartCoroutine(FollowPlayerRoutine());
    }

    private IEnumerator FollowPlayerRoutine()
    {
        Debug.Log("Following");
        while (true)
        {
            Vector3 playerDirection = _player.transform.position - _rb.position;
            _rb.linearVelocity = _maxSpeed * playerDirection;

            float targetAngle = Vector3.SignedAngle(Vector3.forward, playerDirection, Vector3.up);
            float newAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, newAngle, 0f);

            yield return null;
        }

    }

}
