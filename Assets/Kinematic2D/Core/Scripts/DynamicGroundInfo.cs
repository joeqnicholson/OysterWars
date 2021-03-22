using UnityEngine;

namespace Lightbug.Kinematic2D.Core
{

/// <summary>
/// This struct is a contains all the info related to the current dynamic ground.
/// </summary>
public struct DynamicGroundInfo
{
	Transform transform;

	public KinematicPlatform KinematicPlatform{ get; private set; }

	public Vector3 previousPosition;
	public Quaternion previousRotation;
	
	/// <summary>
	/// Resets the all the fields to default.
	/// </summary>
	public void Reset()
	{
		transform = null;
		
		KinematicPlatform = null;

		previousPosition = Vector3.zero;		
		previousRotation = Quaternion.identity;
	}

	/// <summary>
	/// Gets the Transfrom of the current dynamic ground.
	/// </summary>
	public Transform Transform
	{
		get
		{
			return transform;
		}
	}

	/// <summary>
	/// Gets if the character is currently standing on dynamic ground or not.
	/// </summary>
	public bool IsActive
	{
		get
		{
			return transform != null;
		}
	}
	

	/// <summary>
	/// Gets the dynamic ground Rigidbody position.
	/// </summary>
	public Vector3 RigidbodyPosition
	{
		get
		{
			Vector3 position = default( Vector3 );

			if( KinematicPlatform != null )			
				position = KinematicPlatform.RigidbodyComponent.Position;
			
			return position;
		}
	}
	
	/// <summary>
	/// Gets the dynamic ground Rigidbody rotation.
	/// </summary>
	public Quaternion RigidbodyRotation
	{
		get
		{
			Quaternion rotation = default( Quaternion );

			if( KinematicPlatform != null )		
				rotation = KinematicPlatform.RigidbodyComponent.Rotation;
			
			return rotation;
		}
	}

	/// <summary>
	/// Updates the dynamic ground info.
	/// </summary>
	public void UpdateTarget( KinematicPlatform kinematicPlatform , Vector3 characterPosition )
	{
		this.KinematicPlatform = kinematicPlatform;

		if( this.KinematicPlatform == null )
		{
			Reset();
			return;
		}

		this.transform = kinematicPlatform.transform;

		previousPosition = kinematicPlatform.RigidbodyComponent.Position;
		previousRotation = kinematicPlatform.RigidbodyComponent.Rotation;
	}

	

}

}