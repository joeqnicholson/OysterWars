using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solid : AABB
{
    public bool active = false;
    
    public void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        if(active)
        {
            Debug.DrawLine(TopRight(), TopLeft(), Color.green);
            Debug.DrawLine(TopRight(), BottomRight(), Color.green);
            Debug.DrawLine(TopLeft(), BottomLeft(), Color.green);
            Debug.DrawLine(BottomLeft(), BottomRight(), Color.green);
        }
        else
        {
            Debug.DrawLine(TopRight(), TopLeft(), Color.red);
            Debug.DrawLine(TopRight(), BottomRight(), Color.red);
            Debug.DrawLine(TopLeft(), BottomLeft(), Color.red);
            Debug.DrawLine(BottomLeft(), BottomRight(), Color.red);
        }
    }
}
