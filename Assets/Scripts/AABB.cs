using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB : MonoBehaviour
{
    public Vector3 size;
    public Solid[] solids = new Solid[0];
    public Actor[] actors = new Actor[0];


    public void Start()
    {
        if(size == Vector3.zero)
        {
            size = transform.localScale;
        }   
    }

    public bool CollideAtSolid(Vector3 position)
    {

        foreach(Solid solid in solids)
        {
            if(CollidesWith(solid, position))
            {
                return true;
            }
        }

        return false;

    }

    public bool Contains(Vector3 position)
    {
        bool xTouching = (position.x > xMin() && position.x < xMax());
        bool yTouching = (position.y > yMin() && position.y < yMax());

        return xTouching && yTouching;
    }

    public bool CollidesWith(AABB box, Vector3 position)
    {
        bool xTouching = (xMaxAt(position) > box.xMin() && xMaxAt(position) < box.xMax()) || (xMinAt(position) > box.xMin() && xMinAt(position) < box.xMax());
        bool yTouching = (yMaxAt(position) > box.yMin() && yMaxAt(position) < box.yMax()) || (yMinAt(position) > box.yMin() && yMinAt(position) < box.yMax());
        return xTouching && yTouching;
    }

    public Vector3 Center()
    {
        return transform.position;
    }

    public Vector3 Top()
    {
        return transform.position + (Vector3.up * size.y/2f);
    }

    public Vector3 Bottom()
    {
        return transform.position - (Vector3.up * size.y/2f);
    }

    public Vector3 Right()
    {
        return transform.position + (Vector3.right * size.x/2f);
    }

    public Vector3 Left()
    {
        return transform.position - (Vector3.right * size.x/2f);
    }

    public Vector3 TopRight()
    {
        return Top() + Right();
    }

    public Vector3 TopLeft()
    {
        return Top() + Left();
    }

    public Vector3 BottomRight()
    {
        return Bottom() + Right();
    }

    public Vector3 BottomLeft()
    {
        return Bottom() + Left();
    }

    public float yMax()
    {
        return Top().y;
    }

    public float yMin()
    {
        return Bottom().y;
    }

    public float xMax()
    {
        return Right().x;
    }

    public float xMin()
    {
        return Left().x;
    }

    // Stats at specific position

    public Vector3 TopAt(Vector3 pos)
    {
        return pos + (Vector3.up * size.y/2f);
    }

    public Vector3 BottomAt(Vector3 pos)
    {
        return pos - (Vector3.up * size.y/2f);
    }

    public Vector3 RightAt(Vector3 pos)
    {
        return pos + (Vector3.right * size.x/2f);
    }

    public Vector3 LeftAt(Vector3 pos)
    {
        return pos - (Vector3.right * size.x/2f);
    }

    public Vector3 TopRightAt(Vector3 pos)
    {
        return TopAt(pos) + RightAt(pos);
    }

    public Vector3 TopLeftAt(Vector3 pos)
    {
        return TopAt(pos) + LeftAt(pos);
    }

    public Vector3 BottomRightAt(Vector3 pos)
    {
        return BottomAt(pos) + RightAt(pos);
    }

    public Vector3 BottomLeftAt(Vector3 pos)
    {
        return BottomAt(pos) + LeftAt(pos);
    }

    public float yMaxAt(Vector3 pos)
    {
        return TopAt(pos).y;
    }

    public float yMinAt(Vector3 pos)
    {
        return BottomAt(pos).y;
    }

    public float xMaxAt(Vector3 pos)
    {
        return RightAt(pos).x;
    }

    public float xMinAt(Vector3 pos)
    {
        return LeftAt(pos).x;
    }




}
