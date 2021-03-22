using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 moveDirection;
    private Vector3 lastPosition;
    public bool enemyBullet;
    [SerializeField] private float damage;
    float speed = 450;
    void Start()
    {
        
    }


    void Update()
    {
        RaycastHit2D hit;

        lastPosition = transform.position;
        transform.Translate(moveDirection * speed * Time.deltaTime);


        RaycastHit2D hitInfo = Physics2D.Linecast(lastPosition, transform.position);

        if(hitInfo)
        {
            if (enemyBullet)
            {
                WadeMachine wade = hitInfo.collider.gameObject.GetComponent<WadeMachine>();
                if (wade)
                {
                    wade.TakeDamage();
                    Destroy(gameObject);
                }
            }

            if (!enemyBullet)
            {
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.TakeDamage();
                    Destroy(gameObject);
                }
            }


            if (hitInfo.collider.gameObject.layer != 6)
            {
                Destroy(gameObject);
            }
            
        }
            


    }

    

}
