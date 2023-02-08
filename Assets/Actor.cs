using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : AABB
{
    public bool onGround;
    public bool hitLeft;
    public bool hitRight;
    public bool hitUp;
    public bool hitDown;

    public void Start()
    {
        base.Start();
    }

    public void WallChecks()
    {
        hitLeft = CollideAtSolid(transform.position - Vector3.right);
        hitRight = CollideAtSolid(transform.position + Vector3.right);
        hitUp = CollideAtSolid(transform.position + Vector3.up);
        hitDown = CollideAtSolid(transform.position - Vector3.up);
    }


    public void Move(Vector2 speed, bool deltaTime = true)
    {
        MoveX(speed.x * (deltaTime ? Time.deltaTime : 1));
        MoveY(speed.y * (deltaTime ? Time.deltaTime : 1));
    }

    float xRemainder;



    public void MoveX(float amount) 
    { 

        xRemainder += amount; 

        int move = Mathf.RoundToInt(xRemainder); 

        if (move != 0) 
        { 
            xRemainder -= move; 
            int sign = Mathf.RoundToInt(Mathf.Sign(move)); 
            while (move != 0) 
            { 
                if (!CollideAtSolid(transform.position + new Vector3(sign , 0,0))) 
                { 
                    //There is no Solid immediately beside us 
                    transform.Translate(Vector3.right * sign); 
                    move -= sign; 
                } 
                else 
                { 

                    break; 
                } 
            } 
        } 

    } 

    float yRemainder;

    public void MoveY(float amount) 
    {   

        yRemainder += amount; 

        int move = Mathf.RoundToInt(yRemainder); 


        if (move != 0) 
        { 
            yRemainder -= move; 
            int sign = Mathf.RoundToInt(Mathf.Sign(move)); 
            while (move != 0) 
            { 
                if (!CollideAtSolid(transform.position + new Vector3(0, sign  ,0))) 
                { 
                    //There is no Solid immediately beside us 
                    transform.Translate(Vector3.up * sign); 
                    move -= sign; 

                    onGround = false;

                } 
                else 
                {   
                    if(!onGround && hitDown)
                    {
                        onGround = true;
                        OnGroundHit();
                    }

                    break; 
                } 
            } 
        } 

    }   

    public void CheckOnGround()
    {
        if(onGround)
        {
            if(!hitDown)
            {
                onGround = false;
            }
        }
    }


    public virtual void OnGroundHit()
    {
        print("groundHit");
    }

}
