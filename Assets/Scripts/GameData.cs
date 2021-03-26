using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameData : MonoBehaviour
{
    public static GameData _instance;
    public static GameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameData>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("GameData");
                    _instance = container.AddComponent<GameData>();
                }
            }

            return _instance;
        }
    }

    public Vector3 wadePosition;
    public Vector2 wadeXYPosition;
    public WadeMachine machine;

    private void OnEnable()
    {
        machine = GameObject.Find("Wade").GetComponent<WadeMachine>();
        wadePosition = machine.transform.position;
    }

    private void Update()
    {
        wadeXYPosition = new Vector2(wadeXYPosition.x, wadeXYPosition.y);
        wadePosition = machine.transform.position;
    }

    public float WadeAngle(Vector3 position, float directionInt)
    {
        Vector3 relativeWadePosition = wadePosition - position;

        return Vector3.Angle(Vector3.right * -directionInt, relativeWadePosition);
    }

    public float RegulatedWadeAngle(Vector3 position, float directionInt)
    {
        return MathHelper.RegulateAngle(WadeAngle(position, directionInt), MathHelper.Angles45);
    }

    


}
