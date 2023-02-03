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
    public List<EnemySpawn> spawns = new List<EnemySpawn>();
    


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

        List<Collider2D> hitInfos = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        GetComponent<Collider2D>().OverlapCollider(contactFilter, hitInfos);
        for (int i = 0; i < hitInfos.Count; i++)
        {
            EnemySpawn spawn = hitInfos[i].GetComponent<EnemySpawn>();
            if (spawn)
            {
                spawn.GetComponent<BoxCollider2D>().enabled = false;
                spawn.cameraBox = this;
                spawn.GetComponent<SpriteRenderer>().enabled = false;
                spawns.Add(spawn);

            }

            Enemy enemy = hitInfos[i].GetComponent<Enemy>();
            if (enemy)
            {
                enemy.cameraBox = this;
            }
            
        }
        GetComponent<Collider2D>().enabled = false;



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
        foreach(EnemySpawn spawn in spawns)
        {
            spawn.Spawn();
        }
    }

    public void WadeLeaves()
    {
        isWadesBox = false;
        foreach (EnemySpawn spawn in spawns)
        {
            spawn.UnSpawn();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.forward * 10, transform.localScale);
    }

}
