using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTest : Actor
{
    public float x;
    public float y;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move(new Vector2(x,y));
        WallChecks();
    }
}
