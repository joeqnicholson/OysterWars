using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotRoot : MonoBehaviour
{
    public float currentDistance;
    public float zTurn;
    public List<Point> points = new List<Point>();
    private bool lookAtLast;
    public GameObject PointObject;
    public Point currentPoint;
    private float maxDistance = 600;
    public bool maxedOut;
    public float pointsTotalDistance;
    private Colliders colliders;
    public bool active;

    private Vector3 shootPoint;

    private LineRenderer rope;



    void Start()
    {
        colliders = FindObjectOfType<Colliders>();
        rope = GetComponent<LineRenderer>();
    }

    void Update()
    {

        if(active)
        {
            if(lookAtLast)
            {
                CheckForNoWalls();
                CheckForNoWallsPoints();
                CheckForWallsPoints();
            }
            

            Vector3 relativePos = points[0].transform.position - transform.position;
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            zTurn = angle;

            CheckForWalls();
        }

        


    }

    public void Reset()
    {
        active = false;

        for(int i = 0; i < points.Count; i++ )
        {
            Destroy(points[i].gameObject);
        }

        points.Clear();

        Vector3[] positions = new Vector3[0];
        rope.positionCount = 0;
        rope.SetPositions(positions);

        lookAtLast = false;
        currentDistance = 0;
        currentPoint = null;
        pointsTotalDistance = 0;
    }


    public void GetStartingStats(Point newPoint, Hit hitInfo)
    {
        Vector3 relativePos = newPoint.transform.position - transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        zTurn = angle;
        newPoint.createdAngle = angle;

        if(hitInfo.aabb)
        {
            newPoint.aabb = hitInfo.aabb;

        }
        
        newPoint.transform.parent = hitInfo.aabb.transform;

        AddPoint(newPoint);
        active = true;
    }
    

    public void AddPoint(Point newPoint, int index = 0)
    {
        points.Insert(index, newPoint);

        lookAtLast = points.Count != 1; 
        currentPoint = points[0];
        CalculateDistance();
    }

    public Point CreatePoint(Hit hitInfo)
    {
        Vector3 positionToCreate;
        hitInfo.normal = hitInfo.aabb.CornerNormal(hitInfo.aabb.ClosestCorner(hitInfo.point));
        positionToCreate = hitInfo.aabb.ClosestCorner(hitInfo.point);
        Point newPoint = Instantiate(PointObject, positionToCreate + hitInfo.normal * 3, Quaternion.identity).GetComponent<Point>();
        newPoint.createdAngle = zTurn;
        newPoint.normal = hitInfo.normal;
        newPoint.aabb = hitInfo.aabb;
        newPoint.transform.parent = hitInfo.aabb.transform;
        return newPoint;
    }

    void RemovePoint(int index = 8000)
    { 
        if(index == 8000) index = points.Count - 1;
        Destroy(points[index].gameObject);
        points.RemoveAt(index);

        lookAtLast = points.Count != 1; 
        currentPoint = points[0];
        CalculateDistance();
        print("removed");
    }


    private void CalculateDistance()
    {
        currentDistance = Vector3.Distance(transform.position, points[0].transform.position);
        float total = 0;
        for(int i = 1; i < points.Count; i++)
        {
            Vector3 iPos = points[i].transform.position;
            Vector3 iMinPos = points[i-1].transform.position;


            float thisDistance = Vector3.Distance(iPos,iMinPos);
            total += thisDistance;
        }

        pointsTotalDistance = total + currentDistance;;
    }   

    void CheckForWalls()
    {

        Vector2 iPos = transform.position;
        Vector2 iPlusPos = points[0].transform.position;
        Vector2 difference = (iPos - iPlusPos).normalized * 5; 

        Hit hit = Ray.CastTo(iPos - difference, iPlusPos + difference, colliders);

        

        if(hit != null)
        {
            AddPoint(CreatePoint(hit));
        }

        // Debug.DrawLine(iPos - difference, iPlusPos + difference, Color.green);

    }



    void CheckForNoWalls()
    {

        Vector2 myPos = transform.position;
        Vector2 iPlus2Pos = points[1].transform.position;
        Vector2 difference = (myPos - iPlus2Pos).normalized * 5; 
        
        Hit hit = Ray.CastTo(myPos - difference, iPlus2Pos + difference, colliders);

        if(hit != null)
        {
            // Debug.DrawLine(myPos - difference, iPlus2Pos + difference, Color.blue);
        }
        else
        {
        
            bool higherThanPoint;

            if(points[1].transform.position.x < points[0].transform.position.x)
            {
                // Debug.DrawLine(myPos - difference, iPlus2Pos + difference, Color.cyan);
                higherThanPoint = zTurn > points[0].createdAngle;
            }
            else
            {
                // Debug.DrawLine(myPos - difference, iPlus2Pos + difference, Color.yellow);
                higherThanPoint = zTurn < points[0].createdAngle;
            }

            if(higherThanPoint || currentDistance < 20)
            {
                RemovePoint(0);
            }

        }

        
    }

    void CheckForWallsPoints()
    {
        if(points.Count > 1)
        {
            for(int i = 0; i < points.Count - 1; i++ )
            {
                Vector2 iPos = points[i].transform.position;
                Vector2 iPlusPos = points[i+1].transform.position;
                Vector2 difference = (iPos - iPlusPos).normalized * 5; 

                Hit hit = Ray.CastTo(iPos - difference, iPlusPos + difference, colliders);

                if(hit != null)
                {
                    print(i - 1);
                    AddPoint(CreatePoint(hit), i + 1);
                    break;
                }

                // Debug.DrawLine(iPos - difference, iPlusPos + difference, Color.green);

            }
        }
    }


    void CheckForNoWallsPoints()
    {
        if(points.Count > 2)
        {
            for(int i = 0; i < points.Count - 2; i++ )
            {
                Vector2 iPos = points[i].transform.position;
                Vector2 iPlus2Pos = points[i+2].transform.position;
                Vector2 difference = (iPos - iPlus2Pos).normalized * 5; 

                Hit hit = Ray.CastTo(iPos - difference, iPlus2Pos + difference, colliders);

                if(hit != null)
                {
                    // Debug.DrawLine(iPos - difference, iPlus2Pos + difference, Color.blue);
                }
                else
                {
                    

                     bool higherThanPoint;

                    if(points[i+2].transform.position.x < points[i + 1].transform.position.x)
                    {
                        // Debug.DrawLine(iPos - difference, iPlus2Pos + difference, Color.cyan);
                        higherThanPoint = points[i].createdAngle > points[i + 1].createdAngle;
                    }
                    else
                    {
                        // Debug.DrawLine(iPos - difference, iPlus2Pos + difference, Color.yellow);
                        higherThanPoint = points[i].createdAngle < points[i + 1].createdAngle;
                    }

                    if(higherThanPoint)
                    {
                        RemovePoint(i+1);
                    }

                }
            }
        }
        
    }

    public void RenderRope(Vector3 newPoint)
    {

        Vector3[] positions = new Vector3[points.Count+1];
        int x = points.Count - 1;

        for(int i = 0; i < points.Count; i++ )
        {
            positions[x] = points[i].transform.position;

            x-=1;
        }

        positions[points.Count] = newPoint;
        rope.positionCount = positions.Length;
        

        rope.SetPositions(positions);
    }
}




