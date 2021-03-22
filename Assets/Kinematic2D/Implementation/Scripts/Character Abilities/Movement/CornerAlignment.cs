using UnityEngine;
using Lightbug.Kinematic2D.Core;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

public struct CornerAlignmentResult
{
	public bool success;
	public Vector3 normal;
	public Quaternion normalRotation;
	public Vector3 point;
	
	public void Reset()
	{
		success = false;
		normalRotation = Quaternion.identity;
		point = Vector3.zero;
	}
}


[AddComponentMenu("Kinematic2D/Implementation/Abilities/Corner Alignment")]
public class CornerAlignment : CharacterAbility
{
	const float GapDistance = 0.05f;
	#region Events	

	public event CharacterAbilityEvent OnCornerAlignmentPerformed;
	
	#endregion
     
	[Tooltip("Layermask used by this ability.")]
	[SerializeField]
	LayerMask layerMask = 0;

	[Tooltip("Length of the ray fired by the raycast method.")]
	[Range(0.01f,10f)]
	[SerializeField] 
	float cornerDetectionDistance = 1;
	

	public override void Process(float dt)
	{
		if( !movementController.isCurrentlyOnState( MovementState.Normal ) )
			return;		
		
		if( !characterController2D.IsGrounded && characterController2D.WasGrounded )
		{
			if( characterController2D.Velocity.y <= 0 )
				CornerAlign();
		}

	
	}

	
	void CornerAlign()
	{		
		
		CornerAlignmentResult result = CornerAlignCollisions( 
			characterController2D.Velocity.x > 0 , 
			cornerDetectionDistance ,
			layerMask
		);

		if( result.success )
		{
			characterController2D.Teleport( result.point + result.normal * GapDistance , result.normalRotation );

			// characterController2D.ResetVelocity();

			if( OnCornerAlignmentPerformed != null )
				OnCornerAlignmentPerformed();
		}
			
		
	
	}

	/// <summary>
	/// Performs the collision detection method used in the "Corner Alignment" feature.
	/// </summary>
	CornerAlignmentResult CornerAlignCollisions( bool positiveDirection , float cornerDetectionDistance , LayerMask layerMask )
	{
		CornerAlignmentResult result = new CornerAlignmentResult();
		result.Reset();

				
		if( cornerDetectionDistance < 0 )
			return result;
		
		float castDistance = CharacterConstants.SkinWidth + cornerDetectionDistance;		
		
		float directionSign = positiveDirection ? -1 : 1;
		Vector2 rayOrigin = characterController2D.Position + 
		directionSign * characterController2D.Right * characterBody.VerticalCollisionArea/2 - 
		characterController2D.Up * CharacterConstants.SkinWidth;

		Vector2 castDisplacement = directionSign * characterController2D.Right * castDistance;

		HitInfo hitInfo;
		characterController2D.PhysicsComponent.Raycast(
			out hitInfo , 
			rayOrigin , 
			castDisplacement ,
			layerMask
		);

		if( hitInfo.hit )
		{
			result.point = hitInfo.point;
			result.normal = hitInfo.normal;
			result.normalRotation = Quaternion.LookRotation( transform.forward , hitInfo.normal );
			result.success = true;
		}	
			
		return result;
	}

	public override string GetInfo()
	{ 
		return "Allows the character to align itself with the outline of the ground [It requires an extra raycast]."; 
	}

	
	
}

}