using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    public WadeInputs inputs;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(0 ,inputs.moveInput.y, 0) * 15 * Time.deltaTime;
    }
}
