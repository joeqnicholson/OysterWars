using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;
using UnityEngine.SceneManagement;
using System;

public partial class WadeMachine : CharacterMotor
{
    public WadeInventory Inventory;
    public WadeSound Sound;
    private CharacterBody2D body;
    private CharacterMotor motor;
    private float walkSpeed = 110f;
    private float halfGravThreshold = -25;
    private float walkAcceleration = 2600f;
    private float walkDeceleration = 2000f;
    private float jumpSpeed = 178f;
    private bool hasShortHopped;
    private float wallJumpHSpeed = 60;
    private float conveyerJumpHSpeed = 250;
    private float maxFall = -250f;
    private float gravity = 1500f;
    private float moveX;
    private float moveY;
    private float aimX;
    public Vector2 Speed;
    private float varJumpTime = .24f;
    private float varJumpTimer = 0;
    private float spriteLerp;
    private ObjectSprite sprite;
    public float directionInt;
    private float shotTimer;
    private float shotAnimationCooldown = 4;
    private bool crouching;
    private Vector2 shootDirection;
    private Vector2 shootPoint;
    [SerializeField] GameObject currentBullet;
    private float airTimer = 0;
    private float jumpGraceTimer = 0;
    private float jumpGraceTime = 0.1f;
    private int health;
    private int startHealth = 8;
    public bool canOpenChest;
    [SerializeField] private float conveyerAddition;
    private float hInputTimer = 0;
    private bool teleportHit;
    private float invincibiltyTimer = 0;
    private float invincibiltyTime = 1;
    float jumpDownDistance = 0.08f;
    private WadeInputs inputs;
    [SerializeField] bool canHopDown;
    private float forceMoveXTimer = 0;
    private float forceMoveXDirection = 0;
    private float WallJumpForceTime = .32f;
    private Vector3 forceToVector;
    private bool canInteract;
    [SerializeField] private GameObject particleSpawn;
    [SerializeField] private SpriteAnimation JumpParticle;
    [SerializeField] private SpriteAnimation WallJumpParticle;
    [SerializeField] private SpriteAnimation LandParticle;
    [SerializeField] private SpriteAnimation StartRunParticle;
    




    // Start is called before the first frame update
    protected override void Start()
    {
        
        Inventory = GetComponent<WadeInventory>();
        Sound = GetComponent<WadeSound>();
        CurrentWadeState = StNormal;
        OnGroundCollision += WadeGroundedFlag;
        inputs = GetComponent<WadeInputs>();
        health = startHealth;
        base.Start();
        sprite = GetComponentInChildren<ObjectSprite>();
        body = GetComponent<CharacterBody2D>();
        motor = GetComponent<CharacterMotor>();
        enabled = true;
    }

    // Update is called once per frame
    private void Update()
    {
        canHopDown = CanHopDown();

        aimX = inputs.moveInput.x;

        if (forceMoveXTimer > 0)
        {
            forceMoveXTimer -= Time.deltaTime;
            moveX = forceMoveXDirection;
        }
        else
        {
            moveX = inputs.moveInput.x;
        }



        moveY = inputs.moveInput.y;

        shotTimer += Time.deltaTime;



        jumpGraceTimer += Time.deltaTime;

        hInputTimer += Time.deltaTime;

        if (invincibiltyTimer <= invincibiltyTime)
        {
            invincibiltyTimer += Time.deltaTime;
        }




        if (varJumpTimer > 0) { varJumpTimer -= Time.deltaTime; }

        UpdateSprite();

        float lastDirection = directionInt;

        CharacterFlip();

        if(lastDirection != directionInt)
        {
            if (IsGrounded)
            {
                GameObject particle = Instantiate(particleSpawn, transform.position, Quaternion.identity);
                particle.GetComponent<SpriteAnimationController>().direction = directionInt;
                particle.GetComponent<SpriteAnimationController>().Play(StartRunParticle);
            }
        }



        if (inputs.jumpPress) { jumpGraceTimer = 0f; }


        if (Input.GetKeyDown(KeyCode.H))
        {
            hInputTimer = 0;

        }

        if (CurrentWadeState == StNormal)
        {
            NormalUpdate();
        }

        if (CurrentWadeState == StHit)
        {
            HitUpdate();
        }

        if (CurrentWadeState == StChest)
        {
            ChestUpdate();
        }

        SetVelocity(Speed);

    }

    void NormalUpdate()
    {


        ShootStuff();

        if (IsGrounded)
        {

            if (jumpGraceTimer > jumpGraceTime)
            {
                conveyerAddition = TakeConveyerSpeed();
            }

            if (!crouching)
            {
                if (moveY == -1 && moveX == 0)
                {
                    sprite.scale = new Vector3(1.3f, .7f, 1);
                }
            }

            if (moveY == -1 && moveX == 0) { crouching = true; }

            if(Mathf.Abs(moveX) > 0)
            {
                if (sprite.frameTriggerNow)
                {
                    if(sprite.imageIndex == 1 || sprite.imageIndex == 4)
                    {
                        Sound.PlayFootStep();
                    }
                }
            }

            if (crouching && (moveX != 0 || moveY != -1))
            {
                sprite.scale = new Vector3(.7f, 1.3f, 1);
                crouching = false;
            }
        }

        if ((IsGrounded && jumpGraceTimer < jumpGraceTime || !IsGrounded && airTimer < jumpGraceTime && inputs.jumpPress) && !canInteract)
        {
            if (!canHopDown)
            {
                Jump();
            }
            else
            {
                ForceNotGroundedState();
             
                Vector3 deltaPosition = -transform.up * (2 * CharacterConstants.SkinWidth + jumpDownDistance);
                varJumpTimer = 0;
                Teleport(body.RigidbodyComponent.Position + deltaPosition, transform.rotation);
            }
        }

        if (!IsGrounded)
        {

            if (IsHead) { varJumpTimer = 0; }

            conveyerAddition = MathHelper.Approach(conveyerAddition, 0, 55 * Time.deltaTime);

            if ((IsAgainstLeftWall || IsAgainstRightWall) && jumpGraceTimer < jumpGraceTime)
            {
                WallJump(IsAgainstRightWall);
            }

            airTimer += Time.deltaTime;

            

            Speed.y = MathHelper.Approach(Speed.y, maxFall, gravity * Time.deltaTime);

            if (varJumpTimer > 0)
            {
                Speed.y = jumpSpeed;
            }

            if(!hasShortHopped && !inputs.jumpHeld && varJumpTimer > .08f)
            {
                varJumpTimer = .08f;
                hasShortHopped = true;
            }
        }




        float lastSpeed = Speed.x;

        if (moveX == 0)
        {
            Speed.x = MathHelper.Approach(Speed.x, moveX * walkSpeed + conveyerAddition, walkAcceleration * AccelMultipler() * Time.deltaTime);
        }
        else
        {
            Speed.x = MathHelper.Approach(Speed.x, moveX * walkSpeed + conveyerAddition, walkDeceleration * AccelMultipler() * Time.deltaTime);
        }

        if(Mathf.Abs (lastSpeed) < 5f && Mathf.Abs(moveX) > 0.2f && IsGrounded)
        {
            GameObject particle = Instantiate(particleSpawn, transform.position, Quaternion.identity);
            particle.GetComponent<SpriteAnimationController>().direction = directionInt;
            particle.GetComponent<SpriteAnimationController>().Play(StartRunParticle);
        }




        if (crouching)
        {
            if (body.CharacterSize != new Vector3(13, 11, 0))
            {
                body.SetCharacterSize(new Vector3(13, 11, 0));
            }
        }
        else
        {
            if (body.CharacterSize != new Vector3(13, 22, 0))
            {
                body.SetCharacterSize(new Vector3(13, 22, 0));
            }
        }

    }

    private void Jump()
    {
        GameObject particle = Instantiate(particleSpawn, transform.position, Quaternion.identity);
        particle.GetComponent<SpriteAnimationController>().Play(JumpParticle);

        Sound.PlayJumpUp();
        Sound.PlayFootStep();
        hasShortHopped = false;
        sprite.scale = new Vector3(0.6f, 1.4f, 1);
        jumpGraceTimer = Mathf.Infinity;
        varJumpTimer = varJumpTime;
        ForceNotGroundedState();
        if (Mathf.Sign(moveX) == Mathf.Sign(conveyerAddition) && conveyerAddition != 0 && moveX != 0)
        {
            Speed.x = (walkSpeed + conveyerJumpHSpeed) * sprite.direction;
            varJumpTimer = varJumpTime * .5f;
        }
        Speed.y = jumpSpeed;

    }

    private void WallJump(bool rightCollision)
    {
        

        GameObject particle = Instantiate(
            particleSpawn,
            transform.position + transform.right * body.width / 2 * directionInt,
            Quaternion.identity
            );

        particle.GetComponent<SpriteAnimationController>().Play(WallJumpParticle);
        particle.GetComponent<SpriteAnimationController>().direction = -directionInt;

        float moveDirection = rightCollision ? -1 : 1;
        Sound.PlayJumpUp();
        Sound.PlayFootStep();
        jumpGraceTimer = Mathf.Infinity;
        sprite.scale = new Vector3(0.6f, 1.4f, 1);
        varJumpTimer = varJumpTime /1.5f;
        forceMoveXDirection = moveDirection;
        forceMoveXTimer = WallJumpForceTime;
        Speed.x = (walkSpeed + wallJumpHSpeed) * forceMoveXDirection;
        Speed.y = jumpSpeed;


      

    }

    void HitUpdate()
    {



        Time.timeScale = MathHelper.Approach(Time.timeScale, 1, Time.timeScale * 15 * Time.deltaTime);

        Speed.x = MathHelper.Approach(Speed.x, 0, 400 * Time.deltaTime);
        Speed.y = MathHelper.Approach(Speed.y, 0, 400 * Time.deltaTime);
        if (Time.timeScale >= .7) { TransitionToState(StNormal); }
        invincibiltyTimer = 0;

    }

    void ChestUpdate()
    {
        Teleport(forceToVector, transform.rotation);

        sprite.Play(sprite.SmallChest);
        if (sprite.imageIndex == sprite.SmallChest.totalFrames - 1)
        {
            WadeGroundedFlag();
            float squish = Mathf.Min(maxFall / maxFall, 1);
            sprite.scale.x = MathHelper.Approach(1, 1.4f, squish);
            sprite.scale.y = MathHelper.Approach(1, .6f, squish);
            TransitionToState(StNormal);
        }

    }


    float AccelMultipler()
    {
        return 1;

        float airMult = IsGrounded ? 1 : .5f;

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
        if (CurrentWadeState.canFlip)
        {
            if (moveX != 0)
            {
                sprite.direction = Mathf.Sign(moveX);

            }
        }
        directionInt = Mathf.Sign(sprite.direction);

        

    }

    void ShootStuff()
    {
        float x = 14;
        float y = 14;


        if (aimX != 0)
        {
            if (moveY > 0)
            {
                x = 16;
                y = 26;
                shootDirection = Vector2.right * directionInt + Vector2.up;
            }
            else if (moveY < 0)
            {
                x = 16;
                y = 2 * directionInt;
                shootDirection = Vector2.right * directionInt + Vector2.down;
            }
            else
            {
                x = 14;
                y = 14;
                shootDirection = Vector2.right * directionInt;
            }
        }
        else if (aimX == 0 && moveY != 0)
        {
            if (moveY == -1)
            {
                if (IsGrounded)
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

        shootPoint = new Vector2(x, y);

        if (inputs.shootPress)
        {
            Sound.PlayGunShot();
            Debug.Log("youshot");
            Bullet newBullet = Instantiate(currentBullet, transform.position + new Vector3(shootPoint.x, shootPoint.y, 0), Quaternion.identity).GetComponent<Bullet>();
            newBullet.GetComponent<Bullet>().ChangeMoveDirection(shootDirection);
            shotTimer = 0;
        }

        if (inputs.item1Press && (int)Inventory.itemCount[Inventory.LobShot] > 0)
        {
            Debug.Log("youshot");
            Bullet newBullet = Instantiate(currentBullet, transform.position + new Vector3(shootPoint.x, shootPoint.y, 0), Quaternion.identity).GetComponent<Bullet>();
            newBullet.GetComponent<Bullet>().ChangeMoveDirection(shootDirection);
            newBullet.GetComponent<Bullet>().MakeLob();
            shotTimer = 0;
        }




        // ddown = 12,2 dup = 12, 22 airdown = 0,0 up = 0, 27 forward = 17, 13 crouch 8, 13
    }

    public void TakeDamage(float recoilDirection, GameObject projectile = null)
    {
        if (invincibiltyTimer >= invincibiltyTime)
        {
            if (projectile) { Destroy(projectile); }
            directionInt = -recoilDirection;
            health -= 1;
            if (health <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                TransitionToState(StHit);
                Debug.Log("wadeIsHit");
            }
            
            
           
        }
    }

    void UpdateSprite()
    {

        if (CurrentWadeState == StNormal)
        {
            if (!IsGrounded)
            {
                if (moveY > 0)
                {
                    if (Mathf.Abs(aimX) > 0)
                    {
                        sprite.Play(sprite.JumpAimDUp);
                    }
                    else
                    {
                        sprite.Play(sprite.JumpAimUp);
                    }

                }
                else if (moveY < 0)
                {
                    if (Mathf.Abs(aimX) > 0)
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
                    if (shotTimer > shotAnimationCooldown)
                    {
                        sprite.Play(sprite.JumpRegular, true);
                    }
                    else
                    {
                        sprite.Play(sprite.JumpAimForward, true);
                    }
                }


            }

            if (IsGrounded)
            {
                if (Mathf.Abs(moveX) > 0)
                {

                    if (moveY > 0)
                    {
                        sprite.Play(sprite.RunAimUp);
                    }
                    else if (moveY < 0)
                    {
                        sprite.Play(sprite.RunAimDown);
                    }
                    else
                    {
                        if (shotTimer > shotAnimationCooldown)
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
                    if (moveY > 0)
                    {
                        sprite.Play(sprite.AimUp, true);
                    }
                    else if (moveY < 0)
                    {
                        sprite.Play(sprite.Crouch, true);

                    }
                    else
                    {
                        if (shotTimer > shotAnimationCooldown)
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

        if (CurrentWadeState == StHit)
        {
            sprite.Play(sprite.Hit, true);
        }

        sprite.scale.x = MathHelper.Approach(sprite.scale.x, 1f, 2.75f * Time.deltaTime);
        sprite.scale.y = MathHelper.Approach(sprite.scale.y, 1f, 2.75f * Time.deltaTime);
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        Chest chest = collision.gameObject.GetComponent<Chest>();
        LockedDoor lockedDoor = collision.gameObject.GetComponent<LockedDoor>();

        if (chest)
        {
            canInteract = true;
            if (IsGrounded && Mathf.Sign(chest.transform.localScale.x) == -Mathf.Sign(directionInt) && jumpGraceTimer < .2f)
            {
                chest.OpenChest();
                forceToVector = chest.wadeToPosition;
                TransitionToState(StChest);
            }



        }

        if (lockedDoor)
        {
            canInteract = true;
            if (IsGrounded && jumpGraceTimer < .2f)
            {
                lockedDoor.Unlock(Inventory);
                jumpGraceTimer = Mathf.Infinity;
                canInteract = false;

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponentInParent<Enemy>() && CurrentWadeState.canGetHit)
        {
            TakeDamage(-directionInt);
        }

        if (collision.gameObject.layer == 10 && CurrentWadeState.canGetHit)
        {
            TransitionToState(StHit);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Enemy>() && CurrentWadeState.canGetHit)
        {
            TakeDamage(-directionInt);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canInteract = false;
    }


    public void WadeGroundedFlag()
    {
        Sound.PlayJumpLand();
        airTimer = 0;
        forceMoveXTimer = 0;
        float squish = Mathf.Min(Speed.y / maxFall, 1);
        sprite.scale.x = MathHelper.Approach(1, 1.4f, squish);
        sprite.scale.y = MathHelper.Approach(1, .6f, squish);
        Speed.y = 0;

        float yPoint;
       

        if (!LeftBottomHit(20)) { yPoint = RightBottomHit(20).point.y; }
        else if (!RightBottomHit(20)) { yPoint = LeftBottomHit(20).point.y; }
        else if(RightBottomHit(20).distance > LeftBottomHit(20).distance) { yPoint = LeftBottomHit(20).point.y; }
        else { yPoint = RightBottomHit(20).point.y; }

        GameObject particle =Instantiate(particleSpawn, new Vector3(transform.position.x, yPoint,0), Quaternion.identity);
        particle.GetComponent<SpriteAnimationController>().Play(LandParticle);



    }

    void OnGUI()
    {
        //Output the angle found above
        //GUI.Label(new Rect(25, 25, 200, 40), "wades health is" + health);
    }

    bool CanHopDown()
    {
        int oneWay = layerMaskSettings.profile.oneWayPlatforms;
        int obstacles = layerMaskSettings.profile.obstacles;

        bool leftOneWay = Physics2D.Linecast(body.GetBottomLeft(transform.position), body.GetBottomLeft(transform.position) + Vector3.down, oneWay);
        bool leftNormal = Physics2D.Linecast(body.GetBottomLeft(transform.position), body.GetBottomLeft(transform.position) + Vector3.down, obstacles);
        bool rightOneWay = Physics2D.Linecast(body.GetBottomRight(transform.position), body.GetBottomRight(transform.position) + Vector3.down, oneWay);
        bool rightNormal = Physics2D.Linecast(body.GetBottomRight(transform.position), body.GetBottomRight(transform.position) + Vector3.down, obstacles);
        bool middleOneWay = Physics2D.Linecast(transform.position, transform.position + Vector3.down, oneWay);
        bool middleNormal = Physics2D.Linecast(transform.position, transform.position + Vector3.down, obstacles);

        if (middleNormal || leftNormal || rightNormal || !crouching) { return false; }
        if (leftOneWay || rightOneWay || middleOneWay) { return true; }
        return false;
    }

    RaycastHit2D RightBottomHit(float distance = 1)
    {
        int obstacles = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

        RaycastHit2D hitInfo = Physics2D.Linecast(body.GetBottomRight(transform.position), body.GetBottomLeft(transform.position) + Vector3.down * distance, obstacles);
        return hitInfo;
    }

    RaycastHit2D LeftBottomHit(float distance = 1)
    {
        int obstacles = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

        RaycastHit2D hitInfo = Physics2D.Linecast(body.GetBottomLeft(transform.position), body.GetBottomLeft(transform.position) + Vector3.down * distance, obstacles);
        return hitInfo;
    }

    RaycastHit2D MiddleBottomHit(float distance = 1)
    {
        int obstacles = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

        RaycastHit2D hitInfo = Physics2D.Linecast(transform.position, transform.position + Vector3.down * distance, obstacles);
        return hitInfo;
    }

    float TakeConveyerSpeed()
    {
        if (LeftBottomHit())
        {
            ConveyerBelt conveyerLeft = LeftBottomHit().collider.GetComponent<ConveyerBelt>();
            if (conveyerLeft) { return conveyerLeft.Speed(); }
        }
        else if (RightBottomHit())
        {
            ConveyerBelt conveyerRight = RightBottomHit().collider.GetComponent<ConveyerBelt>();
            if (conveyerRight) { return conveyerRight.Speed(); }
        }
        else
        {
            return 0;
        }

        return 0;


    }
}