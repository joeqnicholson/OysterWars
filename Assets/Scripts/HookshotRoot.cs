using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotRoot : Actor
{
    public float distance;
    public float verticalty;
    public float gravity = 1500;
    public float currentForce;
    public float moveTowardsSpeed;
    float zTurn;
    public LayerMask layerMask;
    public List<Point> points = new List<Point>();
    private Transform lastPointLooker;
    private bool lookAtLast;
    public float closeMod;
    public float farMod;
    private Vector3 lastPosition;
    public Vector3 currentVelocity;
    public float lerpedDistanceModifier;
    public GameObject startPoint;
    public GameObject PointObject;
    public float checkTime;
    private bool stopped;
    public float currentDistance;
    public Point currentPoint;
    public float pointsDistance;
    private float maxDistance = 600;
    public bool maxedOut;
    private float pointsTotalDistance;
    Colliders colliders;
    public List<Vector2> previousPositions = new List<Vector2>();


    void Start()
    {
        GetStartingStats(startPoint.GetComponent<Point>());
        colliders = FindObjectOfType<Colliders>();
        lastPointLooker = Instantiate(new GameObject("LastPointLooker"), transform.position, Quaternion.identity).transform;
    }

    void Update()
    {
        if(!stopped)
        {
            // if(lookAtLast)
            // {
            //     CheckForNoWalls();
            // }
            

            // Vector3 relativePos = points[points.Count-1].transform.position - transform.position;
            // float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            // zTurn = transform.eulerAngles.z;

            // CheckForWalls();



            currentDistance = Vector2.Distance(transform.position, points[points.Count - 1].transform.position);

            Vector2 lookAtVector = points[points.Count-1].transform.position;
            Vector2 myPosition = transform.position;
            Vector2 difference = (myPosition - lookAtVector).normalized;
            Vector2 perp = Vector2.Perpendicular(difference);
            Vector3 annoyingConversion = perp;
            float addition = 1/distance;

            verticalty = -difference.normalized.x;

            currentForce = Mathf.MoveTowards(currentForce, verticalty * gravity, moveTowardsSpeed * Mathf.Abs(verticalty) * Time.deltaTime);

            if(currentDistance != distance)
            {
                Vector3 dienow = -difference;
                transform.position += (dienow * (currentDistance - distance)) * Time.deltaTime;
            }

            transform.position += ((annoyingConversion * currentForce)) * Time.deltaTime;

            previousPositions.Add(myPosition);
            if(previousPositions.Count > 1000)
            {
                previousPositions.RemoveAt(0);
            }

            for(int i = 1; i < previousPositions.Count; i++)
            {
                Debug.DrawLine(previousPositions[i-1],previousPositions[i]);
            }

        }

    }



    public void GetStartingStats(Point startingPoint)
    {
        if(startingPoint.createdForce == 0)
        {
            currentForce = startingPoint.transform.position.x > transform.position.x ? 1 : -1;
        }
        else
        {
            currentForce = startingPoint.createdForce;  
        }

        Vector3 relativePos = startingPoint.transform.position - transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        zTurn = transform.eulerAngles.z;

        print(startingPoint.transform.position);
        print(startingPoint.normal);

        AddPoint(startingPoint);

    }
    

    public void AddPoint(Point newPoint)
    {
        points.Add(newPoint);
        distance = Vector2.Distance(transform.position, points[points.Count - 1].transform.position);
        lookAtLast = points.Count != 1; 
        currentPoint = points[points.Count-1];

        if(points.Count > 1)
        {

            Vector2 normal2 = points[points.Count-1].normal;
            Vector2 perp = Vector2.Perpendicular(normal2);
            Vector3 killMe = perp;
            newPoint.transform.position += killMe * 5;
        }

        newPoint.transform.position += newPoint.normal * 5;

        CalculateDistance();
    }

    public Point CreatePoint(Hit hitInfo)
    {
        Point newPoint = Instantiate(PointObject, Vector3Int.RoundToInt(hitInfo.point), Quaternion.identity).GetComponent<Point>();
        newPoint.createdForce = currentForce;
        newPoint.createdAngle = transform.eulerAngles.z;
        newPoint.normal = hitInfo.normal;


        return newPoint;
    }

    void RemovePoint()
    { 
        Destroy(points[points.Count-1].gameObject);
        points.RemoveAt(points.Count - 1);
        distance = Vector2.Distance(transform.position, points[points.Count-1].transform.position);
        lookAtLast = points.Count != 1; 
        currentPoint = points[points.Count-1];
        CalculateDistance();
        print("removed");
    }

    private void CalculateDistance()
    {
        float total = 0;
        for(int i = 1; i < points.Count; i++)
        {
            Vector3 iPos = points[i].transform.position;
            Vector3 iMinPos = points[i-1].transform.position;


            float thisDistance = Vector3.Distance(iPos,iMinPos);
            total += thisDistance;
        }

        pointsTotalDistance = total;
    }   

    void CheckForWalls()
    {

        Vector2 iPos = transform.position;
        Vector2 iMinusPos = points[points.Count-1].transform.position;
        Vector2 difference = (iPos - iMinusPos).normalized * 5; 

        float distance = Vector2.Distance(iPos - difference, iMinusPos + difference) - 5;

        Hit hit = Ray.CastTo(iPos - difference, iMinusPos + difference, colliders);

        if(hit != null)
        {
            AddPoint(CreatePoint(hit));
        }

        Debug.DrawLine(iPos - difference, iMinusPos + difference, Color.green);

    }

    void CheckForNoWalls()
    {

        Vector2 myPos = transform.position;
        Vector2 iMinus2Pos = points[points.Count-2].transform.position;
        Vector2 difference = (myPos - iMinus2Pos).normalized * 5; 

        float distance = Vector2.Distance(myPos - difference, iMinus2Pos + difference) - 5;

        Hit hit = Ray.CastTo(myPos - difference, iMinus2Pos + difference, colliders);

        if(hit != null)
        {
            Debug.DrawLine(myPos - difference, iMinus2Pos + difference, Color.blue);
        }
        else
        {
        
            bool higherThanPoint;

            if(points[points.Count - 2].transform.position.x < points[points.Count - 1].transform.position.x)
            {
                Debug.DrawLine(myPos - difference, iMinus2Pos + difference, Color.red);
                higherThanPoint = transform.eulerAngles.z < points[points.Count - 1].createdAngle;
            }
            else
            {
                Debug.DrawLine(myPos - difference, iMinus2Pos + difference, Color.white);
                higherThanPoint = transform.eulerAngles.z > points[points.Count - 1].createdAngle;
            }

            if(higherThanPoint)
            {
                RemovePoint();
            }

        }

        
    }

    public void Stop()
    {
        stopped = true;
    }

    public void Restart()
    {
        stopped = false;
    }

}









































































// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class HookshotRoot : Point
// {
//     public float distance;
//     public float verticalty;
//     public float gravity;
//     public float currentForce;
//     public float moveTowardsSpeed;
//     float zTurn;
//     public LayerMask layerMask;
//     public List<Point> points = new List<Point>();
//     private bool lookAtLast;
//     public float maxDistance;
//     public float closeMod;
//     public float farMod;
//     private Vector3 lastPosition;
//     public Vector3 currentVelocity;
//     public float lerpedDistanceModifier;
//     public GameObject startPoint;
//     public GameObject PointObject;
//     public float checkTime;
//     public bool bouncy = false;
//     public float bounciness = .75f;
//     private bool stopped;



//     void Start()
//     {


//         Vector2 vec = new Vector2(-1,-2);
//         print(vec.normalized);

//         GetStartingStats(startPoint.GetComponent<Point>());
//     }

//     void Update()
//     {
//         if(!stopped)
//         {


//             Vector3 relativePos = points[points.Count-2].transform.position - transform.position;
//             float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
//             transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

//             CheckForWalls();
//             CheckForNoWalls();
//             CheckCollision();


//             verticalty = transform.right.x;

//             currentForce = Mathf.MoveTowards(currentForce, verticalty * gravity, moveTowardsSpeed * Mathf.Abs(verticalty) * Time.deltaTime);
//         }

//     }



//     public void GetStartingStats(Point startingPoint)
//     {
//         if(startingPoint.createdForce == 0)
//         {
//             currentForce = startingPoint.transform.position.x > transform.position.x ? 1 : -1;
//         }
//         else
//         {
//             currentForce = startingPoint.createdForce;  
//         }

//         Vector3 relativePos = startingPoint.transform.position - transform.position;
//         float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
//         transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
//         zTurn = transform.eulerAngles.z;

//         print(startingPoint.transform.position);
//         print(startingPoint.normal);

//         AddPoint(startingPoint);
//         AddPoint(this);


//     }
    

//     public void AddPoint(Point newPoint, int index = 900)
//     {

//         if(index == 900)
//         {
//             points.Add(newPoint);
//         }
//         else
//         {
//             points.Insert(index, newPoint);
//         }

//         distance = Vector2.Distance(transform.position, points[points.Count - 1].transform.position);

//         lookAtLast = points.Count != 1; 

//     }

//     public Point CreatePoint(Hit hitInfo)
//     {
//         Vector3 hitNormal = hitInfo.normal * 5;
//         Point newPoint = Instantiate(PointObject, Vector3Int.RoundToInt(hitInfo.point) + hitNormal, Quaternion.identity).GetComponent<Point>();
//         newPoint.createdForce = currentForce;
//         newPoint.createdAngle = transform.eulerAngles.z;
//         newPoint.normal = hitInfo.normal;
//         newPoint.transform.parent = hitInfo.collider.gameObject.transform;
//         return newPoint;
//     }

//     void RemovePoint(int index)
//     { 
//         Destroy(points[index].gameObject);
//         points.RemoveAt(index);
//         distance = Vector2.Distance(transform.position, points[points.Count-1].transform.position);
//         lookAtLast = points.Count != 1; 
//         print("removed");
//     }   

//     void CheckForWalls()
//     {
//         if(points.Count > 1)
//         {
//             for(int i = 1; i < points.Count; i++ )
//             {
//                 Vector2 iPos = points[i].transform.position;
//                 Vector2 iMinusPos = points[i-1].transform.position;
//                 Vector2 difference = (iPos - iMinusPos).normalized * 5; 

//                 float distance = Vector2.Distance(iPos - difference, iMinusPos + difference) - 5;

//                 Hit hit = Physics2D.Raycast(iPos - difference, -difference, distance,  layerMask);

//                 if(hit)
//                 {
//                     AddPoint(CreatePoint(hit), i);
//                     break;
//                 }
//                 else
//                 {
                    
//                 }

//                 Debug.DrawLine(iPos - difference, iMinusPos + difference, Color.green);

//             }
//         }
//     }

//     void CheckForNoWalls()
//     {
//         for(int i = 2; i < points.Count; i++ )
//         {
//             Vector2 iPos = points[i].transform.position;
//             Vector2 iMinus2Pos = points[i-2].transform.position;
//             Vector2 difference = (iPos - iMinus2Pos).normalized * 5; 

//             float distance = Vector2.Distance(iPos - difference, iMinus2Pos + difference) - 5;

//             Hit hit = Physics2D.Raycast(iPos - difference, -difference, distance,  layerMask);

//             if(hit)
//             {
//                 Debug.DrawLine(transform.position, points[i - 2].transform.position, Color.blue);
//             }
//             else
//             {
//                 Debug.DrawLine(transform.position, points[i - 2].transform.position, Color.yellow);

//                 bool higherThanPoint;

//                 if(points[i - 1].createdForce >= 0)
//                 {
//                     higherThanPoint = transform.eulerAngles.z < points[i - 1].createdAngle;
//                 }
//                 else
//                 {
//                     higherThanPoint = zTurn > points[i - 1].createdAngle;
//                 }

//                 if(higherThanPoint)
//                 {
//                     RemovePoint(i-1);
//                     break;
//                 }

//             }
//         }
//     }

    

//     void CheckCollision()
//     {
//         if(checkTime > -1)
//         {
//             checkTime -= Time.deltaTime;
//         }


//         Hit hit = Physics2D.CircleCast(transform.position, 16, transform.right, Mathf.Sign(currentForce), layerMask);

//         if(hit && checkTime < 0)
//         {

//             if(hit.normal.y < -0.8f || bouncy)
//             {
//                 currentForce *= -1 * bounciness;
//             }
//             else
//             {
//                 currentForce = 0;
//             }
            
//             checkTime = 0.5f;
//         }
//     }

//     public void Stop()
//     {
//         stopped = true;
//     }

//     public void Restart()
//     {
//         stopped = false;
//     }

// }

