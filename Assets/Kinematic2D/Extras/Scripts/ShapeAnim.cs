// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// namespace Lightbug.Kinematic2D.Extras
// {
// 	using Lightbug.Kinematic2D.Core;

// [AddComponentMenu("Kinematic2D/Extras/Shape Animation")]
// public class ShapeAnim : MonoBehaviour
// {
// 	[SerializeField] AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
// 	[SerializeField] float amplitude = 1f;
// 	[SerializeField] float speedMultiplier = 1f;
	
	
// 	enum AnimationDirection { Forward , Right , Up }
// 	[SerializeField] AnimationDirection animationDirection = AnimationDirection.Right;

// 	float time= 0;
// 	float cursor = 0;
// 	Vector3 initialPosition;	
// 	Vector3 animationDirectionVector;

	
// 	protected override void Awake() 
// 	{		
// 		base.Awake();	

// 		initialPosition = transform.position;

// 		switch(animationDirection)
// 		{
// 			case AnimationDirection.Forward:
// 				animationDirectionVector = transform.forward;
// 				break;
// 			case AnimationDirection.Right:
// 				animationDirectionVector = transform.right;
// 				break;
// 			case AnimationDirection.Up:
// 				animationDirectionVector = transform.up;
// 				break;
// 		}
		
		
// 	}
	
	
// 	void Update()
// 	{
// 		float dt = Time.deltaTime;
// 		time += dt;

// 		MoveRigidbody( initialPosition + curve.Evaluate( cursor ) * amplitude * animationDirectionVector );
		
// 		cursor += speedMultiplier * dt;
// 	}

// }

// }


