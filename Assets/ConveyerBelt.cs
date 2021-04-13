using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConveyerBelt : MonoBehaviour
{
    private bool right;
    [SerializeField] private SpriteAnimation ConveyerRight;
    [SerializeField] private SpriteAnimation ConveyerLeft;
    SpriteAnimationController sprite;
    private float direction;
    private float speed = 100;

    private void Start()
    {
        sprite = GetComponent<SpriteAnimationController>();
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U)) { SwitchDirection(); }

        if (right)
        {
            sprite.Play(ConveyerRight);
            direction = 1;
        }
        else
        {
            sprite.Play(ConveyerLeft);
            direction = -1;
        }
    }

    public float Speed()
    {
        return direction * speed;
    }

    public void SwitchDirection()
    {
        if (right) { right = false; } else { right = true; }
    }

    
}
