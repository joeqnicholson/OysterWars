using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactoryForemanState
{
    Idle, Shoot, Stab, Wrench
}

public class FactoryForemanMachine : Enemy
{
    [SerializeField] SpriteAnimation Idle;
    [SerializeField] SpriteAnimation Shoot;
    [SerializeField] SpriteAnimation Stab;
    [SerializeField] SpriteAnimation Wrench;
    private SpriteAnimationController sprite;
    private FactoryForemanState CurrentState;
    private FactoryForemanState LastAttack;
    private bool hasTriggeredAnimation;
    private float idleTime;
    private float stateTimer;
    [SerializeField] GameObject bullet;
    private Vector3 firePoint;
    private int wrenchThrows;
    public float wadeDistance;
    public Vector3 originalPosition;
    private BoxCollider2D slashBox;
    [SerializeField] StationaryShooterEnemy ventBoy1;
    [SerializeField] StationaryShooterEnemy ventBoy2;


    protected override void Start()
    {
        slashBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        slashBox.enabled = false;
        idleTime = Random.Range(.5f, 2f);
        TransitionToState(FactoryForemanState.Idle);
        base.Start();
        sprite = GetComponent<SpriteAnimationController>();
        sprite.direction = transform.localScale.x;
        originalPosition = transform.position;
    }

    void TransitionToState(FactoryForemanState state)
    {
        FactoryForemanState fromState = CurrentState;

        if(state == FactoryForemanState.Wrench && fromState == FactoryForemanState.Wrench) { sprite.ResetPlay(); }

        if(state == FactoryForemanState.Idle) { idleTime = Random.Range(.5f, 2f); }

        stateTimer = 0;
        CurrentState = state;
        hasTriggeredAnimation = false;
    }

    
    void Update()
    {

        if(health < startHealth / 2)
        {
            ventBoy1.BecomeActive();
            ventBoy2.BecomeActive();
        }
        
        stateTimer += Time.deltaTime;
        switch (CurrentState)
        {
            case FactoryForemanState.Idle:
                {
                    slashBox.enabled = false;
                    wadeDistance = WadeDistanceVector().x;
                    sprite.Play(Idle);
                    if (stateTimer > idleTime)
                    {
                        if(Mathf.Abs(wadeDistance) > 130)
                        {
                            int stateNumber = Random.Range(1, 4);
                            if (stateNumber == 1)
                            {
                                wrenchThrows = 4;
                                LastAttack = FactoryForemanState.Wrench;
                                TransitionToState(FactoryForemanState.Wrench);
                            }
                            else if (stateNumber == 2)
                            {
                                LastAttack = FactoryForemanState.Shoot;
                                TransitionToState(FactoryForemanState.Shoot);
                            }
                            else if (stateNumber == 3)
                            {
                                TransitionToState(
                                    LastAttack == FactoryForemanState.Shoot ?
                                    FactoryForemanState.Wrench :
                                    FactoryForemanState.Shoot
                                    );
                            }
                        }
                        else
                        {
                            TransitionToState(FactoryForemanState.Stab);
                        }
                            
                           
                        
                       

                    }
                    break;
                }
            case FactoryForemanState.Shoot:
                {
                    sprite.Play(Shoot);
                    if (sprite.imageIndex > 1)
                    {
                        RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position + new Vector3(sprite.direction * 31, 41), 80,Vector3.zero);
                        if (hitInfo)
                        {
                            if (hitInfo.collider.GetComponent<WadeMachine>())
                            {
                                hitInfo.collider.GetComponent<WadeMachine>().TakeDamage(sprite.direction);
                            }
                        }
                    }

                    
                    if (sprite.stopped)
                    {
                        TransitionToState(FactoryForemanState.Idle);
                    }

                    break;
                }
            case FactoryForemanState.Wrench:
                {
                    sprite.Play(Wrench);
                    if (sprite.stopped)
                    {
                        wrenchThrows -= 1;

                        if(wrenchThrows > 0)
                        {
                            TransitionToState(FactoryForemanState.Wrench);
                        }
                        else
                        {
                            TransitionToState(FactoryForemanState.Idle);

                        }
                    }
                    break;
                }
            case FactoryForemanState.Stab:
                {

                    sprite.Play(Stab);
                    if (sprite.imageIndex > 2)
                    {
                        slashBox.enabled = true;
                    }

                    if (sprite.stopped)
                    {
                        slashBox.enabled = false;
                        transform.position = originalPosition;
                        TransitionToState(FactoryForemanState.Idle);
                    }

                    
                    break;
                }

        }
        HandleAnimationTriggers();
        sprite.scale.x = MathHelper.Approach(sprite.scale.x, 1f, 2.75f * Time.deltaTime);
        sprite.scale.y = MathHelper.Approach(sprite.scale.y, 1f, 2.75f * Time.deltaTime);
    }

    void HandleAnimationTriggers()
    {
        if (sprite.frameTriggerNow == true)
        {
            if (sprite.currentSprite == Wrench && sprite.imageIndex == 1)
            {

                firePoint = new Vector3(-25, 50, 0) + transform.position;
                GameObject newBullet = Instantiate(bullet, firePoint, Quaternion.identity);

                float randomX = Random.Range(-.2f, .2f);
                float randomY = Random.Range(.7f, .8f);
                newBullet.GetComponent<Bullet>().ChangeSpeed(300);
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(new Vector3((sprite.direction * Mathf.Abs(wadeDistance)/280) + randomX,randomY,0));
                newBullet.GetComponent<Bullet>().enemyBullet = true;
                newBullet.GetComponent<Bullet>().MakeLob(700);
                newBullet.transform.localScale *= 2;

            }

            if(sprite.currentSprite == Shoot && sprite.imageIndex == 5)
            {
                firePoint = new Vector3(-65, 35, 0) + transform.position;
                GameObject newBullet = Instantiate(bullet, firePoint, Quaternion.identity);
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(new Vector3(sprite.direction * 2f, -.3f, 0));
                newBullet.GetComponent<Bullet>().enemyBullet = true;
                newBullet.transform.localScale *= 2;

            }

            if(sprite.currentSprite == Stab && sprite.imageIndex == 3)
            {
                sprite.scale = new Vector2(1.4f, .6f);
                if(GameData.Instance.wadeXYPosition.x + 90 < originalPosition.x)
                {
                    print(wadeDistance + 90);
                    print(originalPosition.x);
                    transform.position += new Vector3(wadeDistance + 90, 0, 0);

                }
            }




        }
        
    }

}
