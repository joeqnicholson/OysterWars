using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;


public class Enemy : CharacterMotor
{
    private GameData gameData;
    [SerializeField] public int startHealth;
    public int health;
    private int damage = 1;
    public BoxCollider2D boxCollider;
    public bool canGetHit = true;
    [SerializeField] private bool respawn;
    public GameObject respawnPrefab;
    CharacterBody2D body;
    public CameraBox cameraBox;
    public bool touchForDamage = true;
    public GameObject explosion;
    public SpriteAnimationController sprite;

    protected override void Start()
    {
        base.Start();
        health = startHealth;
        gameData = GameData.Instance;
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteAnimationController>();
        body = GetComponent<CharacterBody2D>();
        if (!respawn) { respawnPrefab = null; }
        sprite.transform.parent = null;

    }

    public void LateUpdate()
    {
        if(sprite)
        {
            sprite.transform.position = Vector3Int.RoundToInt(transform.position);
        }
    }

    
    public void TakeDamage()
    {
        if (canGetHit)
        {
            health -= 1;
            if (health <= 0)
            {
                GameObject explosionFX =Instantiate(
                    explosion,
                    transform.position + Vector3.up * (boxCollider.size.y / 2),
                    Quaternion.identity
                    );
                

                OnDeath();
                Destroy(sprite.gameObject);
                Destroy(gameObject);
            }
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

    public RaycastHit2D RightBottomHit(float distance = 1)
    {
        int obstacles = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

        RaycastHit2D hitInfo = Physics2D.Linecast(body.GetBottomRight(transform.position), body.GetBottomLeft(transform.position) + Vector3.down * distance, obstacles);
        return hitInfo;
    }

    public RaycastHit2D LeftBottomHit(float distance = 1)
    {
        int obstacles = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

        RaycastHit2D hitInfo = Physics2D.Linecast(body.GetBottomLeft(transform.position), body.GetBottomLeft(transform.position) + Vector3.down * distance, obstacles);
        return hitInfo;
    }

    public virtual void OnDeath()
    {

    }

}
