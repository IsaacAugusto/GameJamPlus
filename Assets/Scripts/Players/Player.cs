using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour, IDamageble<float>
{
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float _hp = 1;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _runningMultiplier = 2;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected bool _isGrounded;
    private float _fallMult = 2.5f;
    private float _lowFallMult = 2;
    protected Rigidbody2D _rb;
    protected float _xInput;
    private SpriteRenderer _sprite;
    private Animator _animator;
    private bool _isRunning;
    private bool _canJump = false;

    virtual protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    virtual protected void Update()
    {
        GetInputs();
        UpdateSprite();
        MovePlayer();
        GroundDetector();
        Jump();
        CheckHP();
    }

    private void GetInputs()
    {
        _xInput = Input.GetAxis("Horizontal");
        _isRunning = Input.GetKey(KeyCode.LeftShift);
    }

    private void UpdateSprite()
    {
        if ((_sprite.flipX && _xInput > 0) || (!_sprite.flipX && _xInput < 0))
            _sprite.flipX = !_sprite.flipX;
    }

    private void MovePlayer()
    {
        _rb.velocity = new Vector2(_xInput * _moveSpeed, _rb.velocity.y);
        if (_isRunning)
            _rb.velocity *= _runningMultiplier * Vector2.right + Vector2.up;
        _animator.SetFloat("AbsVelX", Mathf.Abs(_rb.velocity.x));
        _animator.SetFloat("VelY", _rb.velocity.y);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            if (_canJump)
            {
                _canJump = false;
                _rb.AddForce(Vector2.up * _jumpForce);
            }
        }

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_fallMult - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetKey(KeyCode.W))
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_lowFallMult - 1) * Time.deltaTime;
        }
    }

    private void GroundDetector()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 2f, _groundLayer)
        || Physics2D.Raycast(transform.position + Vector3.right * .5f, Vector2.down, 2f, _groundLayer)
        || Physics2D.Raycast(transform.position - Vector3.right * .5f, Vector2.down, 2f, _groundLayer);
        _animator.SetBool("IsGrounded", _isGrounded);
        if (_isGrounded)
            _canJump = true;
    }


    public void ReciveDamage(float DamageTaken)
    {
        _hp -= DamageTaken;
    }

    private void CheckHP()
    {
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, Vector2.down * 2f);
        Gizmos.DrawRay(transform.position + Vector3.right * .5f, Vector2.down * 2f);
        Gizmos.DrawRay(transform.position - Vector3.right * .5f, Vector2.down * 2f);
    }



}
