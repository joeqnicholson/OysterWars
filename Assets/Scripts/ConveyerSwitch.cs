using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerSwitch : MonoBehaviour
{
    private SpriteAnimationController sprite;
    [SerializeField] SpriteAnimation Left;
    [SerializeField] SpriteAnimation Right;
    private bool right;

    
    
    void Start()
    {
        right = true;
        sprite = GetComponent<SpriteAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (right)
        {
            sprite.Play(Right);
        }
        else
        {
            sprite.Play(Left);
        }
    }

    public void SwitchDirection()
    {
        DungeonOneManager.Instance.SwitchConveyerDirection();
    }

    public void SetRight()
    {
        if (right) { right = false; } else { right = true; }
    }
}
