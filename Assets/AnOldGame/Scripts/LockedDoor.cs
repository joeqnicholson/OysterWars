using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private SpriteAnimation Closed;
    [SerializeField] private SpriteAnimation OpenLeft;
    [SerializeField] private SpriteAnimation OpenRight;
    private SpriteAnimation Open;
    private BoxCollider2D collision;
    SpriteAnimationController sprite;
    private bool open;
    [SerializeField] private Item Key;
    [SerializeField] private int number;


    // Start is called before the first frame update
    void Start()
    {
        Open = OpenLeft;
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
            if(sprite.stopped)
            {
                collision.enabled = false;
            }
            

        }
    }

    public void Unlock(WadeInventory inventory)
    {
       
        if(inventory.TakeNeededItem(Key, 1))
        {
            if(GameData.Instance.wadePosition.x < transform.position.x) { Open = OpenRight; } else { Open = OpenLeft; }

            open = true;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<AudioSource>().Play();
            FindObjectOfType<DungeonManager>().SaveDoor(number);
            
        }

        
    }

    public void SetToOpen()
    {
        open = true;
        print("now opening door");
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void SetNumber(int newNumber)
    {
        number = newNumber;
    }


}
