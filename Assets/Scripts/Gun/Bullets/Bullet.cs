using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Rigidbody2D _rb;
    private float _startTime;
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
        _rb.velocity = transform.right * _speed;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DesactivateBullet();
    }
}
