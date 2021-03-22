using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Extras
{

[AddComponentMenu("Kinematic2D/Extras/Sine Animation")]
public class SineAnim : MonoBehaviour {

	float time= 0;
	Vector3 initialPosition;
	public float amp = 0.25f;
	public Transform initialPositionObj = null;

	enum AnimationDirection { Forward , Right , Up }
	[SerializeField] AnimationDirection animationDirection = AnimationDirection.Right;

	Vector3 animationDirectionVector;

	void Start () 
	{
		if(initialPositionObj == null)
			initialPosition = transform.position;
		else
			initialPosition = initialPositionObj.position;

		switch(animationDirection)
		{
			case AnimationDirection.Forward:
				animationDirectionVector = transform.forward;
				break;
			case AnimationDirection.Right:
				animationDirectionVector = transform.right;
				break;
			case AnimationDirection.Up:
				animationDirectionVector = transform.up;
				break;
		}
		
		
	}
	
	
	void Update () 
	{
		time += Time.deltaTime;
		transform.position = initialPosition + amp * animationDirectionVector * Mathf.Sin(time);		
	}
}

}
