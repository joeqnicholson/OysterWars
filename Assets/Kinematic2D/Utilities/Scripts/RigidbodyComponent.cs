using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is an encapsulation of the Rigidbody and Rigidbody2D components, containing the most commonly used 
/// properties and methods from these components.
/// </summary>
public abstract class RigidbodyComponent : MonoBehaviour
{
	public abstract float Mass{ get; set; }

	public abstract bool IsKinematic{ get; set; }

	public abstract bool UseGravity{ get; set; }

	public abstract bool UseInterpolation{ get; set; }

	public abstract bool ContinuousCollisionDetection{ get; set; }

	public abstract RigidbodyConstraints Constraints{ get; set; }

	 /// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.up).
    /// </summary>
	public Vector3 Up
	{
		get
		{
			return Rotation * Vector3.up;
		}
	}

	/// <summary>
    /// Gets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
    /// </summary>
	public Vector3 Forward
	{
		get
		{
			return Rotation * Vector3.forward;
		}
	}

	/// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right)
    /// </summary>
	public Vector3 Right
	{
		get
		{
			return Rotation * Vector3.right;
		}
	}

	/// <summary>
    /// Gets the rigidbody position.
    /// </summary>
    public abstract Vector3 Position{ get; set; }	

    /// <summary>
    /// Gets the rigidbody rotation.
    /// </summary>
	public abstract Quaternion Rotation{ get; set;}

	public abstract Vector3 Velocity{ get; set; }
	

    /// <summary>
    /// Sets the rigidbody position and rotation.
    /// </summary>
    public abstract void SetPositionAndRotation( Vector3 position , Quaternion rotation );    

	/// <summary>
	/// Interpolates the rigidbody position (equivalent to MovePosition).
	/// </summary>
	/// <param name="position"></param>
    public abstract void Interpolate(Vector3 position);

	/// <summary>
	/// Interpolates the rigidbody position and rotation (equivalent to MovePosition and MoveRotation).
	/// </summary>
	public abstract void Interpolate(Vector3 position, Quaternion rotation );

    /// <summary>
    /// Gets the rigidbody velocity at a specific point in space.
    /// </summary>
    public abstract Vector3 GetPointVelocity( Vector3 point );

	public abstract void AddForceToRigidbody( Vector3 force , ForceMode forceMode = ForceMode.Force );

	protected virtual void Awake()
    {
        this.hideFlags = HideFlags.HideInInspector;
    }

}

}
