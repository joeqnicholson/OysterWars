using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WadeInventory : MonoBehaviour
{
    [SerializeField] private Item Key;
    public int keys;

    
    public Hashtable itemCount = new Hashtable();
    

    void Start()
    {
        itemCount.Add(Key, 0);
    }

    // Update is called once per frame
    void Update()
    {
        keys = (int)itemCount[Key];
    }

    public void PickupItem(Item item)
    {
        int currentValue = (int) itemCount[item];
        int nextValue = currentValue += 1;
        itemCount[item] = nextValue;
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

}
