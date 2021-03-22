using UnityEngine;
using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Implementation
{

/// <summary>
/// A "KinematicPlatform" implementation whose movement and rotation are defined by a single movement or rotation action. Use this component if you want to create
/// platforms with a pendulous nature, or infinite duration actions (for instance, if the platform should rotate forever).
/// </summary>
public class ActionBasedPlatform : KinematicPlatform
{
    [SerializeField]
    protected MovementAction movementAction = new MovementAction();

    [SerializeField]
    protected RotationAction rotationAction = new RotationAction();

    
    protected override void UpdateBehaviour( ref Vector3 position , ref Quaternion rotation , float dt )
    {
        movementAction.Tick( dt , ref position );
        rotationAction.Tick( dt , ref position , ref rotation );
    }

    

}

}
