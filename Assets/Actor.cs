using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public Transform boxStats;
    public Vector2 direction;
    public float x;
    public float y;
    public float moveX;
    public float moveY;


    private void Update()
    {
        Move();
    }


    private void Move()
    {
        MoveX(moveX);
        MoveY(moveY);

        transform.position = new Vector3(x,y,0);
    }


    private void MoveX(float hsp)
    {
        float dt = Time.deltaTime;

        RaycastHit2D hit = BoxCastDrawer.BoxCastAndDraw(transform.position + Vector3.right * hsp * dt, transform.localScale, 0, Vector3.zero);

        if(hit)
        {
            while(!Physics2D.BoxCast(transform.position + Vector3.up * Mathf.Sign(hsp), transform.localScale, 0, Vector3.zero))
            {
                x = x + Mathf.Sign(hsp);
            }
            hsp = 0;

        }

        x = x + hsp * dt;
    }    

    private void MoveY(float vsp)
    {

        float dt = Time.deltaTime;

        RaycastHit2D hit = BoxCastDrawer.BoxCastAndDraw(transform.position + Vector3.up * vsp * dt, transform.localScale, 0, Vector3.zero);

        if(hit)
        {
            while(!Physics2D.BoxCast(transform.position + Vector3.up * Mathf.Sign(vsp), transform.localScale, 0, Vector3.zero))
            {
                y = y + Mathf.Sign(vsp);
            }
            vsp = 0;

        }

        y = y + vsp * dt;

    }    

}
