using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is an encapsulation of the Physics and Physics2D classes, containing the most commonly used 
/// methods from these components. Also it holds the information from the collision and trigger messages received, such as 
/// contacts, trigger, etc.
/// </summary>
public abstract class PhysicsComponent : MonoBehaviour
{
	protected int hits = 0;
	public List<Contact> Contacts { get; protected set; } = new List<Contact>();

	public List<Trigger> Triggers { get; protected set; } = new List<Trigger>();
	
	

	protected virtual void Awake()
    {
        this.hideFlags = HideFlags.HideInInspector;
    }
	
	public abstract void IgnoreLayerCollision( int layerA , int layerB , bool ignore );    
	

	// Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public void ClearContacts()
	{	
		Contacts.Clear();
	}



	// Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public abstract int Raycast( out HitInfo hitInfo , Vector3 origin , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true );

	public abstract int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true );

	public abstract int BoxCast( out HitInfo hitInfo , Vector3 center , Vector3 size , Vector3 up , Vector3 castDisplacement , LayerMask layerMask , bool ignoreTrigger = true );

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public abstract bool OverlapSphere( Vector3 center , float radius , LayerMask layerMask , bool ignoreTrigger = true );

    public abstract bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , LayerMask layerMask , bool ignoreTrigger = true );
	
	
}

}

