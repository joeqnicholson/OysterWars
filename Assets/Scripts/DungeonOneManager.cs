using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonOneManager : MonoBehaviour
{
    public static DungeonOneManager _instance;
    private List<ConveyerBelt> conveyerBelts = new List<ConveyerBelt>();
    private List<ConveyerSwitch> conveyerSwitches = new List<ConveyerSwitch>();
    [SerializeField] private GameObject AllTheSwitches;
    [SerializeField] private GameObject AllTheBelts;
    

    public static DungeonOneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DungeonOneManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("DungeonOneManager");
                    _instance = container.AddComponent<DungeonOneManager>();
                }
            }

            return _instance;
        }
    }

    private void OnEnable()
    {
        int switchIndex = 0;
        int beltIndex = 0;

        while(switchIndex < AllTheSwitches.transform.childCount)
        {
            conveyerSwitches.Add(AllTheSwitches.transform.GetChild(switchIndex).GetComponent<ConveyerSwitch>());
            switchIndex += 1;
        }

        while (beltIndex < AllTheBelts.transform.childCount)
        {
            conveyerBelts.Add(AllTheBelts.transform.GetChild(beltIndex).GetComponent<ConveyerBelt>());
            beltIndex += 1;
        }

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchConveyerDirection()
    {
        foreach(ConveyerBelt belt in conveyerBelts)
        {
            belt.SwitchDirection();
        }

        foreach (ConveyerSwitch conveyerSwitch in conveyerSwitches)
        {
            conveyerSwitch.SetRight();
        }


    }


}
