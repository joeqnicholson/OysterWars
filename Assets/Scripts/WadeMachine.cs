using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;



public class WadeMachine : MonoBehaviour
{
    private CharacterBody2D body;
    private CharacterMotor motor;
    private float walkSpeed = 105f;
    private float halfGravThreshold = -25;
    private float walkAcceleration = 1600f;
    private float walkDeceleration = 1000f;
    private float jumpSpeed = 210f;
    private float maxFall = -320f;
    private float gravity = 1500f;
    private float moveX;
    private float moveY;
    private Vector2 Speed;
    private float varJumpTime = .26f;
    private float varJumpTimer = 0;
    private bool canFlip;
    private float spriteLerp;
    private ObjectSprite sprite;
    private float directionInt;
    private float shotTimer;
    public CameraBox currentCameraBox;
    private bool crouching;
    private Vector2 shootDirection;
    private Vector2 shootPoint;
    [SerializeField] GameObject currentBullet;
    private float airTimer = 0;
    private float jumpGraceTimer = 0;
    private float jumpGraceTime = 0.1f;
    [SerializeField] private int health;
    

    

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<ObjectSprite>();
        body = GetComponent<CharacterBody2D>();
        motor = GetComponent<CharacterMotor>();
        motor.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        shotTimer += Time.deltaTime;

       
        
        jumpGraceTimer += Time.deltaTime;
        

        UpdateSprite();

        canFlip = true;

        if (varJumpTimer > 0) { varJumpTimer -= Time.deltaTime; }

        CharacterFlip();
        
        ShootStuff();

        if (motor.IsGrounded)
        {

            if (!crouching)
            {
                if (moveY == -1 && moveX == 0)
                {
                    sprite.scale = new Vector3(1.3f, .7f, 1);
                }
            }

            if (moveY == -1 && moveX == 0) { crouching = true; }

            

            if (crouching && (moveX != 0 || moveY != -1))
            {
                sprite.scale = new Vector3(.7f, 1.3f, 1);
                crouching = false;
            }
        }

        if (motor.IsGrounded && Speed.y < 0)
        {
            airTimer = 0;
            float squish = Mathf.Min(Speed.y / maxFall, 1);
            sprite.scale.x = Approach(1, 1.4f, squish);
            sprite.scale.y = Approach(1, .6f, squish);
            Speed.y = 0;
        }

        if (Input.GetKeyDown(KeyCode.K)) { jumpGraceTimer = 0f; }

        if (motor.IsGrounded && jumpGraceTimer < jumpGraceTime || !motor.IsGrounded && airTimer < jumpGraceTime && Input.GetKeyDown(KeyCode.K))
        {
            sprite.scale = new Vector3(0.6f, 1.4f, 1);
            varJumpTimer = varJumpTime;
            motor.ForceNotGroundedState();
            Speed.y = jumpSpeed;
        }

        if (!motor.IsGrounded)
        {
            airTimer += Time.deltaTime;

            float mult = (Mathf.Abs(Speed.y) < halfGravThreshold && Input.GetKey(KeyCode.K)) ? .75f : 1f;

            Speed.y = Approach(Speed.y, maxFall, gravity * mult * Time.deltaTime);
        }

        if (varJumpTimer > 0)
        {
            if (Input.GetKey(KeyCode.K))
            {
                Speed.y = jumpSpeed;
            }

            else
            {
                varJumpTimer = 0;
            }
                
        }


        if(moveX == 0)
        {
            Speed.x = Approach(Speed.x, moveX * walkSpeed, walkAcceleration * AccelMultipler() * Time.deltaTime);
        }
        else
        {
            Speed.x = Approach(Speed.x, moveX * walkSpeed, walkDeceleration * AccelMultipler() * Time.deltaTime);
        }
        
        
        motor.SetVelocity(Speed);
    }

    float Approach(float val, float target, float maxMove)
    {
        return val > target ? Mathf.Max(val - maxMove, target) : Mathf.Min(val + maxMove, target);
    }

    float AccelMultipler()
    {
        float airMult = motor.IsGrounded ? 1 : .65f;

        if (moveX != 0)
        {
            return 1f * airMult;
        }
        else
        {
            return .4f * airMult;
        }
    }

    public void CharacterFlip()
    {
        if (canFlip)
        {
            if(moveX != 0)
            {
                sprite.direction = Mathf.Sign(moveX);
                
            }
        }
        directionInt = Mathf.Sign(sprite.direction);

    }

    void ShootStuff()
    {
        float x = 16;
        float y = 10;
        

        if(moveX != 0)
        {
            if (moveY > 0)
            {
                x = 16;
                y = 26;
                shootDirection = Vector2.right * directionInt + Vector2.up ;
            }
            else if(moveY < 0)
            {
                x = 16;
                y = 2 * directionInt;
                shootDirection = Vector2.right * directionInt + Vector2.down ;
            }
            else
            {
                x = 20;
                y = 10;
                shootDirection = Vector2.right * directionInt;
            }
        }
        else if(moveX == 0 && moveY != 0)
        {
            if(moveY == -1)
            {
                if (motor.IsGrounded)
                {
                    x = 19;
                    y = 8;
                    shootDirection = Vector2.right * directionInt;

                }
                else
                {
                    x = 0;
                    y = -5;
                    shootDirection = Vector2.down * 1.3f;
                }
            }
            else if (moveY == 1)
            {
                x = 0;
                y = 27;
                shootDirection = Vector2.up;
            }
        }
        else
        {
            shootDirection = Vector2.right * directionInt;
        }


        shootDirection = Vector2.ClampMagnitude(shootDirection, 1f);

        x *= directionInt;
  
        shootPoint = new Vector2(x,y);

        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("youshot");
            Bullet newBullet = Instantiate(currentBullet, transform.position + new Vector3(shootPoint.x, shootPoint.y, 0), Quaternion.identity).GetComponent<Bullet>();
            newBullet.GetComponent<Bullet>().moveDirection = shootDirection;
            shotTimer = 0;
        }

        


        // ddown = 12,2 dup = 12, 22 airdown = 0,0 up = 0, 27 forward = 17, 13 crouch 8, 13
    }

    public void TakeDamage()
    {
        health -= 1;
        if(health <= 0) { Debug.Log("wadeIsNowDead"); }
    }

    void UpdateSprite()
    {
        sprite.scale.x = Approach(sprite.scale.x, 1f, 2.75f * Time.deltaTime);
        sprite.scale.y = Approach(sprite.scale.y, 1f, 2.75f * Time.deltaTime);

        if (!motor.IsGrounded)
        {
            if (moveY > 0)
            {
                if(Mathf.Abs(moveX) > 0)
                {
                    sprite.Play(sprite.JumpAimDUp);
                }
                else
                {
                    sprite.Play(sprite.JumpAimUp);
                }

            }
            else if(moveY < 0)
            {
                if (Mathf.Abs(moveX) > 0)
                {
                    sprite.Play(sprite.JumpAimDDown);
                }
                else
                {
                    sprite.Play(sprite.JumpAimDown);
                }
            }
            else
            {
                if (shotTimer > 6)
                {
                    sprite.Play(sprite.JumpRegular, true);
                }
                else
                {
                    sprite.Play(sprite.JumpAimForward, true);
                }
            }

                
        }

        if (motor.IsGrounded)
        {
            if(Mathf.Abs(moveX) > 0)
            {

                if (moveY > 0)
                {
                    sprite.Play(sprite.RunAimUp);
                }
                else if(moveY < 0)
                {
                    sprite.Play(sprite.RunAimDown);
                }
                else
                {
                    if(shotTimer > 6)
                    {
                        sprite.Play(sprite.RunRegular, true);
                    }
                    else
                    {
                        sprite.Play(sprite.RunAimForward, true);
                    }

       
                }

                
            }
            else
            {
                if(moveY > 0)
                {
                    sprite.Play(sprite.AimUp, true);
                }
                else if (moveY < 0)
                {
                    sprite.Play(sprite.Crouch, true);
                }
                else
                {
                    if(shotTimer > 6)
                    {
                        sprite.Play(sprite.Idle, true);
                    }
                    else
                    {
                        sprite.Play(sprite.IdleShoot, true);
                    }
                }
                
            }
            
        }
        
    }

    

}
