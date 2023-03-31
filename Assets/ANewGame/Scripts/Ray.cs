using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ray
{
    public static Hit CastTo(Vector3 startPos, Vector3 endPos, Colliders colliders, bool triggers = false)
    {

        Vector3 vRayStart = startPos;
		Vector3 vRayDir = (endPos - startPos).normalized;

        Vector3 vRayUnitStepSize = new Vector3(Mathf.Sqrt(1 + (vRayDir.y / vRayDir.x) * (vRayDir.y / vRayDir.x)), Mathf.Sqrt(1 + (vRayDir.x / vRayDir.y) * (vRayDir.x / vRayDir.y)), 0);
		Vector3 vMapCheck = vRayStart;
		Vector3 vRayLength1D;
		Vector3 vStep;

		// Establish Starting Conditions
		if (vRayDir.x < 0)
		{
			vStep.x = -1;
			vRayLength1D.x = (vRayStart.x - vMapCheck.x) * vRayUnitStepSize.x;
		}
		else
		{
			vStep.x = 1;
			vRayLength1D.x = ((vMapCheck.x + 1) - vRayStart.x) * vRayUnitStepSize.x;
		}

		if (vRayDir.y < 0)
		{
			vStep.y = -1;
			vRayLength1D.y = (vRayStart.y - vMapCheck.y) * vRayUnitStepSize.y;
		}
		else
		{
			vStep.y = 1;
			vRayLength1D.y = (vMapCheck.y + 1 - vRayStart.y) * vRayUnitStepSize.y;
		}

        bool bTileFound = false;
		float fMaxDistance = Vector3.Distance(startPos, endPos);
		float fDistance = 0.0f;

		while (!bTileFound && fDistance < fMaxDistance)
		{
			// Walk along shortest path
			if (vRayLength1D.x < vRayLength1D.y)
			{
				vMapCheck.x += vStep.x;
				fDistance = vRayLength1D.x;
				vRayLength1D.x += vRayUnitStepSize.x;
			}
			else
			{
				vMapCheck.y += vStep.y;
				fDistance = vRayLength1D.y;
				vRayLength1D.y += vRayUnitStepSize.y;
			}

			// Test tile at new test point
			foreach(Solid solid in colliders.solids)
            {
                if(solid.Contains(vMapCheck))
                {
                    Hit hit = new Hit();
                    hit.normal = vRayDir;
                    hit.point = vMapCheck;
                    hit.aabb = solid;
                    return hit;


                }
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
