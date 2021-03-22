using UnityEngine;
using Lightbug.Kinematic2D.Core;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

	

public enum WallSlideMode
{
     Constant ,
     Accelerated
}

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Wall Slide")]
public class WallSlide : CharacterAbility
{
	#region Events	

	public event CharacterAbilityEvent OnWallSlideEnter;
	public event CharacterAbilityEvent OnWallSlideExit;
	
	#endregion
	
	[Header("Tag")]

	[SerializeField]
	[Tooltip("Should the algorithm compare the wall with a given tag?. If this is false the character will ignore the wallSlideTag (considering every valid surface).")]
	bool useWallSlideTag = true;

	[Tooltip("The tag that the objects must have for the character to interact with (only for useWallSlideTag = true). If this string is empty " + 
	"the character will ignore the wallSlideTag (equivalent to useWallSlideTag = false).")]
	[SerializeField]
	string wallSlideTag = "WallSlide";

	[Header("Slide parameters")]

	[Tooltip("Speed for the wall slide ability")]
	[SerializeField] 
	float wallSlideSpeed = 2f;

	[Range_NoSlider(true)]
	[SerializeField]
	float wallSlideAccelerationFactor = 2f;	
		
	[SerializeField] 
	WallSlideMode wallSlideMode = WallSlideMode.Constant;

	[Tooltip("Whether or not the wall jump action will reset the availables jumps in the air. This will require the VerticalMovement ability.")]
	[SerializeField] 
	bool resetAirJumps = true;

	LayerMask layerMask;

	VerticalMovement verticalMovement = null;


	protected override void Awake()
	{
		base.Awake();

		layerMask = characterController2D.layerMaskSettings.profile.obstacles;

		verticalMovement = GetComponent<VerticalMovement>();
	}
	
	public override void Process(float dt)
	{
		if( movementController.isCurrentlyOnState( MovementState.Normal ) )
		{
			if( CheckWallSliding() &&  !characterBrain.CharacterActions.down )
			{
				movementController.SetState( MovementState.WallSlide );
				characterController2D.ResetVelocity();

				if( verticalMovement != null && resetAirJumps )
					verticalMovement.ResetNotGroundedJump();

				if( OnWallSlideEnter != null )
					OnWallSlideEnter();
			}
		}
		else if( movementController.isCurrentlyOnState( MovementState.WallSlide ) )
		{			
			ProcessWallSlide( dt );

			if( characterController2D.IsGrounded || !CheckWallSliding() || characterBrain.CharacterActions.down )
			{
				movementController.SetState( MovementState.Normal );

				if( OnWallSlideExit != null )
					OnWallSlideExit();

			}
		}

		
	}



	/// <summary>
	/// Check if the wall slide ability conditions are met (a wall in front of the character with the corresponding "Wall Sliding" Tag).
	/// </summary>
	public bool CheckWallSliding()
	{
		if(characterController2D.IsGrounded)
			return false;

		if(characterBrain.CharacterActions.down)
			return false;
		
		if( characterController2D.Velocity.y > 0 )
			return false;

		HitInfo info = new HitInfo();

		CardinalCollisionType cardinalCollisionType = 
		characterController2D.IsFacingRight ? 
		CardinalCollisionType.Right : 
		CardinalCollisionType.Left;

		float skin = CharacterConstants.SkinWidth;
		
		info = characterController2D.CharacterCollisions.CardinalCollision( 
			characterBody.RigidbodyComponent.Position , 
			cardinalCollisionType , 
			skin , 
			skin , 
			layerMask 
		);

		if(!info.hit)
			return false;

		float wallSignedAngle = Vector3.SignedAngle(transform.up , info.normal , transform.forward );
		float wallAngle = Mathf.Abs( wallSignedAngle );

		// if ( !info.gameObject.CompareTag(wallSlideTag) || !Utilities.isCloseTo( wallAngle , 90 , 0.1f ) )
		// 	return false;
		
		if( !wallSlideTag.IsNullOrEmpty() )
		{
			if( useWallSlideTag && !info.gameObject.CompareTag(wallSlideTag) )
				return false;
		}

		if( !Utilities.CustomUtilities.isCloseTo( wallAngle , 90 , 0.1f ) )
			return false;
		
		return true;
		
		
	}
	
	void ProcessWallSlide( float dt )
	{
		switch(wallSlideMode)
		{
			case WallSlideMode.Constant:
				characterController2D.SetVelocityY( - wallSlideSpeed );

			break;

			case WallSlideMode.Accelerated:

				characterController2D.SetVelocityY( 
                         Mathf.MoveTowards( 
                              characterController2D.Velocity.y , 
                              - wallSlideSpeed , 
                              wallSlideAccelerationFactor * dt 
                         ) 
                    );

			break;
		}
	}

	public override string GetInfo()
	{ 
		return "With this ability the character can grab onto a wall only if this is falling (negative vertical velocity) " +
		" and the wall is closely at 90 degrees respect to the ground. By doing this the state will change from \"Normal\"to \"WallSlide\""; 
	}
	
}

}