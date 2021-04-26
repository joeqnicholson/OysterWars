using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationController : MonoBehaviour
{
    public float direction = 1;
    public Vector2 scale = Vector2.one;
    [SerializeField] public int imageIndex = 0;
    [SerializeField] public Sprite currentImage;
    [SerializeField] float frameTimer;
    [SerializeField] private bool stopped = false;
    [SerializeField] public SpriteAnimation currentSprite;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(scale.x * direction, scale.y, 1);
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
        if (currentSprite)
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

    public void PlayNothing()
    {
        currentSprite = null;
        spriteRenderer.sprite = null;
    }

    public void Flip()
    {
        if(direction == -1) { direction = 1; }
        else if(direction == 1) { direction = -1; }
    }
}
