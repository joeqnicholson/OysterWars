using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxItem : MonoBehaviour
{
    [SerializeField] float distance;
    Vector3 previousCamPos;
    Vector3 InitialPosition;

    void Start()
    {
        InitialPosition = transform.position;
        previousCamPos = GameData.Instance.cameraMachine.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        distance = Mathf.Clamp(distance, 0f, 4f);

        Vector3 desiredAddition = (Camera.main.transform.position - previousCamPos) / (4/distance);

        transform.position += desiredAddition;

        previousCamPos = GameData.Instance.cameraMachine.transform.position;
    }
}
