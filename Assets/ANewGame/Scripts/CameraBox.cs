

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBox : MonoBehaviour
{
    private bool isWadesBox;
    private float xSize;
    private float ySize;
    private float wadeX;
    private float wadeY;
    public bool onTrackX;
    public bool onTrackY;
    public bool spawnEnemies = true;
    public LayerMask LayerMask;

    


    private void Start()
    {

        isWadesBox = false;
        GetComponent<MeshRenderer>().enabled = false;
        xSize = transform.localScale.x / 2;
        ySize = transform.localScale.y / 2;
        wadeX = GameData.Instance.wadePosition.x;
        wadeY = GameData.Instance.wadePosition.y;

        if (Mathf.Abs(transform.position.x - wadeX) < xSize && Mathf.Abs(transform.position.y - wadeY) < ySize)
        {
            GameData.Instance.cameraMachine.currentCameraBox = this;
            isWadesBox = true;
        }







        if (!isWadesBox)
        {
            WadeLeaves();
        }
        else
        {
            WadeEnters();
        }

    }

    private void Update()
    {
        wadeX = GameData.Instance.wadePosition.x;
        wadeY = GameData.Instance.wadePosition.y;
        

        if (Mathf.Abs(transform.position.x - wadeX) < xSize && Mathf.Abs(transform.position.y - wadeY) < ySize && !isWadesBox)
        {
            CameraBox CCB = GameData.Instance.cameraMachine.currentCameraBox;

            if (CCB)
            {
                CCB.WadeLeaves();
                GameData.Instance.cameraMachine.currentCameraBox = this;
                WadeEnters();
            }
        }
    }

    public void WadeEnters()
    {
        
        isWadesBox = true;

    }

    public void WadeLeaves()
    {
        isWadesBox = false;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.forward * 10, transform.localScale);
    }

}