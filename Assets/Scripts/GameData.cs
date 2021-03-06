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

    public SpriteAnimation explosion;
    public GameObject particleSpawn;
    public Vector3 wadePosition;
    public Vector2 wadeXYPosition;
    public WadeMachine machine;
    public CameraMachine cameraMachine;
    public MusicManager music;
    private Vector3 cameraPosition;


    private void OnEnable()
    {
        cameraMachine = Camera.main.gameObject.GetComponent<CameraMachine>();
        machine = GameObject.Find("Wade").GetComponent<WadeMachine>();
        music = GameObject.Find("Music").GetComponent<MusicManager>();
        wadePosition = machine.transform.position;
    }

    private void Update()
    {
        cameraPosition = cameraMachine.transform.position;
        wadeXYPosition = new Vector2(wadePosition.x, wadePosition.y);
        wadePosition = machine.transform.position;
    }

    public float WadeAngle(Vector3 position, float directionInt)
    {
        Vector3 relativeWadePosition = wadePosition - position;
        return Vector3.Angle(Vector3.right * -directionInt, relativeWadePosition);
    }

    public float WadeSignedAngle(Vector3 position, float directionInt)
    {
        Vector3 relativeWadePosition = wadePosition - position;
        return Vector2.SignedAngle(Vector3.right * -directionInt, relativeWadePosition);
    }



    public float RegulatedWadeAngle(Vector3 position, float directionInt, float[] typeOfAngles)
    {
        return MathHelper.RegulateAngle(WadeAngle(position, directionInt), typeOfAngles);
    }



    public float RegulatedWadeSignedAngle(Vector3 position, float directionInt, float[] typeOfAngles)
    {
        return MathHelper.RegulateAngle(WadeSignedAngle(position, directionInt), typeOfAngles);
    }


    public bool IsOnScreen(Vector3 position, Vector2 size)
    {
        bool insideLeft = position.x > cameraMachine.leftSide - size.x / 2;
        bool insideRight = position.x < cameraMachine.rightSide + size.x / 2;
        bool insideTop = position.y < cameraMachine.topSide;
        bool insideBottom = position.y > cameraMachine.bottomSide - size.y;


        if(insideBottom && insideTop && insideLeft && insideRight) { return true; }
        


        return false;
    }

    public bool OnRightSide(float xPos)
    {
        return cameraMachine.transform.position.x < xPos;
    }

}
