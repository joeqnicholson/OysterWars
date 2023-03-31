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



    public void Start()
    {
        base.Start();
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
        newDirection *= speed;
        newDirection = Vector2.ClampMagnitude(newDirection,speed);
        Speed.x = newDirection.x;
        Speed.y = newDirection.y;
    }

    public override void OnTriggerHit(Trigger trigger)
    {
        if(trigger.gameObject.layer == 11) 
        {
            FindObjectOfType<WadeMachine>().LaunchStart(trigger, Speed.normalized);
            Destroy(gameObject);
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
                    
            Destroy(gameObject);

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
        
         Destroy(gameObject);

    }

    
    

}
