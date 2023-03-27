using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public partial class WadeMachine : Actor
{
    public WadeInventory Inventory;
    public WadeSound Sound;
    private float walkSpeed = 120f;
    private float HalfGravThreshold = 40;
    private float walkAccel = 1250f;
    private float walkReduce = 500;
    private float jumpSpeed = 120f;
    private bool hasShortHopped;
    private float wallJumpHSpeed = 75;
    private float conveyerJumpHSpeed = 250;
    private float maxFall = -200f;
    private float gravity = 900f;
    private float swingSpeed = 500;
    private float swingAcceleration = 125;
    private float swingTimer;
    private float moveX;
    private float moveY;
    private float aimX;
    public Vector2 Speed;
    private float varJumpTime = .22f;
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
    private float swingSpeedYJumpThreshold = -150;
    private Trigger currentLauncher;
    private const float LaunchAccel = 35; 
    private const float LaunchSpeed = 425; 
    private const float EndLaunchSpeed = 190;
    private const float EndLaunchUpMult = .75f;
    private const float LaunchDistanceThreshold = 25; 
    private Vector2 launchDir;
    private bool launching;
    private bool onSpinWheel;
    private float spinWheelDirection = 1;
    private float varJumpSpeed;
    private float swingSpeedTurnThreshold = 40;
    private bool swingTurning;
    private bool swingFalling;







    // Start is called before the first frame update
    public void Start()
    {
        base.Start();
        Application.targetFrameRate = 60;
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
        stateTimer += Time.deltaTime;

        UpdateSprite();

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




        Move(Speed);
        

        WallChecks();
        CheckOnGround();
        

        // sprite.gameObject.transform.position = Vector3Int.RoundToInt(transform.position);

    }

    void NormalUpdate()
    {

        ShootStuff();

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

            if (hitUp) { varJumpTimer = 0; }

            conveyerAddition = MathHelper.Approach(conveyerAddition, 0, 55 * Time.deltaTime);

            if ((hitLeft || hitRight) && jumpGraceTimer < jumpGraceTime)
            {
                WallJump(hitRight);
            }

            airTimer += Time.deltaTime;

            float mult = (Math.Abs(Speed.y) < HalfGravThreshold && (inputs.jumpHeld)) ? .5f : 1f;

            Speed.y = MathHelper.Approach(Speed.y, maxFall, gravity * mult * Time.deltaTime);

            if (varJumpTimer > 0)
            {
                if(inputs.jumpHeld)
                {
                    Speed.y = jumpSpeed + swingYRemainder;
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

        if (Mathf.Abs(Speed.x) > walkSpeed && Mathf.Sign(Speed.x) == moveX)
        {
            //from walk speed
            Speed.x = MathHelper.Approach(Speed.x, moveX * walkSpeed , walkReduce * airMult * Time.deltaTime);
        }
        else
        {
            //to walk speed
            Speed.x = MathHelper.Approach(Speed.x, moveX * walkSpeed * ropeMult, walkAccel * airMult * Time.deltaTime);
        }

        if(stillTimer > 0)
        {
            print("still");
            Speed = Vector2.zero;
        }

        if(Mathf.Abs(lastSpeed) < 5f && Mathf.Abs(moveX) > 0.2f && onGround && CurrentWadeState == StNormal)
        {
            GameObject particle = Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
            particle.GetComponent<SpriteAnimationController>().direction = directionInt;
            particle.GetComponent<SpriteAnimationController>().Play(StartRunParticle);
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
        hasShortHopped = false;
        sprite.scale = new Vector3(0.6f, 1.4f, 1);
        jumpGraceTimer = Mathf.Infinity;
        varJumpTimer = varJumpTime;
        varJumpSpeed = Speed.y;
        
        if (Mathf.Sign(moveX) == Mathf.Sign(conveyerAddition) && conveyerAddition != 0 && moveX != 0)
        {
            Speed.x = (walkSpeed + conveyerJumpHSpeed) * sprite.direction;
            varJumpTimer = varJumpTime * .5f;
        }

        if(Speed.y > jumpSpeed)
        {
            swingYRemainder = Speed.y - jumpSpeed;
        }
        else
        {
            swingYRemainder = 0;
        }

        {
            Speed.y = jumpSpeed ;
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
        if (Time.timeScale >= .7) { Time.timeScale = 1; Reset(); }
        invincibiltyTimer = 0;
    }




    void ClimbUpdate()
    {

        // bool below = transform.position.y < points[0].transform.position.y;

        // if(below)
        // {

        // }

        if(!hitRight && !hitLeft)
        {
            TransitionToState(StSwing);
        }


        Speed.y = MathHelper.Approach(Speed.y, climbSpeed * moveY, climbAcceleration * Time.deltaTime);
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
            currentSwingSpeed = swingSpeed/1.25f * -difference.normalized.x;
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
            Speed.y = MathHelper.Approach(Speed.y, - swingSpeed, gravity * Time.deltaTime);
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
                currentSwingSpeed = Mathf.MoveTowards(currentSwingSpeed, spinWheelDirection * swingSpeed, swingAcceleration * 2 * Time.deltaTime);
            }
            else
            {
                currentSwingSpeed = Mathf.MoveTowards(currentSwingSpeed, verticalty * swingSpeed, swingAcceleration * Mathf.Abs(2 * verticalty) * mult * Time.deltaTime);
            }

            if(moveY < 0)
            {
                Root.currentDistance -= moveY * 60 * Time.deltaTime;
            }

            currentSwingSpeed = Mathf.Clamp(currentSwingSpeed, -swingSpeed, swingSpeed);

            Vector2 distanceHelper = Vector2.zero;

            

            if(Mathf.Abs(currentDistance - Root.currentDistance) > 5)
            {
                distanceHelper = (-difference * 4 * (currentDistance - Root.currentDistance));
            }

            Speed = (perp * currentSwingSpeed) + distanceHelper;

        }

        if((hitLeft || hitRight))
        {

            float differenceX = points[0].transform.position.x - transform.position.x;

            bool below = transform.position.y < points[0].transform.position.y;



            bool inFront = hitLeft ? differenceX <= 0 : differenceX >= 0;

            if(below && inFront)
            {
                bool bothCorners = currentSolid.ContainsY(Top().y) && currentSolid.ContainsY(Bottom().y);

                if(!bothCorners)
                {
                    bool atTop = currentSolid.ContainsY(Bottom().y);

                    float distance;

                    if(atTop)
                    {
                        distance = Mathf.Abs(currentSolid.Top().y - Bottom().y);

                        if(distance > 10)
                        {
                            
                            TransitionToState(StClimb);
                        }
                        else
                        {
                            transform.position += Vector3.up * distance;
                        }
                    }
                    else
                    {
                        bool atBottom = currentSolid.ContainsY(Top().y);

                        if(atBottom)
                        {
                            distance = Mathf.Abs(currentSolid.Bottom().y - Bottom().y);
                            transform.position += Vector3.up * distance;
                            TransitionToState(StClimb);
                        }
                    }


                }
                else
                {
                    TransitionToState(StClimb);
                }
            }
        }

        if(hitUp)
        {
            currentSwingSpeed = 0;
        }

        if(jumpGraceTimer < jumpGraceTime)
        {
            

            if(Mathf.Abs(Speed.x) > 0)
            {
                forceMoveXDirection = Mathf.Sign(Speed.x);

                float forceXTime = Mathf.Lerp(0.2f, 0.4f, Mathf.Abs(Speed.x)/ swingSpeed);

                forceMoveXTimer = forceXTime;
            }



            if(Speed.y > -150)
            {
                Jump();
            }
            else
            {
                print(Speed.x);
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
        currentLauncher = trigger;
        TransitionToState(StLaunch);
        print("launchstart");
    }


    void LaunchUpdate()
    {


        if(!launching)
        {
            Speed = Vector2.Lerp(Speed, launchDir * LaunchSpeed, LaunchAccel * Time.deltaTime);
            
            if(Vector2.Distance(currentLauncher.Center(), transform.position) < LaunchDistanceThreshold)
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
    }

    void ChestUpdate()
    {
        transform.position = forceToVector;

        sprite.Play(sprite.SmallChest);
        if (sprite.imageIndex == sprite.SmallChest.totalFrames - 1)
        {
            OnGroundHit();
            float squish = Mathf.Min(maxFall / maxFall, 1);
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
                    HookshotKnife newBullet = Instantiate(hookshotKnife, transform.position + shootPoint + yOffset, Quaternion.identity).GetComponent<HookshotKnife>();
                    newBullet.GetComponent<HookshotKnife>().ChangeMoveDirection(shootDirection);
                    shotTimer = 0;
                    stillTimer = .5f;

                    StartCoroutine(GameData.Instance.cameraMachine.CameraShake(2, .2f));
                }
            }

            if(inputs.item1Press)
            {
                Sound.PlayGunShot();
                Debug.Log("youshot");
                Bullet newBullet = Instantiate(currentBullet, transform.position + shootPoint + yOffset, Quaternion.identity).GetComponent<Bullet>();
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(shootDirection);
                stillTimer = .5f;
                StartCoroutine(GameData.Instance.cameraMachine.CameraShake(2, .2f));
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
        transform.position = startPos;
        TransitionToState(StNormal);
    }

    void UpdateSprite()
    {
        if (CurrentWadeState == StNormal)
        {
            if (!onGround)
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

            Root.RenderRope(shootPoint + transform.position + yOffset);


            
        }

        if (CurrentWadeState == StHit)
        {
            sprite.Play(sprite.Hit, true);
        }

        sprite.scale.x = MathHelper.Approach(sprite.scale.x, 1f, 2.75f * Time.deltaTime);
        sprite.scale.y = MathHelper.Approach(sprite.scale.y, 1f, 2.75f * Time.deltaTime);
    }


    public void ChangeSpawnPoint(Vector3 newPoint)
    {
        spawnPoint = newPoint;
    }


    public override void OnTriggerHit(Trigger trigger)
    {


        if(trigger.gameObject.layer == 10)
        {
            TakeDamage();
        }
    }


    public override void OnGroundHit()
    {
        Sound.PlayJumpLand();
        airTimer = 0;
        forceMoveXTimer = 0;
        float squish = Mathf.Min(Speed.y / maxFall, 1);
        sprite.scale.x = MathHelper.Approach(1, 1.4f, squish);
        sprite.scale.y = MathHelper.Approach(1, .6f, squish);
        Speed.y = 0;
        TransitionToState(StNormal);
       
        
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