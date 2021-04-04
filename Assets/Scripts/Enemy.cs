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
    
   

    


    protected override void Start()
    {
        base.Start();
        health = startHealth;
        gameData = GameData.Instance;
        boxCollider = GetComponent<BoxCollider2D>();

    }

    
    public void TakeDamage()
    {
        health -= 1;
        if(health <= 0) { Destroy(gameObject); }
    }

    public bool IsOnScreen()
    {
        return GameData.Instance.IsOnScreen(transform.position, boxCollider.size);
    }

}
