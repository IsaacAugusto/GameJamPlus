using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    override protected void Start()
    {
        base.Start();
        _damage = 1;
    }

    override protected void Update()
    {
        base.Update();
    }
}
