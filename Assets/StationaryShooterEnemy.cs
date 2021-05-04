using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StationaryState
{
    Active, NotActive
}

public class StationaryShooterEnemy : Enemy
{
    [SerializeField] private SpriteAnimation NotActive;
    [SerializeField] private SpriteAnimation AimUp;
    [SerializeField] private SpriteAnimation AimRight;
    [SerializeField] private SpriteAnimation AimLeft;
    [SerializeField] private SpriteAnimation AimRightUp;
    [SerializeField] private SpriteAnimation AimLeftUp;
    [SerializeField] private SpriteAnimation AimRightDown;
    [SerializeField] private SpriteAnimation AimLeftDown;
    [SerializeField] private SpriteAnimation AimDown;
    [SerializeField] private SpriteAnimationController sprite;
    [SerializeField] private Vector2 UpVector;
    [SerializeField] private Vector2 RightUpVector;
    [SerializeField] private Vector2 RightVector;
    [SerializeField] private Vector2 RightDownVector;
    [SerializeField] private Vector2 DownVector;
    [SerializeField] private bool sixteenDirections;
    [SerializeField] private Transform shootTransform;
    private Vector3 firePoint;
    private StationaryState CurrentStationaryState;
    [SerializeField] private GameObject currentBullet;
    private float shotTimer;
    [SerializeField] private float shotTime;
    [SerializeField] private Vector3 ActiveDistance;
    


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CurrentStationaryState = StationaryState.NotActive;
        sprite = GetComponent<SpriteAnimationController>();
        shootTransform = Instantiate(new GameObject()).transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentStationaryState)
        {
            case StationaryState.NotActive:
                {
                    if(Mathf.Abs(GameData.Instance.wadePosition.x - transform.position.x) < ActiveDistance.x &&
                        Mathf.Abs(GameData.Instance.wadePosition.y - transform.position.y) < ActiveDistance.y
                        )
                    {
                        
                        CurrentStationaryState = StationaryState.Active;
                    }
                    canGetHit = false;
                    
                    sprite.Play(NotActive);
                    break;
                }
            case StationaryState.Active:
                {
                    canGetHit = true;
                    switch (WadeSignedAngle())
                    {
                        case 0:
                            {
                                sprite.Play(AimLeft);
                                firePoint = new Vector2(-RightVector.x, RightVector.y);


                                break;
                            }
                        case -45:
                            {
                                sprite.Play(AimLeftUp);
                                firePoint = new Vector2(-RightUpVector.x,RightUpVector.y);

                                break;
                            }
                        case -90:
                            {
                                sprite.Play(AimUp);
                                firePoint = UpVector;

                                break;
                            }
                        case -135:
                            {
                                sprite.Play(AimRightUp);
                                firePoint = RightUpVector;
                                break;
                            }
                        case -180:
                            {
                                sprite.Play(AimRight);
                                firePoint = RightVector;

                                break;
                            }
                        case 135:
                            {
                                sprite.Play(AimRightDown);
                                firePoint = RightDownVector;

                                break;

                            }
                        case 90:
                            {
                                sprite.Play(AimDown);
                                firePoint = DownVector;

                                break;
                            }
                        case 45:
                            {
                                sprite.Play(AimLeftDown);
                                firePoint = new Vector2(-RightDownVector.x, RightDownVector.y);

                                break;
                            }
                        case 180:
                            {
                                sprite.Play(AimRight);
                                firePoint = RightVector;

                                break;
                            }
                    }

                    shootTransform.position = transform.position + firePoint;
                    float shootDirectionAngle = GameData.Instance.RegulatedWadeSignedAngle(transform.position, 1, sixteenDirections ? MathHelper.SignedAngles22 : MathHelper.SignedAngles45);

                    shootTransform.rotation = Quaternion.Euler(0, 0, shootDirectionAngle);
                    Vector3 bulletDirection = -shootTransform.right;

                    shotTimer += Time.deltaTime;

                    if (shotTimer > shotTime)
                    {
                        Bullet newBullet = Instantiate(currentBullet, shootTransform.position, Quaternion.identity).GetComponent<Bullet>();
                        newBullet.GetComponent<Bullet>().ChangeMoveDirection(bulletDirection);
                        newBullet.GetComponent<Bullet>().enemyBullet = true;
                        shotTimer = 0;
                    }


                    break;
                }
        }

        
    }
}
