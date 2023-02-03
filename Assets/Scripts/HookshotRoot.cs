using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotRoot : MonoBehaviour
{
    public float distance;
    public float verticalty;
    public float gravity;
    public float currentForce;
    public float moveTowardsSpeed;
    float zTurn;
    public LayerMask layerMask;
    public List<Point> points = new List<Point>();
    private Transform lastPointLooker;
    private bool lookAtLast;
    public float maxDistance;
    public float closeMod;
    public float farMod;
    private Vector3 lastPosition;
    public Vector3 currentVelocity;
    public float lerpedDistanceModifier;
    public GameObject startPoint;
    public GameObject PointObject;
    public float checkTime;
    public bool bouncy = false;
    public float bounciness = .75f;
    private bool stopped;



    void Start()
    {
        GetStartingStats(startPoint.GetComponent<Point>());
        lastPointLooker = Instantiate(new GameObject("LastPointLooker"), transform.position, Quaternion.identity).transform;
    }

    void Update()
    {
        if(!stopped)
        {
            lastPosition = transform.position;

            if(lookAtLast)
            {
                // lastPointLooker rotation
                lastPointLooker.position = transform.position;

                Vector3 relativeLookPos = points[points.Count - 2].transform.position - lastPointLooker.position;
                float angleLooker = Mathf.Atan2(relativeLookPos.y, relativeLookPos.x) * Mathf.Rad2Deg;
                lastPointLooker.rotation = Quaternion.AngleAxis(angleLooker, Vector3.forward); 

                CheckForNoWalls();
            }
            

            // transform rotation
            Vector3 relativePos = points[points.Count-1].transform.position - transform.position;
            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            CheckForWalls();
            CheckCollision();
            transform.rotation = Quaternion.Euler(0,0,zTurn);
            transform.position = points[points.Count-1].transform.position - transform.right * distance;

            verticalty = transform.right.x;

            currentForce = Mathf.MoveTowards(currentForce, verticalty * gravity, moveTowardsSpeed * Mathf.Abs(verticalty) * Time.deltaTime);

            lerpedDistanceModifier = Mathf.Lerp(closeMod, farMod, distance/ maxDistance);

            zTurn += currentForce * lerpedDistanceModifier * Time.deltaTime;
            
            currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
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
    }

    public Point CreatePoint(RaycastHit2D hitInfo)
    {
        Point newPoint = Instantiate(PointObject, hitInfo.point, Quaternion.identity).GetComponent<Point>();
        newPoint.createdForce = currentForce;
        newPoint.createdAngle = zTurn;
        newPoint.normal = hitInfo.normal;
        newPoint.transform.parent = hitInfo.collider.gameObject.transform;
        return newPoint;
    }

    void RemovePoint()
    { 
        Destroy(points[points.Count-1].gameObject);
        points.RemoveAt(points.Count - 1);

        distance = Vector2.Distance(transform.position, points[points.Count-1].transform.position);
        lookAtLast = points.Count != 1; 
        print("removed");
    }   

    void CheckForWalls()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, distance - 2, layerMask);
        
        if(hit)
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            AddPoint(CreatePoint(hit));
        }
        else
        {
            Debug.DrawLine(transform.position, points[points.Count - 1].transform.position, Color.green);
        }   
    }

    void CheckForNoWalls()
    {

        float lookerDistance = Vector2.Distance(transform.position, points[points.Count - 2].transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, lastPointLooker.right, lookerDistance - 2, layerMask);
        
        if(hit)
        {
            Debug.DrawLine(transform.position, points[points.Count - 2].transform.position, Color.blue);
        }
        else
        {

            Debug.DrawLine(transform.position, points[points.Count - 2].transform.position, Color.yellow);

            bool higherThanPoint;

            if(points[points.Count - 1].createdForce >= 0)
            {
                higherThanPoint = zTurn < points[points.Count - 1].createdAngle;
            }
            else
            {
                higherThanPoint = zTurn > points[points.Count - 1].createdAngle;
            }

            if(higherThanPoint)
            {
                RemovePoint();
            }
        }   
    }

    void CheckCollision()
    {
        if(checkTime > -1)
        {
            checkTime -= Time.deltaTime;
        }


        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 16, transform.right, Mathf.Sign(currentForce), layerMask);

        if(hit && checkTime < 0)
        {

            if(hit.normal.y < -0.8f || bouncy)
            {
                currentForce *= -1 * bounciness;
            }
            else
            {
                currentForce = 0;
            }
            
            checkTime = 0.5f;
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
