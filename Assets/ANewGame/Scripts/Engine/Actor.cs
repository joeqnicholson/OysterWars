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
    public bool inTrigger;
    public Trigger currentTrigger;

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
                Solid solidToHit = CollideAtSolid(transform.position + new Vector3(sign , 0  ,0));
                CheckTrigger(transform.position + new Vector3(sign , 0  ,0));
                if (!solidToHit) 
                { 
                    //There is no Solid immediately beside us 
                    transform.Translate(Vector3.right * sign); 
                    move -= sign; 
                } 
                else 
                { 
                    OnCollisionHit(solidToHit);
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
                Solid solidToHit = CollideAtSolid(transform.position + new Vector3(0, sign  ,0));
                CheckTrigger(transform.position + new Vector3(0, sign  ,0));
                if(!solidToHit) 
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

                    OnCollisionHit(solidToHit);

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
                OnNoGroundHit();
            }
        }
    }

    public void CheckTrigger(Vector3 pos)
    {

        Trigger insideTrigger = CollideAtTrigger(pos);   

        if(!inTrigger)
        {
           if(insideTrigger)
           {
                OnTriggerHit(insideTrigger);
                currentTrigger = insideTrigger;
                inTrigger = true;
           }

        }
        else
        {

            if(!insideTrigger)
            {
                OnNoTriggerHit(currentTrigger);
                inTrigger = false;
                currentTrigger = null;
            }
            else
            {
                WhileInTrigger(insideTrigger);
            }
        }

    }



    public virtual void OnNoGroundHit()
    {
        // print("GroundHit");
    }


    public virtual void OnGroundHit()
    {
        // print("GroundHit");
    }

    public virtual void OnTriggerHit(Trigger trigger)
    {
        // print("TriggerHit");
    }

    public virtual void OnNoTriggerHit(Trigger trigger)
    {
        // print("TriggerLeave");
    }

    public virtual void WhileInTrigger(Trigger trigger)
    {
        // print("InTrigger");
    }

    public virtual void OnCollisionHit(Solid solid)
    {
        // print("Collision");
    }

}
