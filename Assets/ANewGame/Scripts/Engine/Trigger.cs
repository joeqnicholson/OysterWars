using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : AABB
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {

        Debug.DrawLine(TopRight(), TopLeft(), Color.blue);
        Debug.DrawLine(TopRight(), BottomRight(), Color.blue);
        Debug.DrawLine(TopLeft(), BottomLeft(), Color.blue);
        Debug.DrawLine(BottomLeft(), BottomRight(), Color.blue);

    }
}
