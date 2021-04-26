using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSprite : MonoBehaviour
{
    public Vector2 scale = Vector2.one;
    public float direction;


    [SerializeField] public int imageIndex = 0;
    [SerializeField] public Sprite currentImage;
    [SerializeField] float frameTimer;
    [SerializeField] bool loop = true;
    [SerializeField] private bool stopped = false;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteAnimation currentSprite;
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
    public SpriteAnimationController controller;

    private void Start()
    {
        currentSprite = Idle;
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = Mathf.Sign(transform.localScale.x);
    }

    private void Update()
    {
        transform.localScale = new Vector3(scale.x  * direction, scale.y, 1);
        Animate();
        spriteRenderer.sprite = currentImage;
    }

    public void Play(SpriteAnimation nextAnimation, bool reset = true)
    {
        if (currentSprite != nextAnimation)
        {
            if (reset)
            {
                imageIndex = 0;
                frameTimer = 0;
                stopped = false;
            }
        }
            currentSprite = nextAnimation;
    }

    public void Animate()
    {
        currentImage = currentSprite.Keyframe(imageIndex);

        if (!stopped)
        {
            frameTimer += Time.deltaTime;
        }

        if (frameTimer >= 1 / currentSprite.FPS())
        {
            if (imageIndex == currentSprite.totalFrames - 1)
            {
                if (currentSprite.loop)
                {
                    imageIndex = 0;
                }
                else
                {
                    stopped = true;
                }
            }
            else
            {
                imageIndex += 1;
            }
            frameTimer = 0;
        }
    }
}
