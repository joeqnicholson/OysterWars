using UnityEngine;
using Lightbug.Kinematic2D.Core;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Wall Alignment")]
public class WallAlignment : CharacterAbility
{
	#region Events		

	public event CharacterAbilityEvent OnWallAlignmentPerformed;	
	
	#endregion


	[SerializeField]
	LayerMask wallLayerMask = 0;

	HitInfo hitInfo = new HitInfo();
     
	public override void Process(float dt)
	{
		
		if( !movementController.isCurrentlyOnState( MovementState.Normal ) )
			return;		
		
		if( !characterController2D.IsGrounded )
			return;	

		if( characterController2D.verticalAlignmentSettings.mode != VerticalAlignmentMode.World )
			return;
			
		
		CardinalCollisionType cardinalCollisionType = 
		characterController2D.IsFacingRight ? 
		CardinalCollisionType.Right : 
		CardinalCollisionType.Left;


		float skin = CharacterConstants.SkinWidth;
		
		hitInfo = characterController2D.CharacterCollisions.CardinalCollision( 
			characterController2D.Position ,
			cardinalCollisionType , 
			skin , 
			skin , 
			wallLayerMask
		);

		if( hitInfo.hit )
		{			 
			float wallSignedAngle = Vector3.SignedAngle( characterController2D.Up , hitInfo.normal , Vector3.forward );
			float wallAngle = Mathf.Abs( wallSignedAngle );
			
			if(Utilities.CustomUtilities.isCloseTo( wallAngle , 90f , 0.1f ) )
			{
                characterController2D.Teleport( hitInfo.point , Quaternion.LookRotation( Vector3.forward , hitInfo.normal ) );
			
				if( OnWallAlignmentPerformed != null )
					OnWallAlignmentPerformed();
			}	
			
		}
	}

	
	
	
}

}