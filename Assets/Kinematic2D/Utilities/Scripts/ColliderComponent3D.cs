using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An implementation of a ColliderComponent for 3D colliders.
/// </summary>
public abstract class ColliderComponent3D : ColliderComponent
{
    protected new Collider collider = null;

    public PhysicMaterial Material
    {
        get
        {
            return collider.sharedMaterial;
        }
        set
        {
            collider.sharedMaterial = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        PhysicMaterial material = new PhysicMaterial("Frictionless 3D");            
        material.dynamicFriction = 0f;
        material.staticFriction = 0f;
        material.frictionCombine = PhysicMaterialCombine.Minimum;
        material.bounceCombine = PhysicMaterialCombine.Minimum;
        material.bounciness = 0f;

        collider.sharedMaterial = material;
        this.collider.hideFlags = HideFlags.NotEditable;
    }
}
