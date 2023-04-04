using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookshotKnife : Actor
{
    [System.NonSerialized]public Vector2 Speed;
    private Vector3 lastPosition;
    private float grav = 0;
    private const float Gravity = 500;
    public GameObject explosionPrefab;
    public GameObject sparkPrefab;
    public bool enemyBullet;
    [SerializeField] private float damage;
    private float speed = 701;

    public Sprite[] knifeSprites = new Sprite[8];

    public SpriteRenderer spriteRenderer;



    public void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Update()
    {
        

        Speed = Vector2.ClampMagnitude(Speed,speed);
        Move(Speed);

        if(!GameData.Instance.IsOnScreen(transform.position, Vector2.one)) { Destroy(gameObject); }


    }

    public void ChangeSpeed(float s)
    {
        speed = s;
        
    }

    public void ChangeMoveDirection(Vector3 newDirection)
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetSprite(newDirection.normalized);

        newDirection *= speed;

        newDirection = Vector2.ClampMagnitude(newDirection,speed);
        Speed.x = newDirection.x;
        Speed.y = newDirection.y;
    }

    public void SetSprite(Vector3 dir)
    {
        print(dir);
        if(dir.y == 0)
        {
            if(dir.x > 0)
            {
                spriteRenderer.sprite = knifeSprites[3];
            }
            else
            {
                spriteRenderer.sprite = knifeSprites[2];
            }
        }
        else if(dir.x == 0)
        {
            if(dir.y > 0)
            {
                spriteRenderer.sprite = knifeSprites[0];
            }
            else
            {
                spriteRenderer.sprite = knifeSprites[1];
            }
        }
        else
        {
            if(dir.x > 0)
            {
                if(dir.y > 0)
                {
                    spriteRenderer.sprite = knifeSprites[4];
                    print("jimbo");
                }
                else
                {
                    spriteRenderer.sprite = knifeSprites[5];
                }
            }
            else
            {
                if(dir.y > 0)
                {
                    spriteRenderer.sprite = knifeSprites[6];
                }
                else
                {
                    spriteRenderer.sprite = knifeSprites[7];
                }
            }
        }
    }

    public override void OnTriggerHit(Trigger trigger)
    {

        if(trigger.gameObject.layer == 11) 
        {
            FindObjectOfType<WadeMachine>().LaunchStart(trigger, Speed.normalized);
            Vector3 conversion = Speed;
            DestroyAndLeaveSprite(trigger.transform.position - (conversion.normalized * 20));
        }

        if(trigger.gameObject.layer == 12)
        {
            Vector3 speedConversion = Speed.normalized * 6;
            Hit hitInfo = new Hit();
            hitInfo.point = transform.position + speedConversion.normalized * 4;
            hitInfo.normal = -speedConversion.normalized;
            hitInfo.aabb = trigger;
            Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);
            FindObjectOfType<WadeMachine>().HookshotStart(hitInfo, true);
            Vector3 conversion = Speed;
            DestroyAndLeaveSprite(trigger.transform.position - (conversion.normalized * 20));
        }

        if(trigger.gameObject.layer == 13)
        {
            Hit hitInfo = new Hit();
            hitInfo.point = trigger.Center();
            hitInfo.normal = -Speed.normalized;
            hitInfo.aabb = trigger; 
            FindObjectOfType<WadeMachine>().HookshotStart(hitInfo);
            Vector3 conversion = Speed;
            DestroyAndLeaveSprite(trigger.transform.position - (conversion.normalized * 20));
        }

       

    }


    public override void OnCollisionHit(Solid solid)
    {
        if(solid.metal)
        {
            Instantiate(sparkPrefab, transform.position, Quaternion.identity);

        }   
        else
        {
            print("good");
            Vector3 speedConversion = Speed.normalized * 6;
            Hit hitInfo = new Hit();
            hitInfo.point = transform.position + speedConversion.normalized * 4;
            hitInfo.normal = -speedConversion.normalized;
            hitInfo.aabb = solid;

            SecretFade secretFade = hitInfo.aabb.gameObject.GetComponent<SecretFade>();

            if (secretFade)
            {
                secretFade.StartFade();
            }

            Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);


            FindObjectOfType<WadeMachine>().HookshotStart(hitInfo);
                    
           
        }
        
         DestroyAndLeaveSprite(transform.position);

    }

    void DestroyAndLeaveSprite(Vector3 position)
    {
        Transform child = transform.GetChild(0);
        child.parent = null;
        child.position = position;
        Destroy(gameObject);
    }



    
    

}
