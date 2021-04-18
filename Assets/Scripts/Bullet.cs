using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float SpeedX;
    private float SpeedY;
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
        RaycastHit2D hit;

        lastPosition = transform.position;

        SpeedY -= grav * Time.deltaTime;

        transform.Translate(new Vector3(SpeedX,SpeedY,0) * Time.deltaTime);

        if(!GameData.Instance.IsOnScreen(transform.position, Vector2.one)) { Destroy(gameObject); }

        RaycastHit2D hitInfo = Physics2D.Linecast(lastPosition, transform.position);

        if(hitInfo)
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
                    wade.TakeDamage(Mathf.Sign(SpeedX), gameObject);
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

    public void ChangeMoveDirection(Vector3 newDirection)
    {
        SpeedX = newDirection.x * speed;
        SpeedY = newDirection.y * speed;
    }

    public void MakeLob()
    {
        grav = Gravity;
    }
    

}
