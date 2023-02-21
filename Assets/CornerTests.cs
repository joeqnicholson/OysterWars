using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerTests : MonoBehaviour
{
    public Solid solid;
    public Transform redDot;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        redDot.transform.position = solid.ClosestCorner(transform.position);
    }
}
