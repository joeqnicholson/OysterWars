using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateLevel : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject collider;
    void OnEnable()
    {
        GetAllTiles();
    }

    public void GetAllTiles()
    {
        var bounds = tilemap.cellBounds;
        for(int x = bounds.min.x; x < bounds.max.x; x++)
        {
             for(int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var cellPosition = new Vector3Int(x, y, 0);
                var sprite = tilemap.GetSprite(cellPosition);
                var tile = tilemap.GetTile(cellPosition);
                if (tile == null && sprite == null)
                {
                    continue;
                }
                else
                {
                    Instantiate(collider, cellPosition * 16, Quaternion.identity);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
