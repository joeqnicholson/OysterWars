using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WadeSprite : SpriteAnimationController
{
    
    public SpriteAnimation Idle;
    public SpriteAnimation IdleShoot;
    public SpriteAnimation RunRegular;
    public SpriteAnimation RunAimForward;
    public SpriteAnimation RunAimUp;
    public SpriteAnimation RunAimDown;
    public SpriteAnimation Crouch;
    public SpriteAnimation AimUp;
    public SpriteAnimation JumpRegular;
    public SpriteAnimation JumpAimDDown;
    public SpriteAnimation JumpAimDown;
    public SpriteAnimation JumpAimDUp;
    public SpriteAnimation JumpAimUp;
    public SpriteAnimation JumpAimForward;
    public SpriteAnimation Hit;
    public SpriteAnimation SmallChest;
    public SpriteAnimation SwingTurn;
    public SpriteAnimation SwingSlow;
    public SpriteAnimation SwingMedium;
    public SpriteAnimation SwingFast;

    public SpriteAnimation WallClimb;
    public SpriteAnimation WallSlide;

    private void Start()
    {
        currentSprite = Idle;
        direction = Mathf.Sign(transform.localScale.x);
    }

}
