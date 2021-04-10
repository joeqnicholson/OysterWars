using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 moveDirection;
    private Vector3 lastPosition;
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
        transform.Translate(moveDirection * speed * Time.deltaTime);

        if(!GameData.Instance.IsOnScreen(transform.position, Vector2.one)) { Destroy(gameObject); }

        RaycastHit2D hitInfo = Physics2D.Linecast(lastPosition, transform.position);

        if(hitInfo)
        {
            if (enemyBullet)
            {
                WadeMachine wade = hitInfo.collider.gameObject.GetComponent<WadeMachine>();
                if (wade)
                {
                    wade.TakeDamage(Mathf.Sign(moveDirection.x), gameObject);
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

            


            if (hitInfo.collider.gameObject.layer == 0 && !hitInfo.collider.GetComponent<DestructableTiles>())
            {
                Destroy(gameObject);
            }
            
        }
            


    }

    

}
