using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BossController _boss;


    [Header("Components")]
    [SerializeField] private SwordBehavior _sword;
    [SerializeField] private Life _life;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _anim;


    [Header("Movement Settings")]
    [SerializeField, Min(0f)] private float _maxSpeed = 3.0f;
    [SerializeField] private float _rotationSmoothTime = .1f;

    [Header("Dodge Settings")]
    [SerializeField] private float _dodgeDistance = 2.0f;
    [SerializeField] private float _dodgeDuration = 0.2f;

    [Header("Attack Settings")]
    [SerializeField] private float _attackCooldown = 0.5f;


    private Vector2 _movementInput;
    private Vector2 _currentInput;
    private Vector3 _lastInputDirection;
    private float _rotationVelocity = 0f;
    private bool _isDodge;
    private Camera _camera;
    private float _timeSinceAttack;
    private float _distanceToBoss;

    public Life Life => _life;
    public float DistanceToBoss => _distanceToBoss;


    private void Start()
    {
        _anim.applyRootMotion = false;
    }

    void FixedUpdate()
    {
        _distanceToBoss = Vector3.Distance(transform.position, _boss.transform.position);
        _timeSinceAttack += Time.deltaTime;

        if (_camera == null)
        {
            _camera = Camera.main;
        }

        Vector3 inputDirection = new Vector3(_movementInput.x, 0f, _movementInput.y).normalized;

        Quaternion camRotation = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f);
        inputDirection = camRotation * inputDirection;

        if (inputDirection != Vector3.zero)
        {
            _anim.SetBool("Move", true);

            _lastInputDirection = inputDirection;

            Vector3 movement = _maxSpeed * Time.deltaTime * inputDirection;
            _rb.MovePosition(_rb.position + movement);


            float targetAngle = Mathf.Atan2(_lastInputDirection.x, _lastInputDirection.z) * Mathf.Rad2Deg;
            float newAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
            _rb.rotation = Quaternion.Euler(0f, newAngle, 0f);

            //Debug.Log($"InputDirection: {inputDirection}, TargetAngle: {targetAngle}, RotationVelocity: {_rotationVelocity}");

        }
        else
        {
            _anim.applyRootMotion = false;

            _lastInputDirection = Vector3.zero;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _anim.StopPlayback();
            _anim.SetBool("Move", false);
            //Debug.Log($"LID: {_lastInputDirection}, Velocity: {_rb.linearVelocity}");
        }

    }

    public void OnMove(InputValue inputValue)
    {
        //Debug.Log("Move Input");
        _movementInput = inputValue.Get<Vector2>();

        _currentInput = inputValue.Get<Vector2>();
        if (!_isDodge)
        {
            _movementInput = inputValue.Get<Vector2>();
        }
    }

    public void OnAttack(InputValue inputValue)
    {
        if (!_isDodge)
        {
            if (_timeSinceAttack >= _attackCooldown)
            {
                _anim.SetTrigger("Attack");
                _timeSinceAttack = 0;
                _sword.Swing();
            }
            Debug.Log("Attack Input");
        }
    }

    public void OnDodge(InputValue inputValue)
    {
        Debug.Log("Dodge Input");
        _anim.SetTrigger("Dodge");

        //_rb.linearVelocity = _lastInputDirection * _dodgeDistance;
        StartCoroutine(DodgeRoutine());
    }


    private IEnumerator DodgeRoutine()
    {
        _isDodge = true;
        _movementInput = Vector3.zero;

        //add default dodge direction in case input is zero?
        Vector3 start = transform.position;
        Vector3 end = transform.position + (_lastInputDirection * _dodgeDistance);

        float t = 0.0f;

        while (t < _dodgeDuration)
        {
            transform.position = Vector3.Lerp(start, end, (t / _dodgeDuration));
            t += Time.deltaTime;

            yield return null;
        }
        _isDodge = false;

        _movementInput = _currentInput;

    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        Debug.Log($"Collision with: {collision.gameObject.name}, Contact Point: {contact.point}");
    //    }
    //}
}
