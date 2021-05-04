using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 Speed;
    private Vector3 lastPosition;
    private float grav = 0;
    private const float Gravity = 1000;
    public bool enemyBullet;
    [SerializeField] private float damage;
    float speed = 350;



    void Start()
    {
        
    }


    void Update()
    {
        

        lastPosition = transform.position;

        Speed.y -= grav * Time.deltaTime;

        transform.Translate(new Vector3(Speed.x,Speed.y,0) * Time.deltaTime);

        if(!GameData.Instance.IsOnScreen(transform.position, Vector2.one)) { Destroy(gameObject); }

        BoxCollision();

        
            

    }

    public void ChangeSpeed(float s)
    {
        speed = s;
    }

    public void ChangeMoveDirection(Vector3 newDirection)
    {
        Speed.x = newDirection.x * speed;
        Speed.y = newDirection.y * speed;
    }

    public void MakeLob(float gravity = Gravity)
    {
        grav = gravity;
    }

    public void BoxCollision()
    {
        RaycastHit2D hitInfo = Physics2D.BoxCast(lastPosition, GetComponent<BoxCollider2D>().size, 0, Speed, 1);
        if (hitInfo)
        {

            if (hitInfo.collider.gameObject.layer == 0 && !hitInfo.collider.GetComponent<DestructableTiles>())
            {
                Destroy(gameObject);
            }

            if (enemyBullet)
            {
                WadeMachine wade = hitInfo.collider.gameObject.GetComponent<WadeMachine>();
                if (wade)
                {
                    wade.TakeDamage(Mathf.Sign(Speed.x), gameObject);
                }
            }

            if (!enemyBullet)
            {
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    if (enemy.IsOnScreen())
                    {
                        enemy.TakeDamage();
                        Destroy(gameObject);
                    }
                }
            }


            if (hitInfo.collider.GetComponent<ConveyerSwitch>())
            {
                hitInfo.collider.GetComponent<ConveyerSwitch>().SwitchDirection();
                Destroy(gameObject);
            }
        }
    }

    public void LineCollision()
    {
        RaycastHit2D hitInfo = Physics2D.Linecast(lastPosition, transform.position);

        if (hitInfo)
        {

            if (hitInfo.collider.gameObject.layer == 0 && !hitInfo.collider.GetComponent<DestructableTiles>())
            {
                Destroy(gameObject);
            }

            if (enemyBullet)
            {
                WadeMachine wade = hitInfo.collider.gameObject.GetComponent<WadeMachine>();
                if (wade)
                {
                    wade.TakeDamage(Mathf.Sign(Speed.x), gameObject);
                }
            }

            if (!enemyBullet)
            {
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    if (enemy.IsOnScreen())
                    {
                        enemy.TakeDamage();
                        Destroy(gameObject);
                    }
                }
            }


            if (hitInfo.collider.GetComponent<ConveyerSwitch>())
            {
                hitInfo.collider.GetComponent<ConveyerSwitch>().SwitchDirection();
                Destroy(gameObject);
            }

        }
    }
    

}
