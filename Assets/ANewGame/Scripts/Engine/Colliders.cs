using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colliders : MonoBehaviour
{
    public Solid[] solids = new Solid[0];
    public Actor[] actors = new Actor[0];
    public Trigger[] triggers = new Trigger[0];


    void Awake()
    {
        solids = FindObjectsOfType<Solid>();
        actors = FindObjectsOfType<Actor>();
        triggers = FindObjectsOfType<Trigger>();
    }

}
