using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    protected float _damage;
    virtual protected void Start()
    {
        
    }

    virtual protected void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {

    }
}
