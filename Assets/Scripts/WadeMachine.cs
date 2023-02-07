using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;
using UnityEngine.SceneManagement;
using System;

public partial class WadeMachine : Actor
{
    public WadeInventory Inventory;
    public WadeSound Sound;
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
    private float WallJumpForceTime = .32f;
    private Vector3 forceToVector;
    private bool canInteract;
    private Vector3 spawnPoint = Vector3.zero;
    [SerializeField] private GameObject particleSpawn;
    [SerializeField] private SpriteAnimation JumpParticle;
    [SerializeField] private SpriteAnimation WallJumpParticle;
    [SerializeField] private SpriteAnimation LandParticle;
    [SerializeField] private SpriteAnimation StartRunParticle;
    private Vector3 startPos;
    public GameObject HookshotRootObject;
    private HookshotRoot root;
    public GameObject hookshotKnife;
    public GameObject PointObject;
    public GameObject CurrentHookshotRoot;
    private float stillTimer;
    public float MoveTowardsSpeed;
    private Vector3 yOffset;
    public List<Vector2> previousPositions = new List<Vector2>();
    public bool hooked;







    // Start is called before the first frame update
    public void Start()
    {
        Application.targetFrameRate = 60;
        startPos = transform.position;
        Inventory = GetComponent<WadeInventory>();
        Sound = GetComponent<WadeSound>();
        CurrentWadeState = StNormal;
        inputs = GetComponent<WadeInputs>();
        health = startHealth;
        sprite = GetComponentInChildren<ObjectSprite>();
        enabled = true;
        yOffset = -Vector3.up * (size.y/2);
        // sprite.transform.parent = null;
    }

    // Update is called once per frame
    private void Update()
    {
        canHopDown = CanHopDown();
        Time.timeScale = .90f;
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

        hooked = Hooked();

        moveY = inputs.moveInput.y;

        shotTimer += Time.deltaTime;



        jumpGraceTimer += Time.deltaTime;

        hInputTimer += Time.deltaTime;

        if (invincibiltyTimer <= invincibiltyTime)
        {
            invincibiltyTimer += Time.deltaTime;
        }

        if(CurrentHookshotRoot)
        {
            CurrentHookshotRoot.transform.position = transform.position;
        }




        if (varJumpTimer > 0) { varJumpTimer -= Time.deltaTime; }
        if (stillTimer > -1) { stillTimer -= Time.deltaTime; }

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

        if ((onGround && jumpGraceTimer < jumpGraceTime || !onGround && airTimer < jumpGraceTime && inputs.jumpPress) && !canInteract)
        {
            if (!canHopDown)
            {
                Jump();
            }
            else
            {
                Vector3 nextPosition = -transform.up * (2 * CharacterConstants.SkinWidth + jumpDownDistance);
                varJumpTimer = 0;
                // transform.position = (body.RigidbodyComponent.Position + nextPosition);
            }
        }



        if (!onGround && stillTimer < 0)
        {
            if(Hooked())
            {
                Vector2 lookAtVector = root.currentPoint.transform.position;
                Vector2 myPosition = transform.position;
                Vector2 difference = (myPosition - lookAtVector).normalized;
                Vector2 perp = Vector2.Perpendicular(difference);

                Move(perp * root.currentForce);

                float currentDistance = Vector2.Distance(lookAtVector, myPosition);

                if(currentDistance > root.currentDistance)
                {
                    Move(-difference * (currentDistance - root.currentDistance));
                }

                previousPositions.Add(myPosition);

                for(int i = 1; i < previousPositions.Count; i++)
                {
                    Debug.DrawLine(previousPositions[i-1],previousPositions[i]);
                }
                Speed.y = 0;
            }
            else
            {
                if (hitUp) { varJumpTimer = 0; }

                conveyerAddition = MathHelper.Approach(conveyerAddition, 0, 55 * Time.deltaTime);

                if ((hitLeft || hitRight) && jumpGraceTimer < jumpGraceTime)
                {
                    WallJump(hitRight);
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

        }

        float lastSpeed = Speed.x;


        if(Hooked() && onGround)
        {
            Speed.x = MathHelper.Approach(Speed.x, 0, walkAcceleration * Time.deltaTime);
        }
        else if (moveX == 0 && !Hooked())
        {
            Speed.x = MathHelper.Approach(Speed.x, moveX * walkSpeed, walkAcceleration * AccelMultipler() * Time.deltaTime);
        }
        else if(!Hooked())
        {
            Speed.x = MathHelper.Approach(Speed.x, moveX * walkSpeed, walkDeceleration * AccelMultipler() * Time.deltaTime);
        }
        else
        {
            Speed.x = 0;
        }
        

        if(Mathf.Abs(lastSpeed) < 5f && Mathf.Abs(moveX) > 0.2f && onGround && CurrentWadeState == StNormal)
        {
            GameObject particle = Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
            particle.GetComponent<SpriteAnimationController>().direction = directionInt;
            particle.GetComponent<SpriteAnimationController>().Play(StartRunParticle);
        }
    }

    private void Jump(bool swing = false)
    {
        GameObject particle = Instantiate(particleSpawn, transform.position, Quaternion.identity);
        particle.GetComponent<SpriteAnimationController>().Play(JumpParticle);
        Sound.PlayJumpUp();
        Sound.PlayFootStep();
        hasShortHopped = false;
        sprite.scale = new Vector3(0.6f, 1.4f, 1);
        jumpGraceTimer = Mathf.Infinity;
        varJumpTimer = varJumpTime;
        
        if (Mathf.Sign(moveX) == Mathf.Sign(conveyerAddition) && conveyerAddition != 0 && moveX != 0)
        {
            Speed.x = (walkSpeed + conveyerJumpHSpeed) * sprite.direction;
            varJumpTimer = varJumpTime * .5f;
        }

        if(swing)
        {
            Speed.y += jumpSpeed;
        }
        else
        {
            Speed.y = jumpSpeed;
        }
        
    }

    private void WallJump(bool rightCollision)
    {
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
        if (Time.timeScale >= .7) { TransitionToState(StNormal); }
        invincibiltyTimer = 0;
    }

    void SwingUpdate()
    {
       HookshotRoot root = CurrentHookshotRoot.GetComponent<HookshotRoot>();

        Vector3 moveAmount = root.currentForce * -CurrentHookshotRoot.transform.up;
        Speed = moveAmount;
        CurrentHookshotRoot.transform.position = transform.position;

        if(root.currentDistance > root.distance)
        {
            Move(-root.transform.up * (root.currentDistance - root.distance));
        }
        

        if(inputs.jumpPress)
        {
            Jump(true);
            Destroy(CurrentHookshotRoot);
            TransitionToState(StNormal);
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


    float AccelMultipler()
    {
        float airMult = onGround ? 1 : .75f;

        if (moveX != 0)
        {
            return airMult;
        }
        else
        {
            if(onGround)
            {
                return 1f;
            }
            else
            {
                return 0.1f;
            }
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

        shootPoint = new Vector2(x, y);

        if (inputs.shootPress)
        {
            Sound.PlayGunShot();
            Debug.Log("youshot");
            Bullet newBullet = Instantiate(currentBullet, transform.position + new Vector3(shootPoint.x, shootPoint.y, 0) + yOffset, Quaternion.identity).GetComponent<Bullet>();
            newBullet.GetComponent<Bullet>().ChangeMoveDirection(shootDirection);
            shotTimer = 0;
        }

        if (inputs.item1Press)
        {
            if(CurrentHookshotRoot)
            {
                Destroy(CurrentHookshotRoot);
            }
            else
            {
                HookshotKnife newBullet = Instantiate(hookshotKnife, transform.position + new Vector3(shootPoint.x, shootPoint.y, 0) + yOffset, Quaternion.identity).GetComponent<HookshotKnife>();
                newBullet.GetComponent<HookshotKnife>().ChangeMoveDirection(shootDirection);
                shotTimer = 0;
                // stillTimer = 2;

                StartCoroutine(GameData.Instance.cameraMachine.CameraShake(2, .2f));
            }

        }




        // ddown = 12,2 dup = 12, 22 airdown = 0,0 up = 0, 27 forward = 17, 13 crouch 8, 13
    }

    public void HookShotStart(RaycastHit2D hitInfo)
    {

        float angle = Vector2.SignedAngle(hitInfo.normal, Speed);

        print(MathHelper.BalancedAngle(angle));

        Point pointToAdd = Instantiate(PointObject, hitInfo.point, Quaternion.identity).GetComponent<Point>();

        pointToAdd.normal = hitInfo.normal;

        CurrentHookshotRoot = Instantiate(HookshotRootObject, transform.position + Vector3.up * 9, transform.rotation);

        root = CurrentHookshotRoot.GetComponent<HookshotRoot>();
        root.startPoint = pointToAdd.gameObject;

    }



    public void TakeDamage(float recoilDirection, GameObject projectile = null)
    {
        if(!invincible)
        {
            if (invincibiltyTimer >= invincibiltyTime)
            {
                if (projectile) { Destroy(projectile); }
                directionInt = -recoilDirection;
                health -= 1;

                if (health <= 0)
                {
                    FindObjectOfType<DungeonManager>().SaveToJson();
                    Reset();
                }
                else
                {
                    TransitionToState(StHit);
                    Debug.Log("wadeIsHit");
                }
            }
        }

    }

    public bool Hooked()
    {
        if(!CurrentHookshotRoot)
        {
            return false;
        }
        else
        {

            if((!root.maxedOut && !inputs.triggerHeld))
            {
                return false;
            }


            if(onGround)
            {
                if(moveX == 0)
                {
                    return false;
                }

                float pointsDifference = Mathf.Sign(root.currentPoint.transform.position.x - transform.position.x);
                bool oppositeDirection = Mathf.Sign(moveX) != pointsDifference;


                if(!oppositeDirection)
                {
                    return false;
                }
            }

        }

        return true;
    }
    
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            if (onGround && Mathf.Sign(chest.transform.localScale.x) == -Mathf.Sign(directionInt) && jumpGraceTimer < .2f)
            {
                chest.OpenChest(this);
                forceToVector = chest.wadeToPosition;
                TransitionToState(StChest);
            }



        }

        if (lockedDoor)
        {
            canInteract = true;
            if (onGround && jumpGraceTimer < .2f)
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
            
        }


    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10 && CurrentWadeState.canGetHit)
        {
            teleportHit = true;
            TransitionToState(StHit);
        }
    }




    public void ChangeSpawnPoint(Vector3 newPoint)
    {
        spawnPoint = newPoint;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy && enemy.touchForDamage && CurrentWadeState.canGetHit)
        {
            TakeDamage(-directionInt);
        }

        if(collision.gameObject.layer == 7 && CurrentWadeState.canGetHit)
        {
            TakeDamage(-directionInt);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canInteract = false;
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


       
        
        // if (!LeftBottomHit(20)) { yPoint = RightBottomHit(20).point.y; }
        // else if (!RightBottomHit(20)) { yPoint = LeftBottomHit(20).point.y; }
        // else if(RightBottomHit(20).distance > LeftBottomHit(20).distance) { yPoint = LeftBottomHit(20).point.y; }
        // else { yPoint = RightBottomHit(20).point.y;}

        GameObject particle =Instantiate(particleSpawn, transform.position + yOffset, Quaternion.identity);
        particle.GetComponent<SpriteAnimationController>().Play(LandParticle);



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