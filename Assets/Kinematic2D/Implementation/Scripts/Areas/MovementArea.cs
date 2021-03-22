using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Kinematic2D.Core;
// using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Implementation
{

public class MovementArea : MonoBehaviour
{
	[Header("Layer")]

	[SerializeField] LayerMask layerMask;	
	

	[SerializeField] CharacterMovementProfile characterMovementData = null;
	public CharacterMovementProfile CharacterMovementData
	{
		get
		{
			return characterMovementData;
		}
	}

		
	void Awake()
	{
		if( characterMovementData == null )
		{
			Debug.Log( "Movement Data not assigned in gameObject " + gameObject.name + "!");			
		}

	}
	

}

}
