using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [CreateAssetMenu( menuName = "Kinematic 2D/Implementation/Movement/Horizontal Movement Data" ) , System.Serializable ]

namespace Lightbug.Kinematic2D.Implementation
{

[System.Serializable]
public class HorizontalMovementProfile
{
	[Header("Movement")]
	
	[Tooltip( "Walk speed in units per second.")]
	[Range_NoSlider(true)] 
	public float baseSpeed = 5f;

	[Header("Control")]

	[Tooltip( "Time for the character to reach the desired walk speed (grounded state).")]
	[Range_NoSlider(true)] 
	public float groundedStartDuration = 0.2f;	

	[Tooltip( "Time for the character to stop moving (grounded state).")]
	[Range_NoSlider(true)] 
	public float groundedStopDuration = 0.2f;

	[Tooltip( "Time for the character to reach the desired walk speed (not grounded state).")]
	[Range_NoSlider(true)] 
	public float notGroundedStartDuration = 0.35f;	

	[Tooltip( "Time for the character to stop moving (not grounded state).")]
	[Range_NoSlider(true)] 
	public float notGroundedStopDuration = 0.35f;

	[Header("Environment")]

	[Tooltip("This value will be multiplied to the character horizontal velocity everytime a new horizontal movement profile is loaded (this is used by the movement areas)")]
	[Range_NoSlider( true )] 
	public float entrySpeedMultiplier = 1f;
	
	
}

}
