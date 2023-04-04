using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public partial class WadeMachine : Actor
{
    public WadeInventory Inventory;
    public WadeSound Sound;
    private const float WalkSpeed = 120f;
    private const float HalfGravThreshold = 40;
    private const float WalkAccel = 1250f;
    private const float walkReduce = 500;
    private const float JumpSpeed = 125f;
    private const float VarJumpTime = .22f;
    private const float VarWallJumpTime = .22f;
    private const float WallJumpHSpeed = 75;
    private const float conveyerJumpHSpeed = 250;
    private const float MaxFall = -200f;
    private const float Gravity = 900f;
    private const float SwingSpeed = 385;
    private const float SwingAccel = 125;

    private const float SwingTurnAroundTime = 0.1f;
    private const float WallSlideSpeed = -20;
    private const float WallSlideAccel = 100;
    private const float WallSlideTime = 1.2f;
    private float swingTimer;
    private float moveX;
    private float wallSlideTimer;
    private int wallSlideDir;
    private float turnAroundTimer;
    private float moveY;
    private float aimX;
    public Vector2 Speed;
    private float varJumpTimer = 0;
    private float spriteLerp;
    private WadeSprite sprite;
    public float directionInt;
    private float shotTimer;
    private float shotAnimationCooldown = 4;
    private bool crouching;
    private Vector2 shootDirection;
    private Vector3 shootPoint;
    [SerializeField] GameObject currentBullet; 
    private float airTimer = 0;
    private float jumpGraceTimer = 0;
    private float jumpGraceTime = 0.1f;
    private int health;
    private int startHealth = 5;
    public bool canOpenChest;
    [SerializeField] private float conveyerAddition;
    private float hInputTimer = 0;
    private bool teleportHit;
    private float invincibiltyTimer = 0;
    private float invincibiltyTime = 1;
    [SerializeField] private bool invincible;
    float jumpDownDistance = 0.08f;
    private WadeInputs inputs;
    [SerializeField] bool canHopDown;
    private float forceMoveXTimer = 0;
    private float forceMoveXDirection = 0;
    private float WallJumpForceTime = .16f;
    private Vector3 forceToVector;
    private bool canInteract;
    private Vector3 spawnPoint = Vector3.zero;
    [SerializeField] private GameObject particleSpawn;
    [SerializeField] private SpriteAnimation JumpParticle;
    [SerializeField] private SpriteAnimation WallJumpParticle;
    [SerializeField] private SpriteAnimation LandParticle;
    [SerializeField] private SpriteAnimation StartRunParticle;
    private Vector3 startPos;
    private HookshotRoot Root;
    public GameObject hookshotKnife;
    private HookshotKnife currentHookshotKnife;
    public GameObject PointObject;
    private float stillTimer;
    private Vector3 yOffset;
    private float currentSwingSpeed;
    private float swingYRemainder;
    private float climbSpeed = 45;
    private float kickOffSpeed = 150;
    private float climbAcceleration = 1000;
    float hookSpeed = 4;
    private float stateTimer;
    private float shotCoolDown = 1f;
    private float SwingSpeedYJumpThreshold = -150;
    private Vector3 ropeRenderPos;
    private const float LaunchAccel = 1100; 
    private const float LaunchSpeed = 425; 
    private const float EndLaunchSpeed = 190;
    private const float EndLaunchUpMult = .75f;
    private float launchDistance; 
    private Vector3 launchStartPos; 
    private Vector2 launchDir;
    private bool launching;
    private bool onSpinWheel;
    private float spinWheelDirection = 1;
    private float varJumpSpeed;
    private float SwingSpeedTurnThreshold = 40;
    private bool swingTurning;
    private bool swingFalling;







    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        startPos = transform.position;
        spawnPoint = startPos;
        Inventory = GetComponent<WadeInventory>();
        Sound = GetComponent<WadeSound>();
        Root = GetComponent<HookshotRoot>();
        CurrentWadeState = StNormal;
        inputs = GetComponent<WadeInputs>();
        health = startHealth;
        sprite = GetComponentInChildren<WadeSprite>();
        enabled = true;
        yOffset = -Vector3.up * (size.y/2);
    }

    // Update is called once per frame
    private void Update()
    {
        if(inputs.startPress)
        {
            Reset();

        }
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

        if (varJumpTimer >= 0) { varJumpTimer -= Time.deltaTime; }
        if (stillTimer >= 0) { stillTimer -= Time.deltaTime; }
        if (swingTimer >= 0) { swingTimer -= Time.deltaTime; }
        if (turnAroundTimer >= 0) { turnAroundTimer -= Time.deltaTime; }
        stateTimer += Time.deltaTime;

       

        float lastDirection = directionInt;

        CharacterFlip();
        

        if(lastDirection != directionInt && CurrentWadeState == StNormal)
        {
            if (onGround)
            {
                GameObject particle = Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
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

        if (CurrentWadeState == StSwing)
        {
            SwingUpdate();
        }


        if (CurrentWadeState == StLaunch)
        {
            LaunchUpdate();
        }

        if (CurrentWadeState == StClimb)
        {
            ClimbUpdate();
        }

        if (CurrentWadeState == StHit)
        {
            HitUpdate();
        }

        if (CurrentWadeState == StChest)
        {
            ChestUpdate();
        }

        UpdateSprite();

        if (wallSlideDir != 0)
        {
            wallSlideTimer = Mathf.Max(wallSlideTimer - Time.deltaTime, 0);
            wallSlideDir = 0;
        }
      




        Move(Speed);
        

        WallChecks();
        CheckOnGround();
        

        // sprite.gameObject.transform.position = Vector3Int.RoundToInt(transform.position);

    }

    void NormalUpdate()
    {

        ShootStuff();

        if(stillTimer < 0)
        {
            if (onGround)
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

            if ((onGround && jumpGraceTimer < jumpGraceTime || !onGround && airTimer < jumpGraceTime && jumpGraceTimer < jumpGraceTime) && !canInteract)
            {

                Jump();
                
            }

            if (!onGround)
            {

                float max = MaxFall;

                if( (hitLeft || hitRight) && Speed.y <= 0 && moveX == sprite.direction)
                {
                    wallSlideDir = hitLeft ? -1 : 1;
                    max = Mathf.Lerp(max, WallSlideSpeed, wallSlideTimer / WallSlideTime);
                }  

                if (hitUp) { varJumpTimer = 0; }
    
                conveyerAddition = MathHelper.Approach(conveyerAddition, 0, 55 * Time.deltaTime);

                if ((hitLeft || hitRight) && jumpGraceTimer < jumpGraceTime)
                {
                    WallJump(hitRight);
                }

                airTimer += Time.deltaTime;

                float mult = (Math.Abs(Speed.y) < HalfGravThreshold && (inputs.jumpHeld)) ? .5f : 1f;

                Speed.y = MathHelper.Approach(Speed.y, max, Gravity * mult * Time.deltaTime);

                if (varJumpTimer > 0)
                {
                    if(inputs.jumpHeld)
                    {
                        Speed.y = varJumpSpeed;
                    }
                    else
                    {
                        varJumpTimer = 0;
                    }
                }
            }

            float lastSpeed = Speed.x;

            float ropeMult;

            if(!Root.active)
            {
                ropeMult = 1;
            }
            else
            {
                if(Mathf.Sign(moveX) == Mathf.Sign(transform.position.x - Root.currentPoint.transform.position.x))
                {
                    ropeMult = .4f;
                }
                else
                {
                    ropeMult = 1f;
                }
            }

            float airMult = onGround ? 1 : .65f;

            if (Mathf.Abs(Speed.x) > WalkSpeed && Mathf.Sign(Speed.x) == moveX)
            {
                //from walk speed
                Speed.x = MathHelper.Approach(Speed.x, moveX * WalkSpeed , walkReduce * airMult * Time.deltaTime);
            }
            else
            {
                //to walk speed
                Speed.x = MathHelper.Approach(Speed.x, moveX * WalkSpeed * ropeMult, WalkAccel * airMult * Time.deltaTime);
            }

            if(Mathf.Abs(lastSpeed) < 5f && Mathf.Abs(moveX) > 0.2f && onGround && CurrentWadeState == StNormal)
            {
                GameObject particle = Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
                particle.GetComponent<SpriteAnimationController>().direction = directionInt;
                particle.GetComponent<SpriteAnimationController>().Play(StartRunParticle);
            }

        }
        else
        {
            Speed = Vector2.Lerp(Speed,Vector2.zero, 15 * Time.deltaTime);
        }



        
    }


    private void Jump()
    {
        if(Root.active)
        {
            Root.Reset();
        }

        GameObject particle = Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
        particle.GetComponent<SpriteAnimationController>().Play(JumpParticle);
        Sound.PlayJumpUp();
        Sound.PlayFootStep();
        sprite.scale = new Vector3(0.6f, 1.4f, 1);
        jumpGraceTimer = Mathf.Infinity;
        varJumpTimer = VarJumpTime;
        
        if (Mathf.Sign(moveX) == Mathf.Sign(conveyerAddition) && conveyerAddition != 0 && moveX != 0)
        {
            Speed.x = (WalkSpeed + conveyerJumpHSpeed) * sprite.direction;
            varJumpTimer = VarJumpTime* .5f;
        }

        if(Speed.y > JumpSpeed)
        {
            swingYRemainder = Speed.y - JumpSpeed;
        }
        else
        {
            swingYRemainder = 0;
        }

        {
            Speed.y = JumpSpeed + swingYRemainder;
        }

        varJumpSpeed = Speed.y;


        if(CurrentWadeState != StNormal)
        {
            TransitionToState(StNormal);
        }
        
    }

    private void WallJump(bool rightCollision)
    {

        if(Root.active)
        {
            Root.Reset();
        }

        GameObject particle = Instantiate(
            particleSpawn, 
            transform.position + transform.right * size.x / 2 * directionInt - transform.up * size.y/2,
            Quaternion.identity
            );

        particle.GetComponent<SpriteAnimationController>().Play(WallJumpParticle);
        particle.GetComponent<SpriteAnimationController>().direction = -directionInt;

        if(Speed.y < JumpSpeed){ swingYRemainder = 0; }
        float moveDirection = rightCollision ? -1 : 1;
        Sound.PlayJumpUp();
        Sound.PlayFootStep();
        jumpGraceTimer = Mathf.Infinity;
        sprite.scale = new Vector3(0.6f, 1.4f, 1);
        varJumpTimer = VarJumpTime/1.5f;
        forceMoveXDirection = moveDirection;
        forceMoveXTimer = WallJumpForceTime;
        Speed.x = (WalkSpeed + WallJumpHSpeed) * forceMoveXDirection;
        Speed.y = JumpSpeed + swingYRemainder;
        varJumpSpeed = JumpSpeed + swingYRemainder;
        
    }

    void HitUpdate()
    {
        Time.timeScale = MathHelper.Approach(Time.timeScale, 1, Time.timeScale * 15 * Time.deltaTime);
        Speed.x = MathHelper.Approach(Speed.x, 0, 400 * Time.deltaTime);
        Speed.y = MathHelper.Approach(Speed.y, 0, 400 * Time.deltaTime);
        if (Time.timeScale >= .7) { Time.timeScale = 1; Reset(); }
        invincibiltyTimer = 0;
    }




    void ClimbUpdate()
    {
        List<Point> points = Root.points;
        bool below = transform.position.y < points[0].transform.position.y;

        if(below)
        {

            Speed.y = MathHelper.Approach(Speed.y, climbSpeed * moveY, climbAcceleration * Time.deltaTime);
        }
        else
        {
            if(moveY < 0)
            {
                Speed.y = MathHelper.Approach(Speed.y, climbSpeed * moveY, climbAcceleration * Time.deltaTime);
            }
            else
            {
                Speed.y = MathHelper.Approach(Speed.y, 0, climbAcceleration * Time.deltaTime);
            }
        }

        if(!hitRight && !hitLeft)
        {
            TransitionToState(StSwing);
        }



        Speed.x = 0;

        if(inputs.jumpPress)
        {
            WallJump(hitRight);
            TransitionToState(StNormal);
        }

    }


    public void HookshotStart(Hit hitInfo, bool spinWheel = false)
    {
        onSpinWheel = spinWheel;

        Vector3 addedPosition = onSpinWheel ? hitInfo.aabb.Center() : hitInfo.point + hitInfo.normal * 3;

        Point pointToAdd = Instantiate(PointObject, addedPosition, Quaternion.identity).GetComponent<Point>();

        pointToAdd.normal = hitInfo.normal;



        shotTimer = 1;

        Root.GetStartingStats(pointToAdd.GetComponent<Point>(), hitInfo);

        List<Point> points = Root.points;
        Vector2 lookAtVector = points[0].transform.position;
        Vector2 myPosition = transform.position;
        Vector2 difference = (myPosition - lookAtVector).normalized;
        
        if(hitInfo.normal.y > 0)
        {
            swingFalling = true;
        }

        if(!onGround)
        {
            currentSwingSpeed = SwingSpeed/1.25f * -difference.normalized.x;
            spinWheelDirection = Mathf.Sign(currentSwingSpeed);
            TransitionToState(StSwing);
        }
    }



    void SwingUpdate()
    {


        List<Point> points = Root.points;

        float currentDistance = Vector2.Distance(transform.position, points[0].transform.position);

        Vector2 lookAtVector = points[0].transform.position;
        Vector2 myPosition = transform.position;
        Vector2 difference = (myPosition - lookAtVector).normalized;
        Vector2 perp = Vector2.Perpendicular(difference);

        float verticalty = -difference.normalized.x;

        float mult = transform.position.y > points[0].transform.position.y ? 4 : 1;


        if(swingFalling)
        {
            Speed.y = MathHelper.Approach(Speed.y, - SwingSpeed, Gravity * Time.deltaTime);
            currentSwingSpeed = Speed.y * -verticalty;

            if(lookAtVector.y > myPosition.y)
            {
                swingFalling = false;
            }

        }
        else
        {

            if(onSpinWheel)
            {
                currentSwingSpeed = Mathf.MoveTowards(currentSwingSpeed, spinWheelDirection * SwingSpeed, SwingAccel * 2 * Time.deltaTime);
            }
            else
            {
                currentSwingSpeed = Mathf.MoveTowards(currentSwingSpeed, verticalty * SwingSpeed + (moveX * 100), SwingAccel * Mathf.Abs(2 * verticalty) * mult * Time.deltaTime);
            }

            if(moveY <= 0)
            {
                Root.currentDistance -= moveY * 100 * (1-verticalty) * Time.deltaTime;
            }

            currentSwingSpeed = Mathf.Clamp(currentSwingSpeed, -SwingSpeed, SwingSpeed);

            Vector2 distanceHelper = Vector2.zero;

            

            if(Mathf.Abs(currentDistance - Root.currentDistance) > 5)
            {
                distanceHelper = (-difference * 4 * (currentDistance - Root.currentDistance));
            }

            Speed = (perp * currentSwingSpeed) + distanceHelper;

        }

        float signMove = Mathf.Sign(currentSwingSpeed);

        if((hitLeft && signMove == -1) || (hitRight && signMove == 1))
        {

            float differenceX = points[0].transform.position.x - transform.position.x;

            bool below = transform.position.y < points[0].transform.position.y;


            bool inFront = hitLeft ? differenceX <= 0 : differenceX >= 0;

            if(below && inFront)
            {
                TransitionToState(StClimb);
            }
            else
            {
                if(turnAroundTimer < 0)
                {
                    currentSwingSpeed *= -1;
                    turnAroundTimer = SwingTurnAroundTime;
                }
                
            }
        }

        // if(hitUp)
        // {
        //     currentSwingSpeed *= -1;
        // }

        if(jumpGraceTimer < jumpGraceTime)
        {
            

            if(Mathf.Abs(Speed.x) > 0)
            {
                forceMoveXDirection = Mathf.Sign(Speed.x);

                float forceXTime = Mathf.Lerp(0.2f, 0.4f, Mathf.Abs(Speed.x)/ SwingSpeed);

                forceMoveXTimer = forceXTime;
            }



            if(Speed.y > -150)
            {
                Jump();
            }
            else
            {

                Root.Reset();
                TransitionToState(StNormal);
            }

            

        }

    }

    public void LaunchStart(Trigger trigger, Vector3 direction)
    {
        stillTimer = 0;
        launching = false;
        launchDir = direction;
        ropeRenderPos = trigger.Center();
        launchDistance = Vector2.Distance(transform.position, trigger.Center());
        launchStartPos = transform.position;
        TransitionToState(StLaunch);
        Speed = Vector2.zero;

    }


    void LaunchUpdate()
    {


        if(!launching)
        {
            Speed = Vector2.MoveTowards(Speed, launchDir * LaunchSpeed, LaunchAccel * Time.deltaTime);
            
            if(Vector2.Distance(launchStartPos, transform.position) > launchDistance)
            {
                launching = true;
                stateTimer = 0;
            }
            
        }
        else
        {
            Speed = launchDir * LaunchSpeed;
            if(stateTimer > 0.15f)
            {
                if(launchDir.y >= 0)
                {
                    Speed = launchDir * EndLaunchSpeed;
                }

                if(Speed.y > 0)
                {
                    Speed.y *= EndLaunchUpMult;
                }
                shotTimer = 400;
                
                TransitionToState(StNormal);
            }

            
            
        }


        RenderRope(shootPoint + transform.position + yOffset, ropeRenderPos, launching);

    }

    void ChestUpdate()
    {
        transform.position = forceToVector;

        sprite.Play(sprite.SmallChest);
        if (sprite.imageIndex == sprite.SmallChest.totalFrames - 1)
        {
            OnGroundHit();
            float squish = Mathf.Min(MaxFall / MaxFall, 1);
            sprite.scale.x = MathHelper.Approach(1, 1.4f, squish);
            sprite.scale.y = MathHelper.Approach(1, .6f, squish);
            TransitionToState(StNormal);
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
                y = 2;
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
                if (onGround)
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

        shootPoint = new Vector3(x, y, 0);


        if(shotTimer > shotCoolDown)
        {
            if (inputs.shootPress)
            {
                if(Root.active)
                {
                    Root.Reset();
                }
                else
                {
                    currentHookshotKnife = Instantiate(hookshotKnife, transform.position + shootPoint + yOffset, Quaternion.identity).GetComponent<HookshotKnife>();
                    currentHookshotKnife.ChangeMoveDirection(shootDirection);
                    shotTimer = 0;
                    stillTimer = .5f;

                    StartCoroutine(GameData.Instance.cameraMachine.CameraShake(.5f, .1f));
                }
            }

            if(inputs.item1Press)
            {
                Sound.PlayGunShot();
                Bullet newBullet = Instantiate(currentBullet, transform.position + shootPoint + yOffset, Quaternion.identity).GetComponent<Bullet>();
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(shootDirection);
                // StartCoroutine(GameData.Instance.cameraMachine.CameraShake(2, .2f));
                shotTimer = 0;
            }
        }






        // ddown = 12,2 dup = 12, 22 airdown = 0,0 up = 0, 27 forward = 17, 13 crouch 8, 13
    }


    public void TakeDamage(float recoilDirection = 0, GameObject projectile = null)
    {
        TransitionToState(StHit);
    }

    public void Reset()
    {
        Root.Reset();
        transform.position = GameData.Instance.cameraMachine.currentCameraBox.startPos.position;
        TransitionToState(StNormal);
    }

    void UpdateSprite()
    {
        if (CurrentWadeState == StNormal)
        {
            if (!onGround)
            {

                if(wallSlideDir != 0)
                {
                    sprite.Play(sprite.WallSlide);
                }
                else if (moveY > 0)
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

            if (onGround)
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

        if(CurrentWadeState == StClimb)
        {
            sprite.Play(sprite.WallClimb);
        }

        if(CurrentWadeState == StSwing)
        {

            

            Vector2 lookAtVector = Root.points[0].transform.position;
            float vertAngle = MathHelper.RegulateAngle(GameData.Instance.WadeVerticalAngle(lookAtVector), MathHelper.SignedAngles45);

            sprite.direction = Mathf.Sign(lookAtVector.x - transform.position.x);

            float absVert = Mathf.Abs(vertAngle);

            float x;
            float y;

            if(absVert == 0)
            {
                x = 0;
                y = 27;
                sprite.Play(sprite.JumpAimUp);
            }
            else if( absVert == 45)
            {
                x = 12;
                y = 20;
                sprite.Play(sprite.JumpAimDUp);
            }
            else if( absVert == 90)
            {
                x = 14;
                y = 14;
                sprite.Play(sprite.JumpAimForward);
            }
            else if(absVert == 135)
            {
                x = 16;
                y = 2;
                sprite.Play(sprite.JumpAimDDown);
            }
            else
            {
                x = 0;
                y = -5;
                sprite.Play(sprite.JumpAimDown);
            }

            x *= sprite.direction;

            shootPoint = new Vector3(x,y,0);

           


            
        }

        if (CurrentWadeState == StHit)
        {
            sprite.Play(sprite.Hit, true);
        }

        if(Root.active)
        {
            Root.RenderRope(shootPoint + transform.position + yOffset);
        }
        else if(currentHookshotKnife)
        {
            RenderRope(shootPoint + transform.position + yOffset, currentHookshotKnife.transform.position);
        }
        



        

        sprite.scale.x = MathHelper.Approach(sprite.scale.x, 1f, 2.75f * Time.deltaTime);
        sprite.scale.y = MathHelper.Approach(sprite.scale.y, 1f, 2.75f * Time.deltaTime);
    }


    public void ChangeSpawnPoint(Vector3 newPoint)
    {
        spawnPoint = newPoint;
    }

    public void RenderRope(Vector3 startPoint, Vector3 endPoint, bool zero = false)
    {
        if(zero)
        {
            Root.rope.positionCount = 0;
        }
        else
        {
            Vector3[] positions = new Vector3[2];
            positions[1] = startPoint;
            positions[0] = endPoint;
            Root.rope.positionCount = 2;
            Root.rope.SetPositions(positions);
        }
    }


    public override void OnTriggerHit(Trigger trigger)
    {
        if(trigger is Spike)
        {
            if(onGround)
            {
                TakeDamage();
            }
            Spike spike = trigger as Spike;

            if(spike.direction.x == 0)
            {
                if(Mathf.Sign(Speed.y) == spike.direction.y)
                {
                    TakeDamage();
                }
            }
            else
            {
                if(Mathf.Sign(Speed.x) == spike.direction.x)
                {
                    TakeDamage();
                }
            }
            
            
        }
    }


    public override void OnGroundHit()
    {
        Sound.PlayJumpLand();
        airTimer = 0;
        forceMoveXTimer = 0;
        float squish = Mathf.Min(Speed.y / MaxFall, 1);
        sprite.scale.x = MathHelper.Approach(1, 1.4f, squish);
        sprite.scale.y = MathHelper.Approach(1, .6f, squish);
        Speed.y = 0;
        TransitionToState(StNormal);
        wallSlideTimer = WallSlideTime;
        
        // if (!LeftBottomHit(20)) { yPoint = RightBottomHit(20).point.y; }
        // else if (!RightBottomHit(20)) { yPoint = LeftBottomHit(20).point.y; }
        // else if(RightBottomHit(20).distance > LeftBottomHit(20).distance) { yPoint = LeftBottomHit(20).point.y; }
        // else { yPoint = RightBottomHit(20).point.y;}

        GameObject particle =Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
        particle.GetComponent<SpriteAnimationController>().Play(LandParticle);



    }

    public override void OnNoGroundHit()
    {
        if(Root.active)
        {
            Root.Reset();
        }
    }

    void OnGUI()
    {

    }

    bool CanHopDown()
    {

        return false;
    }



    float TakeConveyerSpeed()
    {
        // if (LeftBottomHit())
        // {
        //     ConveyerBelt conveyerLeft = LeftBottomHit().collider.GetComponent<ConveyerBelt>();
        //     if (conveyerLeft) { return conveyerLeft.Speed(); }
        // }
        // else if (RightBottomHit())
        // {
        //     ConveyerBelt conveyerRight = RightBottomHit().collider.GetComponent<ConveyerBelt>();
        //     if (conveyerRight) { return conveyerRight.Speed(); }
        // }
        // else
        // {
        //     return 0;
        // }
        return 0;
    }
}