using UnityEngine;

[CreateAssetMenu( menuName = "Kinematic 2D/Core/Layers Profile" )]
public class CharacterMotorLayersProfile : ScriptableObject
{
    [Tooltip("This LayerMask include all the layers related to collidable obstacles.")]
	public LayerMask obstacles;

	[Tooltip("This LayerMask include all the layers related to dynamic platforms.")]	
	public LayerMask dynamicGround;
	
	[Tooltip("This LayerMask include all the layers related to one way platforms (grounded and not grounded state).")]
	public LayerMask oneWayPlatforms;	
	
	
}
