using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Bullet
{
    void Update()
    {
        base.Update();

        if(Speed.y < 0) transform.localScale = new Vector3(2,2,1);
    }
}
