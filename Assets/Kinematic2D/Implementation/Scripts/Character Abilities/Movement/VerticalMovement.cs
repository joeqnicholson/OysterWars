using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Implementation
{

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Vertical Movement")]
public class VerticalMovement : CharacterAbility
{
	

	[SerializeField] 
	VerticalMovementProfile verticalData;


	[Header("Environment")]

	[SerializeField]
	bool isAffectedByMovementAreas = true;

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	#region Events	

	public delegate void NotGroundedJumpDelegate( int airJumpsLeft );
	public delegate void GroundedJumpDelegate();

	public event CharacterAbilityEvent OnJumpPerformed;
	public event CharacterAbilityEvent OnCancelJump;
	public event GroundedJumpDelegate OnGroundedJumpPerformed;
	public event NotGroundedJumpDelegate OnNotGroundedJumpPerformed;
	
	#endregion


	VerticalMovementProfile defaultVerticalData;

	GameObject collidedTrigger = null;

	float gravity = 10f;	


	float jumpSpeed;

	int notGroundedJumpsLeft;

	float jumpTimer = 0;
	bool cancelJump = false;

	bool enableJumpTimer = false;
	

	protected override void Awake()
	{
		base.Awake();

		defaultVerticalData = verticalData;
		

		CalculateGravityParameters();

		notGroundedJumpsLeft = verticalData.availableNotGroundedJumps;

	}
	

	public override void Process( float dt )
	{   	
		if( movementController.isCurrentlyOnState( MovementState.Normal ) )
		{		
		
			ProcessGravity( dt );

			ProcessJump(dt);

			ProcessMovementArea();

		}
		
	}

	public void CalculateGravityParameters()
	{	
		gravity = (2 * verticalData.jumpHeight ) / Mathf.Pow( verticalData.jumpDuration , 2 );
		jumpSpeed = gravity * verticalData.jumpDuration;
	}

	void ProcessGravity( float dt )
	{
		
		if(characterController2D.IsGrounded)
		{			
			characterController2D.SetVelocityY( 0f );
			ResetNotGroundedJump();		
		}
		else
		{	
			float gravity = characterController2D.Velocity.y < 0 ? this.gravity * verticalData.descendingGravityMultiplier : this.gravity;
			characterController2D.AddVelocityY( - gravity * dt );

			if( characterController2D.IsHead )
			{
				
				float minimumVelocity = CharacterConstants.MinimumMovement / dt;
				characterController2D.SetVelocityY( - ( minimumVelocity + 0.001f )  );
			}
		}
	}

	void ProcessJump( float dt )
	{			
		
		if( characterBrain == null )
			return;
		
		if( !verticalData.cancelOnRelease )
		{
			if( characterBrain.CharacterActions.jumpPressed )
			{				
				Jump();
			}

			return;
		}
		

		if( characterBrain.CharacterActions.jumpPressed )
		{
			Jump();

			enableJumpTimer = true;
			jumpTimer = 0;
			cancelJump = false;

			return;
		}

		if( characterController2D.Velocity.y <= 0 )
		{
			jumpTimer = 0;
			enableJumpTimer = false;
			cancelJump = false;
			return;
		}

		if( enableJumpTimer )
		{
			jumpTimer += dt;

			
			if( jumpTimer < verticalData.cancelJumpMinTime )
			{
				if( characterBrain.CharacterActions.jumpReleased )
					cancelJump = true;
				
			} 
			else if( jumpTimer < verticalData.cancelJumpMaxTime )
			{
				if( characterBrain.CharacterActions.jumpReleased )
				{
					jumpTimer = 0;
					enableJumpTimer = false;					
					cancelJump = true;
				}
				
				
				if( cancelJump )
				{
					CancelJump();
					

					cancelJump = false;
					jumpTimer = 0;
					enableJumpTimer = false;
				}

			}
			
			
		}

		
	}


	void CancelJump()
	{
		characterController2D.SetVelocityY( characterController2D.Velocity.y * verticalData.cancelJumpFactor );

		if( OnCancelJump != null )
			OnCancelJump();
	}
	
	

	void Jump()
	{			
		
		if( !characterController2D.IsGrounded )
		{
			if( notGroundedJumpsLeft != 0 )
			{
				characterController2D.ForceNotGroundedState();		
				characterController2D.SetVelocityY( jumpSpeed );
				notGroundedJumpsLeft--;	


				if( OnNotGroundedJumpPerformed != null )
					OnNotGroundedJumpPerformed( notGroundedJumpsLeft );		
			}
		}	
		else
		{
			if( verticalData.jumpOnlyOnStableGround && !characterController2D.IsStable)
			{
				return;
			}

			characterController2D.ForceNotGroundedState();		
			characterController2D.SetVelocityY( jumpSpeed );

			if( OnGroundedJumpPerformed != null )
				OnGroundedJumpPerformed();	
			
		}

		if( OnJumpPerformed != null )
			OnJumpPerformed();
		
	}
	

	/// <summary>
	/// Resets the number of available air jumps to the initial value.
	/// </summary>
	public void ResetNotGroundedJump()
	{
		notGroundedJumpsLeft = verticalData.availableNotGroundedJumps;
	}


	void ProcessMovementArea()
	{
		if(!isAffectedByMovementAreas)
			return;

		if( characterController2D.CollidedTrigger == null )
		{
			if(collidedTrigger != null)
			{
				RevertMovementParameters();
				collidedTrigger = null;
			}
			
			
			return;
		}
		else
		{
			
			if( ( collidedTrigger == null ) || 
				( characterController2D.CollidedTrigger != collidedTrigger ))
			{
				MovementArea movementArea = characterController2D.CollidedTrigger.GetComponent<MovementArea>();
				if( movementArea != null )
				{
					SetMovementParameters( movementArea );
				}

				collidedTrigger = characterController2D.CollidedTrigger;
			}

			
		
		}
	}

	

	

	void SetMovementParameters( MovementArea movementArea )
	{
		CharacterMovementProfile data = movementArea.CharacterMovementData;
		if( data == null  )
			return;
		
		if( data.verticalMovementData != null )
		{
			this.verticalData = data.verticalMovementData;
			CalculateGravityParameters();

			characterController2D.SetVelocityY( characterController2D.Velocity.y * data.verticalMovementData.entrySpeedMultiplier );	
			
		}

		
		
	}

	void RevertMovementParameters()
	{	
		this.verticalData = defaultVerticalData;
		CalculateGravityParameters();
		
	}

	
	public override string GetInfo()
	{ 
		return "It handles the vertical movement of the character in normal state (the vertical movement is affected by " + 
		"gravity and jump velocity).";
	}

	
}

}
