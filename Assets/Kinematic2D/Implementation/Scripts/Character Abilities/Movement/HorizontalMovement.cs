using UnityEngine;
using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Implementation
{

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Horizontal Movement")]
public class HorizontalMovement : CharacterAbility
{
	#region Events	

	public event CharacterAbilityEvent OnEnterMovementArea;
	public event CharacterAbilityEvent OnExitMovementArea;
	
	#endregion

	public HorizontalMovementProfile horizontalData;
	
	public bool isAffectedByMovementAreas = true;


	[SerializeField]
	[Range( 0f , 1f )]
	[Tooltip("This multiplier will affect the overall walk speed, only if the character is crouching.")]
	float crouchSpeedMultiplier = 0.5f;


	[Header("Slide")]

	[SerializeField]
	float slideAcceleration = 10f;

	[SerializeField]
	float unstableInertia = 0.2f;

	
	[SerializeField]
	float unstableToStableInertia = 0.2f;
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


	HorizontalMovementProfile defaultHorizontalData;
	
	
	float horizontalSmoothDampSpeed = 0;

	GameObject collidedTrigger = null;	
	

	protected override void Awake()
	{
		base.Awake();

		if(horizontalData == null)
		{
			Debug.Log("Missing movement data");
			return;
		}

		defaultHorizontalData = horizontalData;

	}


	public override void Process( float dt )
	{
		if( !movementController.isCurrentlyOnState( MovementState.Normal ) && 
			!movementController.isCurrentlyOnState( MovementState.JetPack ) )
			return;

		if(horizontalData == null)
		{
			Debug.Log("Missing movement data");
			return;
		}
		
		ProcessHorizontalMovement( dt );
		ProcessMovementArea();
	}

	void ProcessHorizontalMovement( float dt )
	{				

		switch( characterController2D.CurrentState )
		{
			case CharacterMotorState.NotGrounded:
				ProcessNotGroundedMovement( dt );
				
				break;
			case CharacterMotorState.StableGrounded:
				ProcessStableGroundedMovement( dt );
				
				break;
			case CharacterMotorState.UnstableGrounded:
				ProcessUnstableGroundedMovement( dt );
				
				break;
		}

	}

	void ProcessHorizontalVelocity( float startDuration , float stopDuration )
	{
		float targetSpeed = 0;
		
		if( characterBrain.CharacterActions.right )
			targetSpeed = horizontalData.baseSpeed;
		else if( characterBrain.CharacterActions.left )
			targetSpeed = - horizontalData.baseSpeed;		
		
		if( characterController2D.PoseController.isCurrentlyOnState( PoseState.Crouch ) )
			targetSpeed *= crouchSpeedMultiplier;

		float speed = Mathf.SmoothDamp( 
			characterController2D.Velocity.x , 
			targetSpeed , 
			ref horizontalSmoothDampSpeed , 
			targetSpeed != 0 ? startDuration : stopDuration
		);
		
		characterController2D.SetVelocityX( speed );
	}

	protected virtual void ProcessNotGroundedMovement( float dt )
	{
		// If the previous state was not NotGrounded then convert the last displacement into the new velocity.
		if( characterController2D.PreviousState != CharacterMotorState.NotGrounded )
		{
			// If the character didn't leave the grounded state by force.
			if( !characterController2D.PreviousForceNotGroundedFlag )
				characterController2D.SetVelocity( characterController2D.PreviousLocalDisplacement / dt );
		}
		
		ProcessHorizontalVelocity( horizontalData.notGroundedStartDuration , horizontalData.notGroundedStopDuration );		
	}

	protected virtual void ProcessStableGroundedMovement( float dt )
	{
		// If the previous state was UnstableGrounded then multiply the intertia value with the velocity.
		if( characterController2D.PreviousState == CharacterMotorState.UnstableGrounded )
			characterController2D.SetVelocity( new Vector2( unstableToStableInertia * characterController2D.Velocity.x , characterController2D.Velocity.y ) );
		
		ProcessHorizontalVelocity( horizontalData.groundedStartDuration , horizontalData.groundedStopDuration );
	}

	protected virtual void ProcessUnstableGroundedMovement( float dt )
	{
		// If the previous state was not UnstableGrounded then multiply the intertia value with the velocity.
		if( characterController2D.PreviousState != CharacterMotorState.UnstableGrounded )
			characterController2D.SetVelocity( new Vector2( unstableInertia * characterController2D.Velocity.x , characterController2D.Velocity.y ) );
		

		float acceleration = characterController2D.IsOnRightVerticalSlope ? - slideAcceleration : slideAcceleration;		
		characterController2D.AddVelocityX( acceleration * dt );
	}

	

	



	void ProcessMovementArea()
	{
		if(!isAffectedByMovementAreas)
			return;

		if( characterController2D.CollidedTrigger == null )
		{
			if( collidedTrigger != null )
			{
				collidedTrigger = null;
				RevertMovementParameters();

				if( OnExitMovementArea != null )
					OnExitMovementArea();
			}
			
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

					
					if( OnEnterMovementArea != null )
						OnEnterMovementArea();
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
		
		if( data.horizontalMovementData != null )
		{
			this.horizontalData = data.horizontalMovementData;

			characterController2D.SetVelocityX( characterController2D.Velocity.x * data.horizontalMovementData.entrySpeedMultiplier );			
		}
		
	}

	void RevertMovementParameters()
	{	
		this.horizontalData = defaultHorizontalData;
		
	}


	public override string GetInfo()
	{ 
		return "It handles the horizontal movement of the character in normal state."; 
	}
	
}

}
