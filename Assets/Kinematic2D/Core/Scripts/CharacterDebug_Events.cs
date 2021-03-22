using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Kinematic2D.Core
{

[AddComponentMenu("Kinematic2D/Core/Debug/Character Events")]
public class CharacterDebug_Events : MonoBehaviour
{

	CharacterMotor characterMotor;	

	void OnEnable ()
	{
		characterMotor = transform.root.GetComponentInChildren<CharacterMotor>();
				
		characterMotor.OnHeadCollision += OnHeadCollision;
		characterMotor.OnGroundCollision += OnGroundCollision;
		characterMotor.OnLeftCollision += OnLeftCollision;
		characterMotor.OnRightCollision += OnRightCollision;
		characterMotor.OnCrushedCollision += OnCrushedCollision;

		characterMotor.OnNotGroundedLeftCollision += OnNotGroundedLeftCollision;
		characterMotor.OnNotGroundedRightCollision += OnNotGroundedRightCollision;
		characterMotor.OnGroundedLeftCollision += OnGroundedLeftCollision;
		characterMotor.OnGroundedRightCollision += OnGroundedRightCollision;

		
	}

	void OnDisable ()
	{
		characterMotor.OnHeadCollision -= OnHeadCollision;
		characterMotor.OnGroundCollision -= OnGroundCollision;
		characterMotor.OnLeftCollision -= OnLeftCollision;
		characterMotor.OnRightCollision -= OnRightCollision;

		characterMotor.OnNotGroundedLeftCollision -= OnNotGroundedLeftCollision;
		characterMotor.OnNotGroundedRightCollision -= OnNotGroundedRightCollision;
		characterMotor.OnGroundedLeftCollision -= OnGroundedLeftCollision;
		characterMotor.OnGroundedRightCollision -= OnGroundedRightCollision;
	}
	
	void OnHeadCollision()
	{
		Debug.Log("Head");
	}

	void OnGroundCollision()
	{
		Debug.Log("Ground");
	}

	void OnLeftCollision()
	{
		Debug.Log("Left");
	}

	void OnRightCollision()
	{
		Debug.Log("Right");
	}

	void OnCrushedCollision()
	{
		Debug.Log("Crushed");
	}

	void OnNotGroundedLeftCollision()
	{
		Debug.Log("Not Grounded Left");
	}

	void OnNotGroundedRightCollision()
	{
		Debug.Log("Not Grounded Right");
	}

	void OnGroundedLeftCollision()
	{
		Debug.Log("Grounded Left");
	}

	void OnGroundedRightCollision()
	{
		Debug.Log("Grounded Right");
	}

	
}

}

