using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltsManager : MonoBehaviour
{
    [SerializeField] private GameObject AllTheSwitches;
    [SerializeField] private GameObject AllTheBelts;
    private List<ConveyerBelt> conveyerBelts = new List<ConveyerBelt>();
    private List<ConveyerSwitch> conveyerSwitches = new List<ConveyerSwitch>();

    void Start()
    {
        
    }


    void Update()
    {
        
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
