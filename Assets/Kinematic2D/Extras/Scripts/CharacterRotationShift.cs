using UnityEngine;
using Lightbug.Kinematic2D.Core;

namespace Lightbug.Kinematic2D.Extras
{

using Kinematic2D;

[AddComponentMenu("Kinematic2D/Extras/Rotation Shift")]
public class CharacterRotationShift : MonoBehaviour {


	[SerializeField] bool rotateZ = false;
	[SerializeField] float rotationAmountZ = 180;


	void OnTriggerEnter2D(Collider2D other)
	{
		CharacterMotor characterMotor = other.GetComponent<CharacterMotor>();
		if( characterMotor == null )
			return;

		DoAction( characterMotor );
	}

	void OnTriggerEnter(Collider other)
	{
		CharacterMotor characterMotor = other.GetComponent<CharacterMotor>();
		if( characterMotor == null )
			return;

		DoAction( characterMotor );
	}

	void DoAction( CharacterMotor characterMotor )
	{
		characterMotor.Teleport( transform.position , transform.rotation );		
		characterMotor.ResetVelocity();

		if(rotateZ)
			transform.Rotate(0,0, rotationAmountZ);
	}
}

}
