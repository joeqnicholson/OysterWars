using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] SpriteAnimation ChestClosed;
    [SerializeField] SpriteAnimation ChestOpen;
    [SerializeField] GameObject itemObject;
    private SpriteAnimationController itemSprite;
    [SerializeField] Item item;
    private SpriteAnimationController sprite;
    public bool open = false;
    public Vector3 itemPosition;
    public Vector3 wadeToPosition;
    [SerializeField] bool bigChest = false;
    private bool setAsOpen;
    private WadeMachine machine;
    [SerializeField] private int number;

    void Start()
    {
        sprite = GetComponent<SpriteAnimationController>();
        itemSprite = transform.GetChild(1).GetComponent<SpriteAnimationController>();
        sprite.direction = Mathf.Sign(transform.localScale.x);
        itemObject = transform.GetChild(1).gameObject;
        wadeToPosition = transform.position + Vector3.right * 25 * Mathf.Sign(transform.localScale.x);
        itemObject.transform.position = wadeToPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!open)
        {
            sprite.Play(ChestClosed);
        }
        else
        {
            sprite.Play(ChestOpen);
            transform.GetChild(0).GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, bigChest ? 9:5);
            transform.GetChild(0).GetComponent<BoxCollider2D>().size = new Vector2(25, bigChest? 18:10);
            
            if(machine)
            {
                if (itemSprite.imageIndex == itemSprite.currentSprite.totalFrames - 1)
                {
                    if(machine)
                    {
                        machine.Inventory.PickupItem(item);
                    }
                    enabled = false;
                }
            }
            
        }

    }

    public void OpenChest(WadeMachine wadeMachine)
    {
        machine = wadeMachine;
        open = true;
        GetComponent<AudioSource>().Play();
        itemSprite.Play(item.ChestAnimation);
        GetComponent<BoxCollider2D>().enabled = false;
        FindObjectOfType<DungeonManager>().SaveChest(number);
    }

    public void SetToOpen()
    {
        open = true;
        print("now opening chest");
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void SetNumber(int newNumber)
    {
        number = newNumber;
    }

    
}
