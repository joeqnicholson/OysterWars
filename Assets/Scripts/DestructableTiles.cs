using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructableTiles : MonoBehaviour
{

    public Tilemap dtilemap;
    // Start is called before the first frame update
    void Start()
    {
        dtilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void DestroyTile(Vector3 hitPosition, Vector3 normal)
    {
        float x = hitPosition.x - 0.01f + normal.x;
        float y = hitPosition.y - 0.01f + normal.y;
        dtilemap.SetTile(dtilemap.WorldToCell(new Vector3(x, y, 0)), null);
    }

}
