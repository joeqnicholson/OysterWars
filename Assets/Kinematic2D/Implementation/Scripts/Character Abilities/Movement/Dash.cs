using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Kinematic2D.Implementation
{

public enum DashMode
{ 
     facingDirection , 
     InputDirection 
}

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Dash")]
public class Dash : CharacterAbility
{    
	#region Events		

	public delegate void DashDelegate( Vector2 dashDirection );	

	public event DashDelegate OnDashPerformed;	
	
	#endregion
		
	[Range_NoSlider(true)] [SerializeField] float initialVelocity = 12f;
	[Range_NoSlider(true)] [SerializeField] float duration = 0.4f;
	[SerializeField] AnimationCurve movementCurve = AnimationCurve.Linear( 0,1,1,0 );
	
	[SerializeField] DashMode dashMode = DashMode.facingDirection;
	[Range_NoSlider(true)] [SerializeField] int availableNotGroundedDashes = 1;

	
	int airDashesLeft;
	float dashCursor = 0;
	Vector2 dashDirection = Vector2.right;
	bool isDone = true;

	
	/// <summary>
	/// Setup the velocity vector before performing the dash ability.
	/// </summary>
	void Setup()
	{		
		if(dashMode == DashMode.InputDirection)
		{

			if( characterBrain.CharacterActions.up )
			{
				if( characterBrain.CharacterActions.right)
				{
					dashDirection = new Vector2(1,1).normalized;
				}
				else if( characterBrain.CharacterActions.left )
				{
					dashDirection = new Vector2(-1,1).normalized;
				}
				else
				{
					dashDirection = Vector2.up;
				}
			}
			else if( characterBrain.CharacterActions.down )
			{
				if( characterBrain.CharacterActions.right)
				{
					dashDirection = new Vector2(1,-1).normalized;
				}
				else if( characterBrain.CharacterActions.left )
				{
					dashDirection = new Vector2(-1,-1).normalized;
				}
				else
				{
					dashDirection = Vector2.down;
				}
			}
			else
			{
				if(characterController2D.IsFacingRight)
					dashDirection = Vector2.right;
				else
					dashDirection = Vector2.left;
			}

		}
		else
		{
			if(characterController2D.IsFacingRight)
				dashDirection = Vector2.right;
			else
				dashDirection = Vector2.left;
		}
		
		if( OnDashPerformed != null )
			OnDashPerformed( dashDirection );
					
	}
	
	
	public override void Process( float dt )
	{
		if( movementController.isCurrentlyOnState( MovementState.Normal ) )
		{
			if( !characterController2D.IsGrounded )
			{			
				if( characterBrain.CharacterActions.dashPressed && airDashesLeft != 0 )
				{
					airDashesLeft--;	
					Setup();	
					ResetDash();			
					movementController.SetState( MovementState.Dash );

				}
			}
			else
			{
				airDashesLeft = availableNotGroundedDashes;
				if( characterBrain.CharacterActions.dashPressed )
				{
					Setup();	
					ResetDash();
					movementController.SetState( MovementState.Dash );
				}
			}
			
		}
		else if( movementController.isCurrentlyOnState( MovementState.Dash ) )
		{
			ProcessDash( dt );

			if( isDone )
			{				
				movementController.SetState( MovementState.Normal );
			}

			
		}
		
		

	}

	void ProcessDash( float dt )
	{
		characterController2D.SetVelocity( initialVelocity * movementCurve.Evaluate(dashCursor) * dashDirection );

		float animationDt = dt / duration;
		dashCursor += animationDt; 

		if( dashCursor >= 1 )
          {
			isDone = true;
			dashCursor = 0;
		}
	}

	void ResetDash()
	{		
		characterController2D.ResetVelocity();
		isDone = false;
		dashCursor = 0;
	}

	public override string GetInfo()
	{ 
		return "By default it impulses the character with a given acceleration during a given duration time. " + 
		"This effect is simulated by the animation curve parameter from below."; 
	}
	
	
}

}


