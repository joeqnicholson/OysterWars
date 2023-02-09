using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [System.NonSerialized]public Vector2 Speed;
    private Vector3 lastPosition;
    private float grav = 0;
    private const float Gravity = 500;
    public GameObject explosionPrefab;
    public bool enemyBullet;
    [SerializeField] private float damage;
    private float speed = 250;
    private bool hookshot;


    public void Update()
    {
        
        lastPosition = transform.position;
        Speed.y -= grav * Time.deltaTime;
        Speed = Vector2.ClampMagnitude(Speed,speed);
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
        newDirection *= speed;
        newDirection = Vector2.ClampMagnitude(newDirection,speed);
        Speed.x = newDirection.x;
        Speed.y = newDirection.y;
    }

    public void MakeLob(float gravity = Gravity)
    {
        grav = gravity;
    }

    public void MakeHookShot()
    {
        hookshot = true;
    }

    public void BoxCollision()
    {
        // RaycastHit2D hitInfo = Physics2D.BoxCast(lastPosition, GetComponent<BoxCollider2D>().size, 0, Speed.normalized, Speed.magnitude);
        RaycastHit2D hitInfo = Physics2D.Linecast(lastPosition, transform.position);
        if (hitInfo)
        {
            if (enemyBullet)
            {
                WadeMachine wade = hitInfo.collider.gameObject.GetComponent<WadeMachine>();
                if (wade)
                {
                    Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);
                    wade.TakeDamage(Mathf.Sign(Speed.x), gameObject);
                }
            }

            if (!enemyBullet)
            {
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                SecretFade secretFade = hitInfo.collider.GetComponent<SecretFade>();
                if (enemy)
                {
                        if (enemy.IsOnScreen())
                        {
                            if (enemy.canGetHit)
                            {
                                Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);
                                enemy.TakeDamage();
                                Destroy(gameObject);
                            }
                            
                        }
                }

                if (secretFade)
                {
                    secretFade.StartFade();
                }
            }


            if (hitInfo.collider.GetComponent<ConveyerSwitch>())
            {
                hitInfo.collider.GetComponent<ConveyerSwitch>().SwitchDirection();
                Destroy(gameObject);
            }


            if (hitInfo.collider.gameObject.layer == 0)
            {
                Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);
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
                Instantiate(explosionPrefab, hitInfo.point, Quaternion.identity);
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
                // Destroy(gameObject);
            }

        }
    }
    

}
