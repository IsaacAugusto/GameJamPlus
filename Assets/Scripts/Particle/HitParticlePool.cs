using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticlePool : MonoBehaviour
{
    [SerializeField] private GameObject _particlePrefab;
    public static HitParticlePool Instance;
    private List<GameObject> _particleList = new List<GameObject>();
    private GameObject _particlePool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
    void Start()
    {
        _particlePool = new GameObject("HitParticlePool");   
    }

    void Update()
    {
        
    }
    public GameObject GetHitParticle()
    {
        foreach (GameObject particle in _particleList)
        {
            if (!particle.activeInHierarchy)
            {
                particle.SetActive(true);
                return particle;
            }
        }
        GameObject Particle = Instantiate(_particlePrefab, _particlePool.transform);
        _particleList.Add(Particle);
        return Particle;
    }
}
