using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SwordBehavior _sword;
    [SerializeField] private Rigidbody _rb;

    [Header("Movement Settings")]
    [SerializeField, Min(0f)] private float _maxSpeed = 3.0f;
    [SerializeField] private float _rotationSmoothTime = .1f;

    [Header("Dodge Settings")]
    [SerializeField] private float _dodgeDistance = 2.0f;
    [SerializeField] private float _dodgeDuration = 0.2f;

    [Header("Attack Settings")]
    [SerializeField] private float _attackCooldown = 0.5f;


    private Vector2 _movementInput;
    private Vector3 _lastInputDirection;
    private float _rotationVelocity;
    private bool _isDodge;
    private Camera _camera;
    private float _timeSinceAttack;


    void Update()
    {
        _timeSinceAttack += Time.deltaTime;

        if (_camera == null)
        {
            _camera = Camera.main;
        }

        Vector3 inputDirection = new Vector3(_movementInput.x, 0f, _movementInput.y).normalized;

        Quaternion camRotation = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f);
        inputDirection = camRotation * inputDirection;

        if (inputDirection != Vector3.zero && !_isDodge)
        {
            _lastInputDirection = inputDirection;
            Vector3 movement = transform.position + (_maxSpeed * Time.deltaTime * inputDirection);
            //_rb.MovePosition(movement);

            if (_lastInputDirection != Vector3.zero)
            {
                float targetAngle = Vector3.SignedAngle(Vector3.forward, _lastInputDirection, Vector3.up);
                float newAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, newAngle, 0f);
            }
        }
        _rb.linearVelocity = _maxSpeed * inputDirection;
    }

    public void OnMove(InputValue inputValue)
    {
        //Debug.Log("Move Input");
        if (!_isDodge)
        {
            _movementInput = inputValue.Get<Vector2>();
        }
    }

    public void OnAttack(InputValue inputValue)
    {
        if (!_isDodge)
        {
            if(_timeSinceAttack >= _attackCooldown)
            {
                _timeSinceAttack = 0;
                _sword.Swing();
            }
            Debug.Log("Attack Input");
        }
    }

    public void OnDodge(InputValue inputValue)
    {
        Debug.Log("Dodge Input");
        //_rb.linearVelocity = _lastInputDirection * _dodgeDistance;
        StartCoroutine(DodgeRoutine());
    }


    private IEnumerator DodgeRoutine()
    {
        _isDodge = true;
        _movementInput = new Vector3(0,0,0);

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

    }
}
