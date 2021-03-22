using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameData gameData;
    [SerializeField] private int health;
    private int damage = 1;
    private BoxCollider2D boxCollider;
    private int directionInt = 1;

    


    void Start()
    {
        gameData = GameData.Instance;
        boxCollider = GetComponent<BoxCollider2D>();

    }

    
    public void TakeDamage()
    {
        health -= 1;
        if(health <= 0) { Debug.Log(gameObject.name + "IsNowDead"); }
    }

    void OnGUI()
    {
        //Output the angle found above
        GUI.Label(new Rect(25, 25, 200, 40), "Angle Between Objects" + gameData.RegulatedWadeAngle(transform.position, 1));
    }
}
