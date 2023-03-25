using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ray
{
    public static Hit CastTo(Vector3 startPos, Vector3 endPos, Colliders colliders, bool triggers = false)
    {

        Vector3 direction = endPos - startPos;
        bool oneTimeOnly = direction.magnitude < 2;
        direction = Vector3.ClampMagnitude(direction, 1f);
        Vector3 checkPos = startPos;

        if(Vector3.Distance(checkPos,endPos) > 2)
        {
            while(Vector3.Distance(checkPos,endPos) > 2)
            {

                foreach(Solid solidBox in colliders.solids)
                {

                    if(solidBox.Contains(checkPos))
                    {

                        Hit hit = new Hit()
                        {
                            point = checkPos,
                            normal = -direction,
                            aabb = solidBox
                        };

                        return hit;          
                    } 
                }

                if(triggers)
                {
                    foreach(Trigger triggerBox in colliders.triggers)
                    {

                        if(triggerBox.Contains(checkPos))
                        {

                            Hit hit = new Hit()
                            {
                                point = checkPos,
                                normal = -direction,
                                aabb = triggerBox
                            };

                            return hit;          
                        }

                        
                    }
                }

                checkPos += direction;

            }
        }



        return null;
    }
}

public class Hit
{
    public Vector3 point;
    public Vector3 normal;
    public AABB aabb;
}
