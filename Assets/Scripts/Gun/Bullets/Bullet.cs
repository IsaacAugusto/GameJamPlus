using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage;
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;
    private float _startTime;
    public bool Ally = true;
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
        if (Ally)
        {
            _rb.velocity = transform.right * _speed;
        }
        else
        {
            _rb.velocity = -transform.right * _speed;
        }
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

    private void DealDamage(float damage, Collision2D collision)
    {
        if (collision.gameObject.GetComponent<IDamageble<float>>() != null)
        {
            collision.gameObject.GetComponent<IDamageble<float>>().ReciveDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DealDamage(Damage, collision);
        var particle = HitParticlePool.Instance.GetHitParticle();
        particle.transform.position = transform.position;
        DesactivateBullet();
    }
}
