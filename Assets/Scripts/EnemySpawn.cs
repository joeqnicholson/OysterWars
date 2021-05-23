using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject spawnPrefab;
    private GameObject instantiatedSpawn;
    public CameraBox cameraBox;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        instantiatedSpawn = Instantiate(spawnPrefab, transform.position, transform.rotation);
        Enemy enemy = instantiatedSpawn.GetComponent<Enemy>();
        if (enemy) { enemy.cameraBox = cameraBox; }


        instantiatedSpawn.transform.localScale = transform.localScale;
    }

    public void UnSpawn()
    {
        Destroy(instantiatedSpawn);
    }

}
