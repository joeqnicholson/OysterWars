using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RounderTest : MonoBehaviour
{
    public int x = 0;
    public int xUnit = 0;

    public int xMod;

    public int unit = 12;

    void Update()
    {
        xUnit = MathHelper.RoundToSmallest(x,unit);
        xMod = x % unit;

    }
}
