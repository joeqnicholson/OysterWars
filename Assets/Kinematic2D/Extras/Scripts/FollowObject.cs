using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Extras
{
	using Lightbug.Kinematic2D.Core;

public class FollowObject : MonoBehaviour
{

	public Transform target = null;	

	[Header("Position")]
	[SerializeField] bool followPosition = true;
	[SerializeField] bool followRotation = false;

	[SerializeField] bool followX = true;
	[SerializeField] bool followY = true;
	[SerializeField] bool followZ = true;

	[SerializeField] Vector3 offset = Vector2.zero;
	[SerializeField] float smoothTargetPositionTime = 0.25f;

	[Header("Rotation")]
	[Range(0.2f , 20f)] [SerializeField] float slerpFactor = 2f;

		

	Vector3 smoothDampVelocity;
	Vector3 currentTargetUp;

	// Transform catched
	Transform catchedTransform = null;
	new protected Transform transform
	{
		get
		{
			if( catchedTransform == null )
				catchedTransform = gameObject.transform;
			
			return catchedTransform;
		}
	}

	void Start ()
	{

		if(target == null)
			Destroy(this);


		if(followPosition)
			transform.position = target.position + offset.x * target.right + offset.y * target.up + offset.z * target.forward;	

		
		
	}
	
	
	void LateUpdate()
	{		
		float dt = Time.deltaTime;
		Vector3 targetPos = target.position + offset.x * target.right + offset.y * target.up + offset.z * target.forward;
		
		
		if(followPosition)
		{
			float posX = followX ? Mathf.SmoothDamp(transform.position.x , targetPos.x , ref smoothDampVelocity.x, smoothTargetPositionTime)
			: transform.position.x;

			float posY = followY ? Mathf.SmoothDamp(transform.position.y , targetPos.y , ref smoothDampVelocity.y, smoothTargetPositionTime)
			: transform.position.y;
			
			float posZ = followZ ? Mathf.SmoothDamp(transform.position.z , targetPos.z , ref smoothDampVelocity.z, smoothTargetPositionTime)
			: transform.position.z;

			transform.position = new Vector3(posX , posY , posZ);
		}
	
		if( followRotation )
			FollowRotation(dt);	
		
		
	}

	void FollowRotation(float dt)
	{
		Quaternion currentRotation = Quaternion.Slerp ( transform.rotation , target.rotation , slerpFactor * dt );

		transform.rotation = currentRotation;
	}
}

}
