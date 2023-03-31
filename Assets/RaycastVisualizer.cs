using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastVisualizer : MonoBehaviour
{
    Colliders colliders;
    public Transform sq1;
    public Transform sq2;
    void Start()
    {
        colliders = FindObjectOfType<Colliders>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray.CastTo(sq1.position, sq2.position, colliders);
    }
}
