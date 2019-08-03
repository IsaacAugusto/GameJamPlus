using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private GameObject _bulletPrefab;
    private GameObject _bulletPool;
    private List<GameObject> _bullets = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else if (Instance != this)
        {
            Destroy(this);
        }
    }
    void Start()
    {
        _bulletPool = new GameObject("BulletPool");
    }

    void Update()
    {

    }

    public GameObject GetBulletFromPool()
    {
        foreach (GameObject bullet in _bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        GameObject _bullet = Instantiate(_bulletPrefab, _bulletPool.transform);
        _bullets.Add(_bullet);
        return _bullet;
    }

}
