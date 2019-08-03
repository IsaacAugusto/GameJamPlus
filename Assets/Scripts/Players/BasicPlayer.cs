using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayer : Player, IDamageble<float>
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        Debug.Log(_hp);
        Debug.Log(base._hp);
    }

}
