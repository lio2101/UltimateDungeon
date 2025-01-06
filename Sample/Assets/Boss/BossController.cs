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
    [SerializeField] private Animator _anim;
    [SerializeField] private Life _life;

    [Header("Settings")]
    [SerializeField] float _maxSpeed = 10f;
    [SerializeField] float _attackCD = 3f;
    [SerializeField] float _transitionTime = 0.5f;

    [SerializeField] float _attackSpeed;
    [SerializeField] float _minDistance = 2f;
    [SerializeField] private float _rotationSmoothTime = .1f;


    [Header("Log")]
    [SerializeField] BossMode _currentMode;

    private PlayerController _player;
    private float _distanceToPlayer;
    private float _rotationVelocity;
    private float _timeSinceAttack;
    private bool _isResting;

    public float Attackspeed => _attackSpeed;
    public float Maxspeed => _maxSpeed;
    public Life Life => _life;



    void Start()
    {
        _currentMode = BossMode.Idle;
        _anim.applyRootMotion = false;

    }

    void FixedUpdate()
    {
        if (_player == null) return;

        _timeSinceAttack += Time.deltaTime;

        if (_isResting) return;

        _distanceToPlayer = Vector3.Distance(_rb.position, _player.transform.position);
        //Debug.Log(_distanceToPlayer);

        if (_distanceToPlayer > _minDistance)
        {
            _anim.SetBool("Move", true);
            FollowPlayer();
        }
        else
        {
            Debug.Log("stop follow");
            _anim.SetBool("Move", false);
        }

        _rb.linearVelocity = new Vector3(0, 0, 0);

        if (_timeSinceAttack >= _attackCD)
        {
            Rest(_sword.SwingDuration);
            _anim.SetTrigger("Attack");
            _timeSinceAttack = 0;
            _sword.Swing();
            Rest(_transitionTime);
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
            this.GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void FollowPlayer()
    {
        Vector3 playerDirection = _player.transform.position - _rb.position;
        playerDirection.y = 0;
        _rb.MovePosition(Vector3.MoveTowards(transform.position, playerDirection, 0.01f * _maxSpeed));

        float targetAngle = Vector3.SignedAngle(Vector3.forward, playerDirection, Vector3.up);
        float newAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, newAngle, 0f);
    }

    private void Rest(float time)
    {
        StartCoroutine(WaitFor(time));
    }

    public IEnumerator WaitFor(float time)
    {
        _isResting = true;
        yield return new WaitForSeconds(time);
        _isResting = false;
    }

}
