using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Implementation
{

[System.Serializable]
public class VerticalMovementProfile
{

	[Range_NoSlider(true)] 
	[Tooltip("Time to reach the jump height.")]
	public float jumpDuration = 0.4f;

	[Range_NoSlider(true)] 
	[Tooltip("the jump height measured from the ground.")]
	public float jumpHeight = 2.5f;

	[Range_NoSlider( true )] 
	[Tooltip("This value will be multiplied to the character vertical velocity everytime a new vertical movement profile is loaded (this is used by the movement areas)")]
	public float entrySpeedMultiplier = 1f;
	
	[Range_NoSlider( true )]
	[Tooltip("This value will be multiplied to the gravity calculated (from the jump height and duration), affecting the descending movement. " + 
	"Useful for simulating different jumping and falling behaviours.")]
	public float descendingGravityMultiplier = 1f;

	[Tooltip("Number of jumps that the character can make in the air.")]
	public int availableNotGroundedJumps = 1;

	[Header("Jump cancellation")]	

	[Tooltip("If the jump button is released the character will stop jumping.")]
	public bool cancelOnRelease = true;

	[Tooltip("The ratio between 0 (ground) and 1 (maximum height) in which the cancel action will occur.")]
	[Range_NoSlider(true)] 
	public float cancelJumpMinTime = 0.1f;

	[Range_NoSlider(true)] 
	public float cancelJumpMaxTime = 0.2f;

	[Tooltip("The higher is the cancel Jump Factor the faster the chracter will try to return to the ground.")]
	[Range_NoSlider(true)] 
	public float cancelJumpFactor = 0.5f;

	[Header("Slopes")]	

	[Tooltip("Should the character jump exclusively on stable ground?")]
	public bool jumpOnlyOnStableGround = false;

}

}
