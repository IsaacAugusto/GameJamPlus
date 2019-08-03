using System;
using UnityEngine;

public class PatrolBehaviour : MonoBehaviour, IDamageble<float> {
    [SerializeField] private float _hp = 5;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _velocity = 5;
    [SerializeField] private float _checkGroundDistance = 5;
    [SerializeField] private float _checkWallDistance = 5;
    [SerializeField] [Range(0, 360)] private float _checkGroundAngle;
    [SerializeField] private LayerMask _groundOrWall;
    [SerializeField] private float _detectDistance;
    [SerializeField] private bool _playerDetected = false;
    private bool _isInFireState = false;
    private Transform _playerTransform;
    private Vector2 dir = Vector2.right;

    [SerializeField] private Transform _bulletSpawnTransform;
    [SerializeField] private Transform _gunTransform;
    private float _shootDelay;

    private Vector3 _checkGroundDir = Vector2.right;
    private Vector3 _checkWallDir;

    void Update() {
        _checkGroundDir = Quaternion.AngleAxis(_checkGroundAngle, Vector3.back) * dir;

        if (transform.localScale.x == -1)
          _checkGroundDir.y *= -1;

        if (!CheckGround() || CheckWall())
          FlipDirection();

        Move();
        CheckDeath();
        DetectPlayer();
        FireState();
        AimAtPlayer();
    }
    private void FlipDirection() {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void Move()
    {
        if (transform.localScale.x == -1)
        {
            dir = Vector2.left;
            _checkWallDir = Vector2.left;
        }

        else
        {
            dir = Vector2.right;
            _checkWallDir = Vector2.right;
        }

        if (!_isInFireState)
        {
            _rigidbody.velocity = dir * _velocity;
        }
    }

    private bool CheckWall()
    {
        return Physics2D.Raycast(transform.position, _checkWallDir, _checkWallDistance, _groundOrWall).collider;

    }

    private bool CheckGround() {
        return Physics2D.Raycast(transform.position, _checkGroundDir, _checkGroundDistance, _groundOrWall).collider;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _checkGroundDir.normalized * _checkGroundDistance);
        Gizmos.DrawLine(transform.position, transform.position + _checkWallDir * _checkWallDistance);
        Gizmos.DrawWireSphere(transform.position, _detectDistance);
    }

    private void DetectPlayer()
    {
        var player = Physics2D.OverlapCircle(transform.position, _detectDistance, LayerMask.GetMask("Player"));
        _playerDetected = player;
        _playerTransform = player.transform;
    }

    private void FireState()
    {
        if (_playerDetected)
        {
            _isInFireState = true;
        }
        else
        {
            _isInFireState = false;
        }

        if (_isInFireState)
        {
            if (_playerTransform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
            _shootDelay--;
            if (_shootDelay <= 0)
            {
                var bullet = BulletPool.Instance.GetBulletFromPool();
                bullet.transform.position = _bulletSpawnTransform.position;
                bullet.transform.rotation = _gunTransform.rotation;
                bullet.GetComponent<Bullet>().Ally = false;
                _shootDelay = 50;
            }
        }
    }

    private void AimAtPlayer()
    {
        if (_isInFireState)
        {
            if (transform.localScale.x == 1)
            {
                var dir = _playerTransform.position - _gunTransform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                var dir = _gunTransform.position - _playerTransform.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                _gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

    }

    private void CheckDeath()
    {
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReciveDamage(float DamageTaken)
    {
        _hp -= DamageTaken;
    }
}
