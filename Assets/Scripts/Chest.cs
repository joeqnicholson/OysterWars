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
    private SpriteAnimationController animator;
    public bool open = false;
    public Vector3 itemPosition;
    public Vector3 wadeToPosition;
    [SerializeField] bool bigChest = false;

    void Start()
    {
        animator = GetComponent<SpriteAnimationController>();
        itemSprite = transform.GetChild(1).GetComponent<SpriteAnimationController>();
        itemObject = transform.GetChild(1).gameObject;
        wadeToPosition = transform.position + Vector3.right * 25 * Mathf.Sign(transform.localScale.x);
        itemObject.transform.position = wadeToPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!open)
        {
            animator.Play(ChestClosed);
        }
        else
        {
            animator.Play(ChestOpen);
            transform.GetChild(0).GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, bigChest ? 9:5);
            transform.GetChild(0).GetComponent<BoxCollider2D>().size = new Vector2(25, bigChest? 18:10);
            GetComponent<BoxCollider2D>().enabled = false;

            if (itemSprite.imageIndex == itemSprite.currentSprite.totalFrames - 1)
            {
                GameData.Instance.machine.Inventory.PickupItem(item);
                print("jimbo");

                enabled = false;
            }
        }

    }

    public void OpenChest()
    {
        open = true;
        itemSprite.Play(item.ChestAnimation);
    }

    
}
