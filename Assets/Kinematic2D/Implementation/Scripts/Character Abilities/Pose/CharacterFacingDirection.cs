using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

/// <summary>
/// The facing direction actions used when the character is sliding down a slope.
/// </summary>
public enum SlidingFacingDirection 
{
	Free ,
	FaceTheSlidingDirection ,
	FaceTheOppositeSlidingDirection ,
	MantainInitialFacingDirection
}



public class CharacterFacingDirection : CharacterAbility
{

	[SerializeField] bool startFacingRight = true;
	
	[Tooltip("If this reference is not null the character will always face (Horizontally) towards the reference position (ignoring the actions).")]
	[SerializeField] Transform targetReference = null;
	
	[SerializeField] SlidingFacingDirection slidingFacingDirection = SlidingFacingDirection.FaceTheSlidingDirection;


	protected override void Awake()
	{
		base.Awake();

		if( startFacingRight )
			characterController2D.LookToTheRight();
		else
			characterController2D.LookToTheLeft();
		
	}

	public override void Process(float dt)
     {			
		if( 	!movementController.isCurrentlyOnState( MovementState.Normal ) &&
			!movementController.isCurrentlyOnState( MovementState.JetPack ) )
			return;

		if( targetReference != null )
		{
			LookAtTheTarget();			
		}
		else
		{
			
			if( characterController2D.CurrentState == Core.CharacterMotorState.UnstableGrounded )
			{
				UpdateSlidingFacingDirection();
			}
			else
			{
				UpdateFacingDirection();
			}

		}

     }

	void LookAtTheTarget()
	{
		float signedAngle = Vector3.SignedAngle(transform.up , Vector3.up , Vector3.forward );
		Vector3 delta = targetReference.position - transform.position;
		Vector3 rotatedDelta = Quaternion.AngleAxis( signedAngle , Vector3.forward ) * delta;

		if( rotatedDelta.x > 0 )
		{
			characterController2D.LookToTheRight();
		}
		else
		{
			characterController2D.LookToTheLeft();
		}
	}
	
	/// <summary>
	/// Update the facing direction of the character.
	/// </summary>
	void UpdateFacingDirection()
	{		
			
		if( characterBrain.CharacterActions.right )
		{
			characterController2D.LookToTheRight();
		}
		else if( characterBrain.CharacterActions.left )
		{
			characterController2D.LookToTheLeft();
		}
		
	}

	void UpdateSlidingFacingDirection()
	{
		switch( slidingFacingDirection )
		{
			case SlidingFacingDirection.Free:
				UpdateFacingDirection();
				break;

			case SlidingFacingDirection.FaceTheSlidingDirection:
				
				if( characterController2D.IsOnRightVerticalSlope )
				{
					if( characterController2D.IsFacingRight )
						characterController2D.LookToTheLeft();
				}
				else
				{
					if( !characterController2D.IsFacingRight )
						characterController2D.LookToTheRight();
				}
		
				break;
			
			case SlidingFacingDirection.FaceTheOppositeSlidingDirection:
				
				if( characterController2D.IsOnRightVerticalSlope )
				{
					if( !characterController2D.IsFacingRight )
						characterController2D.LookToTheRight();
				}
				else
				{
					if( characterController2D.IsFacingRight )
						characterController2D.LookToTheLeft();
				}
		
				break;

			case SlidingFacingDirection.MantainInitialFacingDirection:
						
				break;
		}
		
	}

	public override string GetInfo()
	{
		return "It handles the facing direction of the character for the normal and jetPack states.";
	}
}

}
