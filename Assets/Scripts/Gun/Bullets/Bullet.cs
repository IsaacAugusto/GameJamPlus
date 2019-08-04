////////using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage = 1 ;
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;
    private float _startTime;
    public bool Right = true;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _startTime = Time.time;
    }

    void Update()
    {
        GoFoward();
        DesactivateBulletOverTime();
    }

    private void GoFoward()
    {
        _rb.velocity = transform.TransformDirection(Vector2.right) * _speed;
    }

    private void DesactivateBullet()
    {
        gameObject.SetActive(false);
    }

    private void DesactivateBulletOverTime()
    {
        if (Time.time - _startTime >= 3)
        {
            DesactivateBullet();
        }
    }

    private void DealDamage(float damage, Collider2D collision)
    {
        if (collision.gameObject.GetComponent<IDamageble<float>>() != null)
        {
            collision.gameObject.GetComponent<IDamageble<float>>().ReciveDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DealDamage(Damage, collision);
        var particle = HitParticlePool.Instance.GetHitParticle();
        particle.transform.position = transform.position;
        DesactivateBullet();
    }
}
