using UnityEngine;
using Lightbug.Kinematic2D.Core;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

[AddComponentMenu("Kinematic2D/Implementation/Animation/CharacterAnimation( Animator )")]
public class CharacterAnimation : MonoBehaviour
{	
	
	[Header("State names")]

	[SerializeField]
	string slopeSlideName = "SlopeSlide";

	[SerializeField] 
	string groundName = "Ground";

	[SerializeField] 
	string airName = "Air";

	[SerializeField] 
	string dashName = "Dash";

	[SerializeField] 
	string wallSlideName = "WallSlide";

	[SerializeField] 
	string jetPackName ="JetPack";

	[SerializeField] 
	string crouchName = "Crouch";

	[Header("Animation parameters")]

	[SerializeField] 
	string localVerticalVelocityName = "LocalVerticalVelocity";

	[SerializeField] 
	string horizontalSpeedName = "HorizontalSpeed";

	[SerializeField]
	string horizontalAxisName = "HorizontalAxis";

	[Range(0.1f , 1f)]
	[SerializeField] 
	float airBlendSensitivity = 0.4f;


	CharacterController2D characterController2D;		
	CharacterBrain characterBrain;

	public Animator animator { get; private set; }
	

	protected virtual void Awake()
	{		
		
		characterController2D = transform.parent.GetComponentInChildren<CharacterController2D>();
		if( characterController2D == null )
		{			
			Debug.Log( "\"CharacterController2D\" component is missing, Does the parent contain this component?" );
			this.enabled = false;
			return;
		}
		
		
		characterBrain = transform.parent.GetComponentInChildren<CharacterBrain>();
		if( characterBrain == null )
		{
			Debug.Log( "\"CharacterBrain\" component is missing, Does the parent contain this component?" );
			this.enabled = false;
			return;
		}
		
		animator = gameObject.GetOrAddComponent<Animator>();
		if( animator.runtimeAnimatorController == null )
		{
			Debug.Log("The Runtime animator controller is Missing");
		}
				
	}
	
	
		
	void LateUpdate()
	{		
		if( animator.runtimeAnimatorController == null )
			return;
		
		float dt = Time.deltaTime;

		if( characterController2D.PoseController.isCurrentlyOnState( PoseState.Crouch ))
		{
			if( !isCurrentlyOnState( crouchName ) )
				PlayAnimation( crouchName );
			
			return;
		}
		
		UpdateAnimationParameters();


		switch (characterController2D.MovementController.CurrentState )
		{
			case MovementState.Normal:

				switch( characterController2D.CurrentState )
				{
					case CharacterMotorState.StableGrounded:

						if( !isCurrentlyOnState( groundName ) )
							PlayAnimation( groundName );						
						
						break;
					case CharacterMotorState.UnstableGrounded:
						if( !isCurrentlyOnState( slopeSlideName ) )
							PlayAnimation( slopeSlideName );
						
						break;
					case CharacterMotorState.NotGrounded:

						if( !isCurrentlyOnState( airName ) )
						PlayAnimation( airName );
						
						break;
				}

				
				

				break;

			case MovementState.Dash:

				if( !isCurrentlyOnState( dashName ) )
					PlayAnimation( dashName );

				break;
		
			case MovementState.WallSlide:

				if( !isCurrentlyOnState( wallSlideName ) )
					PlayAnimation( wallSlideName );
			
				break;		    
		
			case MovementState.JetPack:

			if( !isCurrentlyOnState( jetPackName ) )
				PlayAnimation( jetPackName );

				break;
		    
		}
		
	}	


	
	/// <summary>
	/// Checks if the current animation state is equal to a given state.
	/// </summary>
	protected virtual bool isCurrentlyOnState( string stateName )
	{
		return animator.GetCurrentAnimatorStateInfo(0).IsName( stateName );
	}

	/// <summary>
	/// Checks if the current animation state is equal to a given state.
	/// </summary>
	protected virtual void PlayAnimation( string animationName )
	{
		animator.Play( animationName );
	}


	/// <summary>
	/// Sends the animation parameters values to the Animator.
	/// </summary>
	protected virtual void UpdateAnimationParameters()
	{
		float localVerticalVelocity = airBlendSensitivity * characterController2D.Velocity.y;
		float horizontalSpeed = Mathf.Abs( characterController2D.Velocity.x );
		float horizontalAxis = characterBrain.CharacterActions.left ? - 1f : characterBrain.CharacterActions.right ? 1f : 0f;
				
		animator.SetFloat( localVerticalVelocityName , localVerticalVelocity );
		animator.SetFloat( horizontalSpeedName , horizontalSpeed );
		animator.SetFloat( horizontalAxisName , horizontalAxis );
	}

}

}
