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
    private Transform _playerTransform;
    private Vector2 dir;

    [SerializeField] private Transform _bulletSpawnTransform;
    [SerializeField] private Transform _gunTransform;
    private float _shootDelay;
    private RaycastHit2D hit;

    private Vector3 _checkGroundDir;
    private Vector3 _checkWallDir;


    private void Start()
    {
        if (!_playerDetected)
        _playerTransform = FindObjectOfType<Player>().transform;
    }
    void Update() {
        _checkWallDir = transform.TransformDirection(Vector2.right);
        dir = transform.TransformDirection(Vector2.right);
        _checkGroundDir = Quaternion.AngleAxis(_checkGroundAngle, Vector3.back) * dir;

        if (!CheckGround() || CheckWall())
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
    private void FlipDirection() {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void Move()
    {
        if (!_playerDetected)
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
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector2.right) * _checkGroundDistance);
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector2.right) * _checkWallDistance);
        //Gizmos.DrawWireSphere(transform.position, _detectDistance);
        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, (_playerTransform.position - transform.position) * _detectDistance);
    }

    private void DetectPlayer()
    {
        hit = Physics2D.Raycast(transform.position, _playerTransform.position - transform.position, _detectDistance, LayerMask.GetMask("Ground"));
        if (hit == true)
        {
            _playerDetected = false;
        }
        else
        {
            _playerDetected = Physics2D.OverlapCircle(transform.position, _detectDistance, LayerMask.GetMask("Player"));
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
        _shootDelay -= Time.deltaTime;
        if (_shootDelay <= 0)
        {
            var bullet = BulletPool.Instance.GetBulletFromPool();
            bullet.GetComponent<Bullet>().Damage = 1;
            bullet.transform.position = _bulletSpawnTransform.position;
            bullet.transform.rotation = _gunTransform.rotation;
            _shootDelay = .4f;
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
                _gunTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                var dir = _playerTransform.position - _gunTransform.position;
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
