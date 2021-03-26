using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] SpriteAnimation ChestClosed;
    [SerializeField] SpriteAnimation ChestOpen;
    [SerializeField] GameObject item;
    private SpriteAnimationController animator;
    public bool open = false;
    public Vector3 itemPosition;


    void Start()
    {
        animator = GetComponent<SpriteAnimationController>();
        itemPosition = item.transform.position + Vector3.up * 20;
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
            RaiseItem();
        }

        

    }

    void RaiseItem()
    {
        item.transform.position = Vector3.Lerp(item.transform.position, itemPosition, 2 * Time.deltaTime);
    }

    
}
