using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WadeInventory : MonoBehaviour
{
    [SerializeField] public Item Key;
    [SerializeField] public  Item LobShot;
    [SerializeField] public  Item KeyCard;
    public int keys;

    
    public Hashtable itemCount = new Hashtable();
    

    void Start()
    {
        itemCount.Add(Key, 0);
        itemCount.Add(LobShot, 0);
        itemCount.Add(KeyCard, 0);
    }

    // Update is called once per frame
    void Update()
    {
        keys = (int)itemCount[LobShot];
    }

    public void PickupItem(Item item)
    {
        int currentValue = (int) itemCount[item];
        int nextValue = currentValue += 1;
        itemCount[item] = nextValue;


        Texture2D jimbo;
        
    }

    public bool TakeNeededItem (Item item, int times)
    {
        if(times <= (int)itemCount[item])
        {
            int currentValue = (int)itemCount[item];
            int nextValue = currentValue - times;
            itemCount[item] = nextValue;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool WadeHasItem(Item desiredItem)
    {
        if((int)itemCount[desiredItem] > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

}
