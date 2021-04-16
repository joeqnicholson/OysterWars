using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private SpriteAnimation Closed;
    [SerializeField] private SpriteAnimation Open;
    private BoxCollider2D collision;
    SpriteAnimationController sprite;
    private bool open;
    [SerializeField] private Item Key;



    // Start is called before the first frame update
    void Start()
    {
        open = false;
        
        collision = transform.GetChild(0).GetComponent< BoxCollider2D>();
        sprite = GetComponent<SpriteAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            sprite.Play(Open);
        }
        else
        {
            sprite.Play(Closed);
        }


        if(sprite.currentSprite == Open)
        {
            if(sprite.imageIndex == 15)
            {
                collision.enabled = false;
            }
        }
    }

    public void Unlock(WadeInventory inventory)
    {
        if(inventory.TakeNeededItem(Key, 1))
        {
            open = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
