using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public bool onGround;


    public Vector2 size;

    public bool hitLeft;
    public bool hitRight;
    public bool hitUp;
    public bool hitDown;


    public void WallChecks()
    {
        hitLeft = Physics2D.BoxCast(transform.position - Vector3.right , size, 0, Vector3.zero);
        hitRight = Physics2D.BoxCast(transform.position + Vector3.right , size, 0, Vector3.zero);
        hitUp = Physics2D.BoxCast(transform.position + Vector3.up , size, 0, Vector3.zero);
        hitDown = Physics2D.BoxCast(transform.position - Vector3.up , size, 0, Vector3.zero);
    }


    public void Move(Vector2 speed, bool deltaTime = true)
    {
        MoveX(speed.x * (deltaTime ? Time.deltaTime : 1));
        MoveY(speed.y * (deltaTime ? Time.deltaTime : 1));
    }

    float xRemainder;

    public bool collideAt(Vector2 position)
    {
        return Physics2D.BoxCast(position, size, 0, Vector3.zero);
    }

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
                if (!collideAt(transform.position + new Vector3(sign , 0,0))) 
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
                if (!collideAt(transform.position + new Vector3(0, sign  ,0))) 
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
