using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;


namespace Lightbug.Kinematic2D.Implementation
{
	
[AddComponentMenu("Kinematic2D/Implementation/Character Controller 2D")]
public sealed class CharacterController2D : CharacterMotor
{	
	
	CharacterBrain characterBrain;
	

	StateMachineController<MovementState> movementController = new StateMachineController<MovementState>();
	public StateMachineController<MovementState> MovementController
	{
		get
		{
			return movementController;
		}
	}

	StateMachineController<PoseState> poseController = new StateMachineController<PoseState>();
	public StateMachineController<PoseState> PoseController
	{
		get
		{
			return poseController;
		}
	}
	
	

	List<CharacterAbility> abilitiesList = new List<CharacterAbility>();

	
	// Movement State -------------------------------------------------------------------------------------------
	public MovementState CurrentMovementState
	{
		get
		{ 
			return movementController.CurrentState;
		} 
	}  

	public MovementState PreviousMovementState
	{
		get
		{ 
			return movementController.PreviousState;
		} 
	} 

	// Pose State -------------------------------------------------------------------------------------------------
	public PoseState CurrentPoseState
	{
		get
		{ 
			return poseController.CurrentState;
		} 
	} 

	public PoseState PreviousPoseState
	{
		get
		{ 
			return poseController.PreviousState;
		} 
	} 

	//--------------------------------------------------------------------------------------------------------------
	
		
	
	
	protected override void Awake()
	{          		
		base.Awake();
			
		characterBrain = transform.root.GetComponentInChildren<CharacterBrain>();
		if( characterBrain == null )
			Debug.Log( "\"CharacterBrain\" component is missing." );		
				
		movementController.Initialize( gameObject );
		poseController.Initialize( gameObject );		
         
		InitializeAbilities();
	}	

	void InitializeAbilities()
	{
		CharacterAbility[] abilitiesArray = transform.root.GetComponentsInChildren<CharacterAbility>();
		for (int i = 0; i < abilitiesArray.Length ; i++)
		{
			abilitiesList.Add( abilitiesArray[i] );
		}

	}

			
	protected override void UpdateBehaviour( float dt )
	{			
		UpdateAbilities( dt );
		
		base.UpdateBehaviour( dt );	

		if( characterBrain != null )
			if( !characterBrain.IsAI )
				characterBrain.ResetActions();

	}
	
		
	void UpdateAbilities( float dt )
	{

		for (int i = 0; i < abilitiesList.Count; i++)
		{
			CharacterAbility ability = abilitiesList[i];

			if( ability == null )
				continue;

			if( !ability.enabled )
				return;
		
			ability.Process( dt );		

		}       

	}

	

	
}


}
