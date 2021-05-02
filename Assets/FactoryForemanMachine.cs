using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactoryForemanState
{
    Idle, Shoot, Dash, Wrench
}

public class FactoryForemanMachine : Enemy
{
    [SerializeField] SpriteAnimation Idle;
    [SerializeField] SpriteAnimation Shoot;
    [SerializeField] SpriteAnimation Dash;
    [SerializeField] SpriteAnimation Wrench;
    private SpriteAnimationController sprite;
    private FactoryForemanState CurrentState;
    private bool hasTriggeredAnimation;
    private float idleTime;
    private float stateTimer;
    [SerializeField] GameObject bullet;
    private Vector3 firePoint;
    private int wrenchThrows;


    protected override void Start()
    {
        
        idleTime = Random.Range(.5f, 2f);
        TransitionToState(FactoryForemanState.Idle);
        base.Start();
        sprite = GetComponent<SpriteAnimationController>();
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
        stateTimer += Time.deltaTime;
        switch (CurrentState)
        {
            case FactoryForemanState.Idle:
                {
                    sprite.Play(Idle);
                    if (stateTimer > idleTime)
                    {
                        int stateNumber = Random.Range(1, 3);
                        if(stateNumber == 1)
                        {
                            TransitionToState(FactoryForemanState.Wrench);
                            wrenchThrows = 4;
                        }
                        else if(stateNumber == 2)
                        {
                            TransitionToState(FactoryForemanState.Shoot);
                        }
                        //else if(stateNumber == 2)
                        //{
                        //    wrenchThrows = 3;
                        //    TransitionToState(FactoryForemanState.Wrench);
                        //}
                    }
                    break;
                }
            case FactoryForemanState.Shoot:
                {
                    sprite.Play(Shoot);
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
            case FactoryForemanState.Dash:
                {
                    sprite.Play(Dash);
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
                firePoint = new Vector3(25, 50, 0) + transform.position;
                GameObject newBullet = Instantiate(bullet, firePoint, Quaternion.identity);

                float randomX = Random.Range(.1f, 1f);
                float randomY = Random.Range(.7f, 1f);

                newBullet.GetComponent<Bullet>().ChangeMoveDirection(new Vector3(randomX,randomY,0));
                newBullet.GetComponent<Bullet>().enemyBullet = true;
                newBullet.GetComponent<Bullet>().MakeLob();

                hasTriggeredAnimation = true;
            }

            if(sprite.currentSprite == Shoot && sprite.imageIndex == 5)
            {
                firePoint = new Vector3(65, 35, 0) + transform.position;
                GameObject newBullet = Instantiate(bullet, firePoint, Quaternion.identity);
                newBullet.GetComponent<Bullet>().ChangeMoveDirection(new Vector3(1, -.20f, 0));
                newBullet.GetComponent<Bullet>().enemyBullet = true;
                print("ShootBullet");
                hasTriggeredAnimation = true;
            }

            if (sprite.currentSprite == Dash && sprite.imageIndex == 1)
            {
                print("Dash");
            }


        }
        
    }

}
