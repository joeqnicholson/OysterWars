using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colliders : MonoBehaviour
{
    public List<Solid> solids = new List<Solid>(0);
    public List<Actor> actors = new List<Actor>(0);
    public List<Trigger> triggers = new List<Trigger>(0);

    void Awake()
    {
        solids = new List<Solid>(FindObjectsOfType<Solid>());
        actors = new List<Actor>(FindObjectsOfType<Actor>());
        triggers = new List<Trigger>(FindObjectsOfType<Trigger>());
    }

    public void UpdateSolids(List<Solid> newSolids)
    {
        foreach(Solid solid in newSolids)
        {
            solid.active = true;
        }

        foreach(Solid solid in solids)
        {
            solid.active = false;
        }

        solids = newSolids;

    }

}
