using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxItem : MonoBehaviour
{
    [SerializeField] float distance;

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 shake = GameData.Instance.cameraMachine.shakeVector;

        Vector3 desiredDirection =

            (GameData.Instance.cameraMachine.Target -

            GameData.Instance.cameraMachine.transform.position)
            ;

        transform.position = Vector3.Lerp(transform.position + shake, transform.position + shake + desiredDirection / distance, 4 * Time.deltaTime );


    }
}
