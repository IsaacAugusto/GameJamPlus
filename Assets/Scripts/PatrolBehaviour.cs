﻿using System;
using UnityEngine;

public delegate void EnemyEvent(PatrolBehaviour enemy);

public class PatrolBehaviour : MonoBehaviour, IDamageble<float>
{
    public static event EnemyEvent OnEnemyDied;

    [SerializeField] private float _hp = 5;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _velocity = 5;
    [SerializeField] private float _checkGroundDistance = 5;
    [SerializeField] private float _checkWallDistance = 3;
    [SerializeField] private float _shootDelay = .5f;
    [SerializeField] [Range(0, 360)] private float _checkGroundAngle;
    [SerializeField] private LayerMask _groundOrWall;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _detectDistance;
    [SerializeField] private bool _playerDetected = false;
    [SerializeField] private bool _enemyDetected = false;
    [SerializeField] private bool _wallDetected = false;
    [SerializeField] private bool _groundDetected = false;
    private SpriteRenderer _gunSprite;
    private Animator _anim;
    private Transform _playerTransform;
    private Vector2 dir;
    private AudioSource _source;

    [SerializeField] private Transform _bulletSpawnTransform;
    [SerializeField] private Transform _gunTransform;
    private float _shootTimer;
    private RaycastHit2D hit;

    private Vector3 _checkGroundDir;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
        if (!_playerDetected)
            _playerTransform = FindObjectOfType<Player>().transform;
        _gunSprite = _gunTransform.GetComponentInChildren<SpriteRenderer>();
    }
    void Update()
    {
        dir = transform.right;
        if (transform.rotation.y == 0)
        {
            _checkGroundDir = Quaternion.AngleAxis(_checkGroundAngle, Vector3.back) * dir;
        }
        else
        {
            _checkGroundDir = Quaternion.AngleAxis(_checkGroundAngle, Vector3.forward) * dir;
        }
        _groundDetected = CheckGround();
        _wallDetected = CheckWall();
        _enemyDetected = CheckEnemy();

        if (!_groundDetected || _wallDetected || _enemyDetected)
            FlipDirection();
        Move();
        CheckDeath();

        DetectPlayer();
        if (_playerDetected)
        {
            FireState();
        }
        AimAtPlayer();
    }
    private void FlipDirection()
    {
        if (transform.rotation.y == 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Move()
    {
        if (!_playerDetected)
        {
            _rigidbody.velocity = new Vector2((dir * _velocity).x, _rigidbody.velocity.y);
        }
        else
        {
            _rigidbody.velocity = Vector2.up * _rigidbody.velocity.y;
        }
    }

    private bool CheckWall()
    {
        Debug.DrawRay(transform.position + Vector3.down * 0.5f, transform.right * 1.5f, Color.yellow);
        return Physics2D.Raycast(transform.position + Vector3.down, transform.right, _checkWallDistance, _groundOrWall);

    }

    private bool CheckEnemy()
    {
        Debug.DrawRay(transform.position + transform.right * .8f, transform.right * 1.5f, Color.blue);
        return Physics2D.Raycast(transform.position + transform.right * .8f, transform.right, _checkWallDistance, _enemyLayer);
    }

    private bool CheckGround()
    {
        Debug.DrawRay(transform.position,_checkGroundDir * _checkGroundDistance, Color.black);
        return Physics2D.Raycast(transform.position, _checkGroundDir, _checkGroundDistance, _groundOrWall);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawRay(transform.position, _checkGroundDir * _checkGroundDistance);
        //Gizmos.color = Color.green;
        //if (rayhit)
        //{
        //    Gizmos.DrawLine(transform.position, rayhit.point);
        //}
       // Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(dir));
        //Gizmos.DrawLine(transform.position, transform.position + _checkWallDir * _checkWallDistance);
        //Gizmos.DrawWireSphere(transform.position, _detectDistance);
        //Gizmos.color = Color.green;
    }

    private void DetectPlayer()
    {
        if (_playerTransform)
        {
            hit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, Vector3.Distance(transform.position, _playerTransform.position), LayerMask.GetMask("Ground"));
            if (hit == true)
            {
                _playerDetected = false;
                _anim.SetBool("IsShooting", false);
            }
            else
            {
                Debug.DrawRay(transform.position, (_playerTransform.position - transform.position) * 5, Color.cyan);
                _playerDetected = Physics2D.OverlapCircle(transform.position, _detectDistance, LayerMask.GetMask("Player"));
                _anim.SetBool("IsShooting", true);
            }
        }
    }

    private void FireState()
    {
        if (_playerTransform.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0)
        {
            var bullet = BulletPool.Instance.GetBulletFromPool();
            bullet.GetComponent<Bullet>().Damage = 1;
            bullet.transform.position = _bulletSpawnTransform.position;
            _source.Play();
            bullet.transform.rotation = _gunTransform.rotation;
            _shootTimer = _shootDelay;
        }
    }

    private void AimAtPlayer()
    {
        if (_playerDetected)
        {
            if (transform.localRotation.y == 0)
            {
                var dir = _playerTransform.position - _gunTransform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _gunSprite.flipY = (Mathf.Abs(angle) > 90);
                _gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                var dir = _playerTransform.position - _gunTransform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _gunSprite.flipY = (Mathf.Abs(angle) > 90);
                _gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

    }

    private void CheckDeath()
    {
        if (_hp <= 0)
        {
            OnEnemyDied?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void ReciveDamage(float DamageTaken)
    {
        _hp -= DamageTaken;
    }
}
