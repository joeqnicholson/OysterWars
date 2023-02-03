using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxItem : MonoBehaviour
{
    [SerializeField] float distance;
    Vector3 previousCamPos;
    Vector3 InitialPosition;
    Transform emptyFollow;

    void Start()
    {
        InitialPosition = transform.position;
        previousCamPos = GameData.Instance.cameraMachine.transform.position;
        emptyFollow = Instantiate(new GameObject("EmptyFollow"), transform.position, transform.rotation).transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        distance = Mathf.Clamp(distance, 0f, 4f);

        Vector3 desiredAddition = (Camera.main.transform.position - previousCamPos) / (4/distance);

        emptyFollow.position += desiredAddition;
        
        
        previousCamPos = Camera.main.transform.position;
        transform.position = Vector3Int.RoundToInt(emptyFollow.position);

    }

    void FixedUpdate()
    {
        // transform.position = Vector3Int.RoundToInt(emptyFollow.position);
    }
}
