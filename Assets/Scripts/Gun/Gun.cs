using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnPosition;
    protected float _damage;
    protected float _magSize = 12;
    protected float _reloadTime = 5;
    protected float _shootDelay = .2f;
    private float _timeSinceLastShoot;
    private float _delay;
    [SerializeField] private bool _canShoot;
    [SerializeField] private float _bulletsShooted;
    virtual protected void Start()
    {
        _delay = _shootDelay;
        _canShoot = true;
    }

    virtual protected void Update()
    {
        AimAtMouse();
        Fire();
        DelayCount();
        ReloadSystem();
    }

    private void AimAtMouse()
    {
        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void DelayCount()
    {
        if (_delay >= 0)
        {
            _delay -= Time.deltaTime;
        }
    }

    private void ReloadSystem()
    {
        if (_bulletsShooted > 0)
        {
            if (Time.time - _timeSinceLastShoot >= _reloadTime / 6)
            {
                _bulletsShooted -= Time.deltaTime * 5;
            }
            if (_bulletsShooted >= 12 && _canShoot)
            {
                _canShoot = false;
                StartCoroutine(ReloadCoroutine(_reloadTime));
            }
        }
    }

    private IEnumerator ReloadCoroutine(float reloadTime)
    {
        _canShoot = false;
        yield return new WaitForSeconds(reloadTime);
        _bulletsShooted = 0;
        _canShoot = true;
    }

    protected void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            if (_delay < 0 && _canShoot)
            {
                _timeSinceLastShoot = Time.time;
                var bullet = BulletPool.Instance.GetBulletFromPool();
                bullet.transform.position = _bulletSpawnPosition.position;
                bullet.transform.rotation = transform.rotation;
                _delay = _shootDelay;
                _bulletsShooted++;
            }
        }
    }
}
