using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;



public enum BozuState
{
    Runner, Shooter, Thrower, NotOnScreen
}

public class BozuMachine : Enemy
{

    CharacterMotor motor;
    [SerializeField] SpriteAnimation BozuRun;
    [SerializeField] SpriteAnimation BozuJump;
    [SerializeField] SpriteAnimation BozuAimIdle;
    [SerializeField] SpriteAnimation BozuAimDUp;
    [SerializeField] SpriteAnimation BozuAimUp;
    [SerializeField] SpriteAnimation BozuAimDDown;
    [SerializeField] SpriteAnimation BozuAimDown;
    private BozuState LastBozuState;
    SpriteAnimationController sprite;
    private float runSpeed = 97;
    [SerializeField]private Vector2 Speed;
    private float jumpSpeed = 210f;
    private float maxFall = -320f;
    private float gravity = 1500f;
    [SerializeField] private BozuState CurrentBozuState;
    private float varJumpTime = .2f;
    [SerializeField] private float varJumpTimer = 0;
    private float directionInt;
    [SerializeField] LayerMask jumpLayerMask;
    bool aboutToTurn = false;
    [SerializeField]private float wadeSignedAngle;
    [SerializeField]private float shootDirectionAngle;
    float colliderHeight;
    private Vector3 bulletDirection;
    private Vector3 shootPoint;
    [SerializeField] Transform shootTransform;
    [SerializeField] private float shotTimer;
    [SerializeField] float shotTime = 2;
    [SerializeField] GameObject currentBullet;
    [SerializeField] private bool sixteenDirections;
    private bool isOnScreen = false;



    protected override void Start()
    {
        base.Start();
        LastBozuState = CurrentBozuState;
        CurrentBozuState = BozuState.NotOnScreen;
        motor = GetComponent<CharacterMotor>();
        sprite = GetComponentInChildren<SpriteAnimationController>();
        if (GameData.Instance.OnRightSide(transform.position.x)) { sprite.direction = -1; }
        OnGroundCollision += BozuGroundedFlag;
        OnHeadCollision += BozuHeadFlag;
        directionInt = Mathf.Sign(sprite.transform.localScale.x);
        colliderHeight = GetComponent<BoxCollider2D>().size.y;
    }



    
    void Update()
    {
        VisibilityBehaviour();
        directionInt = sprite.direction;
        switch (CurrentBozuState)
        {
            case BozuState.Runner:
                {
                    CheckJump();
                    if (!IsGrounded)
                    {
                        InAir();
                    }
                    else
                    {
                        Speed.y = 0;
                    }

                    Speed.x = MathHelper.Approach(Speed.x, runSpeed * directionInt, 1000 * Time.deltaTime);
                    UpdateSprite();

                    break;
                }
            case BozuState.Shooter:
                {
                    if (directionInt == 1 && GameData.Instance.wadePosition.x < transform.position.x) { sprite.direction *= -1; }
                    if (directionInt == -1 && GameData.Instance.wadePosition.x > transform.position.x) { sprite.direction *= -1; }
                    wadeSignedAngle = GameData.Instance.RegulatedWadeSignedAngle(transform.position, -directionInt, MathHelper.SignedAngles45);
                    UpdateSprite();
                    shootTransform.position = shootPoint;
                    shootDirectionAngle = GameData.Instance.RegulatedWadeSignedAngle(transform.position, 1, sixteenDirections? MathHelper.SignedAngles22 : MathHelper.SignedAngles45);

                    shootTransform.rotation = Quaternion.Euler(0, 0, shootDirectionAngle);
                    bulletDirection = -shootTransform.right;
                    
                    shotTimer += Time.deltaTime;

                    if(shotTimer > shotTime)
                    {
                        print("bozuShoot");
                        Bullet newBullet = Instantiate(currentBullet, shootTransform.position, Quaternion.identity).GetComponent<Bullet>();
                        newBullet.GetComponent<Bullet>().ChangeMoveDirection(bulletDirection);
                        newBullet.GetComponent<Bullet>().enemyBullet = true;
                        shotTimer = 0;
                    }

                    if (!IsGrounded) { Speed.y = -200f; } else { Speed.y = 0; }

                    break;
                }
            case BozuState.Thrower:
                {
                    break;
                }
            case BozuState.NotOnScreen:
                {
                    sprite.PlayNothing();
                    Speed = Vector2.zero;
                    break;
                }
                
        }
        motor.SetVelocity(new Vector2(Speed.x, Speed.y));
        
        
        
    }

    void CheckJump()
    {
        
        Vector3 startLinePos = transform.position + (Vector3.up * colliderHeight / 2);
        Vector3 endLinePos = startLinePos + Vector3.right * directionInt * 45;
        Vector3 highStartLinePos = transform.position + (Vector3.up * colliderHeight / 2) + (Vector3.up * colliderHeight);
        Vector3 highEndLinePos = highStartLinePos + Vector3.right * directionInt * 47;
        Vector3 turnStartLinePos = transform.position + (Vector3.up * colliderHeight / 2) + (Vector3.up * colliderHeight * 2f);
        Vector3 turnEndLinePos = turnStartLinePos + Vector3.right * directionInt * 55;


        bool tallWall = Physics2D.Linecast(turnStartLinePos, turnEndLinePos, jumpLayerMask);
        bool midWall = Physics2D.Linecast(highStartLinePos, highEndLinePos, jumpLayerMask);
        bool lowWall = Physics2D.Linecast(startLinePos, endLinePos, jumpLayerMask);
        bool lowCeiling = Physics2D.Linecast( transform.position, transform.position + (Vector3.up * colliderHeight * 2.5f), jumpLayerMask);
        bool highCeiling = Physics2D.Linecast( transform.position, transform.position + (Vector3.up * colliderHeight * 1.5f), jumpLayerMask);
        bool groundInFront = Physics2D.Linecast(transform.position + Vector3.right * directionInt * 45, transform.position + Vector3.right * directionInt * 55 + Vector3.down * 25, jumpLayerMask);
        bool deepGroundInFront = Physics2D.Linecast(transform.position + Vector3.right * directionInt * 45, transform.position + Vector3.right * directionInt * 55 + Vector3.down * 50, jumpLayerMask);

        if (Speed.y == 0 )
        {
            if (tallWall && midWall && lowWall || midWall && lowWall && highCeiling)
            {
                if (!aboutToTurn)
                {
                    StartCoroutine(Turn());
                }
            }
            else if (!tallWall && midWall && lowWall && !aboutToTurn || midWall && !groundInFront && !tallWall)
            {
                Jump(.3f);
            }
            else if (!midWall && lowWall && !aboutToTurn)
            {
                Jump(.2f);
            }
            else if(!IsGrounded && !Physics2D.Linecast(transform.position, transform.position + Vector3.down * 50, jumpLayerMask) && groundInFront)
            {
                Jump(.2f);
            }
            else if (!IsGrounded
                && !Physics2D.Linecast(transform.position, transform.position + Vector3.down * 50, jumpLayerMask)
                && !deepGroundInFront
                && !aboutToTurn
                )
            {
                Jump(.2f);
            }
        }  
    }

    public IEnumerator Turn()
    {
        float duration = .26f;

        while(duration > 0 && !IsAgainstLeftWall && !IsAgainstRightWall)
        {
            aboutToTurn = true;
            duration -= Time.deltaTime;
            yield return null;

        }

        aboutToTurn = false;
        sprite.direction *= -1;

    }

    void UpdateSprite()
    {

        sprite.scale.x = MathHelper.Approach(sprite.scale.x, 1f, 2.75f * Time.deltaTime);
        sprite.scale.y = MathHelper.Approach(sprite.scale.y, 1f, 2.75f * Time.deltaTime);


        switch (CurrentBozuState)
        {
            case BozuState.Runner:
                {
                    if (!motor.IsGrounded)
                    {
                        sprite.Play(BozuJump);
                    }
                    else
                    {
                        sprite.Play(BozuRun);
                    }
                    break;
                }
            case BozuState.Shooter:
                {
                    float x = 0;
                    float y = 0;
                    switch (wadeSignedAngle * directionInt)
                    {
                        case 0:
                            {
                                x = 17;
                                y = 13;
                                sprite.Play(BozuAimIdle);
                                break;
                            }
                        case 45:
                            {
                                y = 22;
                                x = 13;
                                sprite.Play(BozuAimDUp);
                                break;
                            }
                        case 90:
                            {
                                sprite.Play(BozuAimUp);
                                x = 0;
                                y = 34;
                                break;
                            }
                        case -45:
                            {
                                x = 11;
                                y = 2;
                                sprite.Play(BozuAimDDown);
                                break;
                            }
                        case -90:
                            {
                                x = 0;
                                y = -6;
                                sprite.Play(BozuAimDown);
                                break;
                            }

                    }
                    x *= directionInt;
                    shootPoint =  transform.position + new Vector3(x, y, 0);

                    
                    break;
                }
        }
        

    }

    void InAir()
    {

        varJumpTimer += Time.deltaTime;

        if(varJumpTime < varJumpTimer)
        {
            Speed.y = MathHelper.Approach(Speed.y, maxFall, gravity * Time.deltaTime);
        }
        
    }

    private void BozuGroundedFlag()
    {

        sprite.scale = new Vector2(1.4f, .6f);
        Speed.y = 0;
    }

    private void BozuHeadFlag()
    {
        if (!IsGrounded)
        {
            print('F');
            varJumpTimer = 1000;
            Speed.y /= 2 ;
        }
    }

    private void Jump(float currentVarJumpTime)
    {

        ForceNotGroundedState();
        sprite.scale = new Vector3(0.5f, 1.5f, 1);
        varJumpTime = currentVarJumpTime;
        varJumpTimer = 0;
        Speed.y = jumpSpeed;
    }

    private void VisibilityBehaviour()
    {
        if (isOnScreen)
        {
            if (!IsOnScreen())
            {

                if(CurrentBozuState == BozuState.Runner) { Destroy(gameObject); }
                LastBozuState = CurrentBozuState;
                CurrentBozuState = BozuState.NotOnScreen;
                isOnScreen = false;
            }
        }
        else
        {
            if (IsOnScreen())
            {

                CurrentBozuState = LastBozuState;
                isOnScreen = true;
            }
        }
    }


}
