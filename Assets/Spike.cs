using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Trigger
{
    public Vector2 direction;

    public void Awake()
    {
        transform.parent = null;

    }

    public void Start()
    {
        Vector3 move = direction;
        transform.position += move * 4; 
    }
}
