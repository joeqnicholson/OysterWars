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


    protected override void Start()
    {
        
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
        print(WadeDistanceVector());
        wadeDistance = Vector3.Distance(transform.position, GameData.Instance.wadePosition);
        stateTimer += Time.deltaTime;
        switch (CurrentState)
        {
            case FactoryForemanState.Idle:
                {
                    sprite.Play(Idle);
                    if (stateTimer > idleTime)
                    {
                        if(Mathf.Abs(WadeDistanceVector().x) > 110)
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
                        RaycastHit2D hitInfo = Physics2D.BoxCast(transform.position + new Vector3(sprite.direction * 31, 41), new Vector2(140,40),0, Vector3.zero);
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
                        transform.position = originalPosition;
                        TransitionToState(FactoryForemanState.Idle);
                    }

                    
                    break;
                }

        }
        HandleAnimationTriggers();
    }

    void HandleAnimationTriggers()
    {
        if (hasTriggeredAnimation == false)
        {
            if (sprite.currentSprite == Wrench && sprite.imageIndex == 1)
            {
                print("Throw Wrench");
                firePoint = new Vector3(-25, 50, 0) + transform.position;
                GameObject newBullet = Instantiate(bullet, firePoint, Quaternion.identity);

                float randomX = Random.Range(-.2f, .2f);
                float randomY = Random.Range(.7f, .8f);
                newBullet.GetComponent<Bullet>().ChangeSpeed(300);
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(new Vector3((sprite.direction * wadeDistance/280) + randomX,randomY,0));
                newBullet.GetComponent<Bullet>().enemyBullet = true;
                newBullet.GetComponent<Bullet>().MakeLob(700);
                newBullet.transform.localScale *= 2;

                hasTriggeredAnimation = true;
            }

            if(sprite.currentSprite == Shoot && sprite.imageIndex == 5)
            {
                firePoint = new Vector3(-65, 35, 0) + transform.position;
                GameObject newBullet = Instantiate(bullet, firePoint, Quaternion.identity);
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(new Vector3(sprite.direction * 2f, -.3f, 0));
                newBullet.GetComponent<Bullet>().enemyBullet = true;
                newBullet.transform.localScale *= 2;
                print("ShootBullet");
                hasTriggeredAnimation = true;
            }




        }
        
    }

}
