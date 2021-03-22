using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Implementation
{

[AddComponentMenu("Kinematic2D/Implementation/Abilities/Jet Pack")]
public class JetPack : CharacterAbility
{
	#region Events	

	public event CharacterAbilityEvent OnJetPackStarted;
	public event CharacterAbilityEvent OnJetPackStopped;
	
	#endregion


	[SerializeField] float propelSpeed = 10f;
	[SerializeField] float duration = 0.5f;

	float smoothDampSpeed;


	public override void Process(float dt)
	{	
		if( movementController.isCurrentlyOnState( MovementState.Normal ) )
		{
			if( characterBrain.CharacterActions.jetPack )
			{
				movementController.SetState( MovementState.JetPack );
				characterController2D.ForceNotGroundedState();

				smoothDampSpeed = 0f;

				if( OnJetPackStarted != null )
					OnJetPackStarted();
			}
			

		}
		else if( movementController.isCurrentlyOnState( MovementState.JetPack ) )
		{	

			float ySpeed = Mathf.SmoothDamp( characterController2D.Velocity.y , propelSpeed , ref smoothDampSpeed , duration );
			characterController2D.SetVelocityY( ySpeed );

			if( !characterBrain.CharacterActions.jetPack || characterController2D.IsGrounded )
			{
				movementController.SetState( MovementState.Normal );

				if( OnJetPackStopped != null )
					OnJetPackStopped();
			}

		}
		

	}

	public override string GetInfo()
	{ 
		return "Allows the character to get a certain vertical velocity in a given time, just like a \"jetpack\"."; 
	}

	
	
}

}
