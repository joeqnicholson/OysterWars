using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBox : MonoBehaviour
{

    private float xSize;
    private float ySize;
    private float wadeX;
    private float wadeY;
    public bool onTrackX;
    public bool onTrackY;
    



    private void Start()
    {
        xSize = transform.localScale.x/2;
        ySize = transform.localScale.y/2;
    }

    private void Update()
    {
        wadeX = GameData.Instance.wadePosition.x;
        wadeY = GameData.Instance.wadePosition.y;
        if (Mathf.Abs(transform.position.x - wadeX) < xSize && Mathf.Abs(transform.position.y - wadeY) < ySize)
        {
            GameData.Instance.machine.currentCameraBox = this;
        }
    }
}
