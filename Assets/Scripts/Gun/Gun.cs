using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour {
  [SerializeField] private Transform _bulletSpawnPosition;
  private ReloadInterface _reloadInterface;
  protected float _damage = 1;
  protected float _magSize = 12;
  protected float _reloadTime = 5;
  protected float _shootDelay = .2f;
  private float _timeSinceLastShoot;
  private float _delay;
  [SerializeField] private bool _canShoot;
  [SerializeField] private float _bulletsShooted;
  private SpriteRenderer _sprite;
  private SpriteRenderer _playerSprite;
  private Animator _animator;

  virtual protected void Start() {
    _reloadInterface = FindObjectOfType<ReloadInterface>();
    _sprite = GetComponent<SpriteRenderer>();
    _playerSprite = FindObjectOfType<Player>().GetComponent<SpriteRenderer>();
    _animator = GetComponent<Animator>();
    _delay = _shootDelay;
    _canShoot = true;
  }

  virtual protected void Update() {
    AimAtMouse();
    Fire();
    DelayCount();
    ReloadSystem();
  }

  private void AimAtMouse() {
    var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    _sprite.flipY = (Mathf.Abs(angle) > 90);
    _playerSprite.flipX = (Mathf.Abs(angle) > 90);
    transform.rotation = Quaternion.AngleAxis(-angle, Vector3.back);

  }

  private void DelayCount() {
    if (_delay >= 0) {
      _delay -= Time.deltaTime;
    }
  }



  private void ReloadSystem() {
    if (_bulletsShooted > 0) {
      if (Time.time - _timeSinceLastShoot >= _reloadTime / 6) {
        _bulletsShooted -= Time.deltaTime * 5;
      }
      if (_bulletsShooted >= _magSize && _canShoot) {
        _canShoot = false;
        StartCoroutine(ReloadCoroutine(_reloadTime));
      }
    }

    _reloadInterface.ReloadBarFill(_bulletsShooted, _magSize);
  }

  private IEnumerator ReloadCoroutine(float reloadTime) {
    _canShoot = false;
    yield return new WaitForSeconds(reloadTime);
    _bulletsShooted = 0;
    _canShoot = true;
  }

  protected void Fire() {
    if (Input.GetMouseButton(0)) {
      if (_delay < 0 && _canShoot) {
        _timeSinceLastShoot = Time.time;
        var bullet = BulletPool.Instance.GetBulletFromPool();
        bullet.GetComponent<Bullet>().Damage = _damage;
        bullet.transform.position = _bulletSpawnPosition.position;
        bullet.transform.rotation = transform.rotation;
        _delay = _shootDelay;
        _bulletsShooted++;

        _animator.SetTrigger("Shoot");
      }
    }
  }
}
