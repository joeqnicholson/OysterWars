using UnityEngine;
using Lightbug.Utilities;

/// <summary>
/// This component represents a box collider in a 3D world.
/// </summary>
public class BoxColliderComponent3D : ColliderComponent3D
{
    BoxCollider boxCollider = null;

    public override Vector3 Size
    {
        get
        {
            return boxCollider.size;
        }
        set
        {
            boxCollider.size = value;
        }
    }

    public override Vector3 BoundsSize
    {
        get
        {
            return boxCollider.bounds.size;
        }
    }

    
    public override Vector3 Offset 
    {
        get
        {
            return boxCollider.center;
        }
        set
        {
            boxCollider.center = value;
        }
    }

    protected override void Awake()
    {
        boxCollider = gameObject.GetOrAddComponent<BoxCollider>(); 
        collider = boxCollider;
        
        base.Awake();

    }

    
}
