using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper : MonoBehaviour
{
    public static float[] Angles45 = new float[] { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f, 360f };
    public static float[] SignedAngles45 = new float[] { 0f, 45f, 90f, 135f, 180f, -45f, -90f, -135f, -180f };
    public static float[] SignedAngles22 = new float[] { 0f, 22.5f, 45f, 67.5f, 90f, 112.5f, 135f, 157.5f, 180f, -22.5f, -45f, -67.5f, -90f, -112.5f, -135f, -157.5f, -180f };
    public static float[] Angles22 = new float[] { 0f, 22.5f, 45f, 67.5f, 90f, 112.5f, 135f, 157.5f, 180f, 202.5f, 225f, 247.5f, 270f, 292.5f, 315f, 337.5f, 360f };

    public static float RegulateAngle(float initial, float[] values)
    {
        
        float x = 0;

        foreach(float value in values)
        {
            if(Mathf.Abs(initial - value) < Mathf.Abs(initial - x))
            {
                x = value;
            }
        }
        return x;
    }

    public static float Approach(float val, float target, float maxMove)
    {
        return val > target ? Mathf.Max(val - maxMove, target) : Mathf.Min(val + maxMove, target);
    }

    public static float AbsDifference(float numberOne, float numberTwo)
    {
        return (Mathf.Abs(Mathf.Abs(numberOne) - Mathf.Abs(numberTwo)));
    }


    
    public static float BalancedAngle(float i)
    {
        if(i==0 && i == Mathf.Infinity){return 0;}

        float sign = Mathf.Sign(i);

        if(Mathf.Abs(i) > 90)
        {
            return (90 - (Mathf.Abs(i)-90)) * sign;
        }
        else
        {
            return i;
        }
    }


    
}
