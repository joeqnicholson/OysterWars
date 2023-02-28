using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : AABB
{
    
    public void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(TopRight(), TopLeft(), Color.green);
        Debug.DrawLine(TopRight(), BottomRight(), Color.green);
        Debug.DrawLine(TopLeft(), BottomLeft(), Color.green);
        Debug.DrawLine(BottomLeft(), BottomRight(), Color.green);
    }
}
