using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;


public class Enemy : CharacterMotor
{
    private GameData gameData;
    [SerializeField] private int startHealth;
    private int health;
    private int damage = 1;
    public BoxCollider2D boxCollider;
    public bool canGetHit = true;
    
   

    


    protected override void Start()
    {
        base.Start();
        health = startHealth;
        gameData = GameData.Instance;
        boxCollider = GetComponent<BoxCollider2D>();

    }

    
    public void TakeDamage()
    {
        if (canGetHit)
        {
            health -= 1;
            if (health <= 0) { Destroy(gameObject); }
        }
    }

    public bool IsOnScreen()
    {
        return GameData.Instance.IsOnScreen(transform.position, boxCollider.size);
    }

    public float WadeSignedAngle()
    {
        return GameData.Instance.RegulatedWadeSignedAngle(transform.position, 1, MathHelper.SignedAngles45);
    }

    public float WadeDistance()
    {
        return Vector2.Distance(GameData.Instance.wadeXYPosition, transform.position);
    }

    public Vector2 WadeDistanceVector()
    {
        float xPos = GameData.Instance.wadeXYPosition.x - transform.position.x;
        float yPos = GameData.Instance.wadeXYPosition.y - transform.position.y;


        return new Vector2(xPos, yPos);
    }

    

}
