using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Core
{

[System.Serializable]
public class DebugSettings
{
	public bool drawGizmos= true;

	public bool drawGroundMovementGizmos = true;

	public bool  drawGroundClampingGizmos = true;

	public bool drawGroundAlignmentGizmos = true;

	public bool drawVerticalAlignmentGizmos = true;
}


[System.Serializable]
public class LayerMaskSettings
{
	public CharacterMotorLayersProfile profile = null;
}

[System.Serializable]
public class GroundSettings
{	
	[Tooltip("With this option enabled the character will never change its state to \"not grounded\".")]
	public bool alwaysNotGrounded = false;

	[Tooltip("The maximum angle of a slope that the character can walk. (0.1 to 89.9 degrees)"
	+ "If the slope angle is higher than this value the character will perform the Slope Action.")]
	[Range_NoSlider( 0.1f , 89.9f )]
	public float maxSlopeAngle = 55;	
	

	[Tooltip("If the distance between the character and the ground (with a slope angle less than the maximum allowed) "
	+ "is less than this value the character will automatically be grounded, otherwise it will be not grounded.")]
	[Range_NoSlider( 0.01f , Mathf.Infinity )] 
	public float groundClampingDistance = 0.2f;
	
}

[System.Serializable]
public class VerticalAlignmentSettings
{
	[Tooltip("This mode will determine the vertical direction used by the character in order to align itself while it is not grounded.\n" + 
	"Local: Uses it own vertical direction (transform.up).\n" + 
	"World: Uses a fixed direction (worldVerticalDirection).\n" +
	"Object: Calculates the vertical direction based on a selected object from the scene.")]
	public VerticalAlignmentMode mode = VerticalAlignmentMode.World;
	
	
	public Vector3 worldVerticalDirection = Vector3.up;


	[Tooltip("Reference object for the \"Object\" Vertical Alignment Mode") ]
	public Transform verticalReferenceObject = null;

	[Tooltip("If this is true the vertical direction will be calculated from the character position to the reference position." + 
	" In other words, the head of the character will be pointing towards the reference)") ]
	public bool towardsTheReference = true;
	

}

[System.Serializable]
public class GroundAlignmentSettings
{
	[Tooltip("Whether or not the character should align itself to the ground slope")]
	public bool isEnabled = false;

	[Tooltip("Distance of the ray fired in the ground alignment method." + 
	"If the slope angle to be detected is big enough a low value of this field " + 
	"may not ensure ground alingment and provoke weird behaviours.")]
	[Range_NoSlider(true)] 
	public float detectionDistance = 1;

	[Tooltip("If this is true the character will still do ground alignment" +
	" even if one of both detection rays (left and right) doesn't detect stable ground.")]
	public bool canAlignWithOneRay = true;
	

}



[System.Serializable]
public class SlideSettings
{	

	[Tooltip("The maximum slide speed")]
	[Range_NoSlider(true)]
	public float speed = 7f;
	

	[Tooltip("Whether or not the slide speed should be affected by the slope angle (animation curve).")]
	public bool isAffectedBySlope = true;

	[Tooltip("Horizontal Axis : Angular difference (normalized) between the current slope angle and the max slope angle allowed.\n" + 
	"Vertical Axis : slide speed (normalized).")]
	public AnimationCurve influenceCurve = AnimationCurve.Linear( 0f ,0.5f ,1f , 1f );


}

[System.Serializable]
public class DynamicGroundSettings
{	

	[Tooltip("Should the character be affected by the movement of the ground?")]	
	public bool isEnabled = true;
	

}

[System.Serializable]
public class VelocitySettings
{	

	[Tooltip("Whether or not the character should recalcule its velocity to mantain it.\nThis will affect the "+
	"Grounded -> Not Grounded transition.")]
	public bool groundedToNotGroundedContinuity = true;

	[Tooltip("Whether or not the character should recalcule its velocity to mantain it.\nThis will affect the "+
	"Unstable (sliding) -> Stable transition.")]
	public bool unstableToStableContinuity = false;

	[Tooltip("Whether or not the character should recalcule its velocity to mantain it.\nThis will affect the "+
	"Unstable (sliding) -> Not Grounded transition.")]
	public bool unstableToNotGroundedContinuity = true;

}

[System.Serializable]
public class DepenetrationSettings
{	
	[Tooltip("Should the character depenetrate itself from moving colliders? [Extra cost in performance]")]		
	public bool isEnabled = true;

}


	



}


