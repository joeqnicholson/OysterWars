using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraBox : AABB
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
    public BoundsInt area;
    public Tilemap tileMap;


    private void Start()
    {
        tileMap = FindObjectOfType<Tilemap>();
        area.position = Vector3Int.RoundToInt(transform.position);
        area.size = Vector3Int.RoundToInt(transform.localScale);
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

        GetColliders();

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

    public void GetColliders()
    {

        float x = transform.position.x - xSize + 4;
        float y = transform.position.y - ySize + 4;
        
        while( y < transform.position.y + ySize)
        {
            while(x < transform.position.x + ySize)
            {
                Vector3 pos = new Vector3(x,y,0);


                Vector3Int tilePos = tileMap.WorldToCell(pos);

                var tile = tileMap.GetTile<Tile>(tilePos);
                GameObject newTile = Instantiate(new GameObject("solid"), tile.transform.position, tile.transform.rotation);
                newTile.AddComponent<Solid>();
                Solid solid = newTile.GetComponent<Solid>();
                solid.size = new Vector2(8,8);
                x += 8;
            }
            x = transform.position.x - xSize + 4;
            y += 8;
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
