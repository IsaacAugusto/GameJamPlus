using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour, IDamageble<float>
{
    [SerializeField] protected float _jumpForce;
    [SerializeField] protected float _hp;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected LayerMask _groundLayer;
    [SerializeField] protected bool _canJump;
    private float _fallMult = 2.5f;
    private float _lowFallMult = 2;
    protected Rigidbody2D _rb;
    protected float _xInput;
    virtual protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    virtual protected void Update()
    {
        GetInputs();
        MovePlayer();
        GroundDetector();
        Jump();
    }

    private void MovePlayer()
    {
        _rb.velocity = new Vector2(_xInput * _moveSpeed, _rb.velocity.y);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (_canJump)
            {
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
        _canJump = Physics2D.Raycast(transform.position, Vector2.down, .5f, _groundLayer)
        ||Physics2D.Raycast(transform.position + Vector3.right * .5f, Vector2.down, 1.2f, _groundLayer)
        ||Physics2D.Raycast(transform.position - Vector3.right * .5f, Vector2.down, 1.2f, _groundLayer);
    }

    private void GetInputs()
    {
        _xInput = Input.GetAxis("Horizontal");
    }

    public void ReciveDamage(float DamageTaken)
    {
        _hp -= DamageTaken;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay(transform.position, Vector2.down);
        Gizmos.DrawRay(transform.position + Vector3.right * .5f, Vector2.down);
        Gizmos.DrawRay(transform.position - Vector3.right * .5f, Vector2.down);
    }



}
