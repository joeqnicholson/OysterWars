using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;



namespace Lightbug.Kinematic2D.Core
{



[AddComponentMenu("Kinematic2D/Implementation/Character Controller 2D")]
public abstract class CharacterMovementController : MonoBehaviour
{	
	public virtual void PreUpdate( float dt )
	{
		
	}

	public virtual void SetLinearDisplacement( ref Vector3 linearDisplacement , float dt )
	{
		
	}

	public virtual void PostUpdate( float dt )
	{
		
	}	
		
	
}


}
