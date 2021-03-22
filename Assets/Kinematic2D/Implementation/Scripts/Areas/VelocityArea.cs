using UnityEngine;
using Lightbug.Kinematic2D.Core;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

public enum ActionType
{
	SetVelocityY ,
	SetAccelerationY ,
	SetVelocityX ,
	SetAccelerationX	
}

public class VelocityArea : MonoBehaviour
{
	[SerializeField] 
	LayerMask layerMask = 0;
	
	[SerializeField] 
	ActionType actionType = ActionType.SetVelocityY;
	
	[SerializeField] 
	bool positiveValue = true;

	[Range_NoSlider(true)]
	[SerializeField] 
	float value = 4;

	[SerializeField] 
	bool forceNotGroundedState = true;

	[Tooltip("When the character enters the area automatically change its state to the Normal State.\n" + 
	"(note: this is useful to avoid states that fully control the velocity, like for example the dash state, " + 
	"if this field is disabled the dash movement will be inmune to the velocity area)")]
	[SerializeField] bool setNormalState = true;

	bool isActive = false;


	CharacterMotor targetCharacterMotor = null;
	CharacterController2D targetCharacterController = null;

	void OnTriggerEnter2D(Collider2D other)
	{
		if( !Utilities.CustomUtilities.BelongsToLayerMask( other.gameObject.layer , layerMask) )
			return;

		targetCharacterMotor = other.GetComponent<CharacterMotor>();
		if( targetCharacterMotor != null )
		{		
			targetCharacterController = other.GetComponent<CharacterController2D>();
			isActive = true;
		}
		
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if( !Utilities.CustomUtilities.BelongsToLayerMask( other.gameObject.layer , layerMask) )
			return;

		targetCharacterMotor = other.GetComponent<CharacterMotor>();
		if( targetCharacterMotor != null )
		{		
			targetCharacterController = other.GetComponent<CharacterController2D>();
			isActive = false;
		}
		
	}


	void OnTriggerEnter(Collider other)
	{
		if( !Utilities.CustomUtilities.BelongsToLayerMask( other.gameObject.layer , layerMask) )
			return;

		targetCharacterMotor = other.GetComponent<CharacterMotor>();
		if( targetCharacterMotor != null )
		{		
			targetCharacterController = other.GetComponent<CharacterController2D>();
			isActive = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if( !Utilities.CustomUtilities.BelongsToLayerMask( other.gameObject.layer , layerMask) )
			return;

		targetCharacterMotor = other.GetComponent<CharacterMotor>();
		if( targetCharacterMotor != null )
		{		
			targetCharacterController = other.GetComponent<CharacterController2D>();
			isActive = false;
		}
	}

	void DoAction( CharacterMotor characterMotor )
	{
		if(forceNotGroundedState)
			characterMotor.ForceNotGroundedState();

		if(setNormalState)
			targetCharacterController.MovementController.SetState( MovementState.Normal );

		float sign = positiveValue ? 1 : -1;
		switch(actionType)
		{
			case ActionType.SetVelocityX:
				characterMotor.SetVelocityX( sign * value );
			break;

			case ActionType.SetVelocityY:
				characterMotor.SetVelocityY( sign * value );
			break;

			case ActionType.SetAccelerationX:
				characterMotor.AddVelocityX( sign * value * Time.deltaTime);
			break;

			case ActionType.SetAccelerationY:
				characterMotor.AddVelocityY( sign * value * Time.deltaTime );
			break;			
		}
		
	}


	void Update()
	{
		if( !isActive )
			return;
		
		DoAction( targetCharacterMotor );
		
	}
}

}
