using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Implementation
{

public enum WallJumpType
{
	Normal ,
	Toward ,
	Away
}

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Wall Jump")]
public class WallJump : CharacterAbility
{
	#region Events	

	public delegate void WallJumpDelegate( Vector2 direction );

	public event WallJumpDelegate OnWallJumpPerformed;
	
	#endregion
	
	[SerializeField] float initialVelocity = 12;
	
	[SerializeField] [Range_NoSlider(true)] float towardAngle = 55f;
	[SerializeField] [Range_NoSlider(true)] float normalAngle = 45f;
	[SerializeField] [Range_NoSlider(true)] float awayAngle = 25f;

	


	//VerticalMovement verticalMovement = null;

	protected override void Awake()
	{
		base.Awake();

		//verticalMovement = GetComponent<VerticalMovement>();
	}


	Vector2 GetWallJumpVector( bool wallJumpToTheRight , WallJumpType wallJumpType )
	{
		Vector2 output = Vector2.zero;
		float angle = 0;

		switch( wallJumpType )
		{
			case WallJumpType.Normal:
				angle = normalAngle;
				break;
			case WallJumpType.Toward:
				angle = towardAngle;
				break;
			case WallJumpType.Away:
				angle = awayAngle;
				break;
		}

		output.x = Mathf.Cos( Mathf.Deg2Rad * angle );
		if(!wallJumpToTheRight)
			output.x = -output.x;

		output.y = Mathf.Sin( Mathf.Deg2Rad * angle );

		return output;
	}

	void Setup()
	{	

		bool wallJumpToTheRight = !characterController2D.IsFacingRight;
		Vector2 targetWallJumpDirection = Vector2.zero;
		


		if( characterBrain.CharacterActions.left )
		{
			targetWallJumpDirection = GetWallJumpVector( wallJumpToTheRight , wallJumpToTheRight ? WallJumpType.Toward : WallJumpType.Away);
			if(!wallJumpToTheRight)
				characterController2D.LookToTheOppositeSide();

		}
		else if( characterBrain.CharacterActions.right )
		{ 				
			targetWallJumpDirection = GetWallJumpVector( wallJumpToTheRight , wallJumpToTheRight ? WallJumpType.Away : WallJumpType.Toward);
			if(wallJumpToTheRight)
				characterController2D.LookToTheOppositeSide();
		}
		else
		{
			targetWallJumpDirection = GetWallJumpVector( wallJumpToTheRight , WallJumpType.Normal );
			characterController2D.LookToTheOppositeSide();
		}
		
		
		characterController2D.SetVelocity( targetWallJumpDirection.normalized * initialVelocity );


		if( OnWallJumpPerformed != null )
			OnWallJumpPerformed( targetWallJumpDirection );
	}

	
	public override void Process( float dt )
	{			
		if( !movementController.isCurrentlyOnState( MovementState.WallSlide ) )
			return;
		
		if( characterBrain.CharacterActions.jumpPressed )
		{
			Setup();			

			movementController.SetState( MovementState.Normal );
		}		

		
	}

	public override string GetInfo()
	{ 
		return "It allows the character to perform a wall jump, only if the current state is \"WallSlide\"."; 
	}

	
}

}
