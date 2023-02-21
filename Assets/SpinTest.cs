using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinTest : Actor
{
    public List<Vector2> previousPositions = new List<Vector2>();
    float startDistance;
    void Start()
    {


 

        Vector2 lookAtVector = Vector2.zero;
        Vector2 myPosition = transform.position;
        previousPositions.Add(myPosition);

        startDistance = Vector2.Distance(lookAtVector, myPosition);


    }


    void Update()
    {
        Vector2 lookAtVector = Vector2.zero;
        Vector2 myPosition = transform.position;
        Vector2 difference = (myPosition - lookAtVector).normalized;
        Vector2 perp = Vector2.Perpendicular(difference);
        Vector3 annoyingConversion = perp;
        Vector3 fuckOff = difference;
        float distance = Vector3.Distance(transform.position, Vector3.zero);
        float addition = 1/distance;



        if(distance != startDistance)
        {
            Vector3 dienow = -difference;
            Move(dienow * (distance - startDistance));
        }

        Move((-fuckOff/1.95f) + (annoyingConversion * 550));

        previousPositions.Add(myPosition);

        for(int i = 1; i < previousPositions.Count; i++)
        {
            Debug.DrawLine(previousPositions[i-1],previousPositions[i]);
        }
    }
}
