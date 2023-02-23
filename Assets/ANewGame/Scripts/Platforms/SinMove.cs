using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMove : MonoBehaviour
{
    public float distanceX;
    public float distanceY;
    public float speedX;
    public float speedY;
    public float index;
    public Vector3 startPos;

    public void Start()
    {
        startPos = transform.position;
    }

    public void Update()
    {
        index += Time.deltaTime;
        float x = distanceX*Mathf.Cos (speedX*index);
        float y = Mathf.Abs (distanceY*Mathf.Sin (speedY*index));
        transform.localPosition= startPos + new Vector3(x,y,0);
    }

}
