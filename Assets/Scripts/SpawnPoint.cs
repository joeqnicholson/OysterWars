using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        WadeMachine machine = collision.gameObject.GetComponent<WadeMachine>();
        if (machine)
        {
            machine.ChangeSpawnPoint(transform.GetChild(0).position);
        }
    }


}
