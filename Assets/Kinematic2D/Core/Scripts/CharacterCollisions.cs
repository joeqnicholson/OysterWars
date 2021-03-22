using UnityEngine;
using System.Text;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{



/// <summary>
/// Directions of the cardinal collision method.
/// </summary>
public enum CardinalCollisionType
{
	Up, 
	Down, 
	Left, 
	Right
}


public struct GroundDetectionRay
{
	public bool collision;
	public Vector3 point;
	public float distance;
	public Vector3 normal;
	public float verticalSlopeSignedAngle;
	public float verticalSlopeAngle;

	public bool stable;

	public void Reset()
	{
		collision = false;
		point = Vector3.zero;
		distance = 0;
		normal = Vector3.up;
		verticalSlopeSignedAngle = 0;
		verticalSlopeAngle = 0;

		stable = false;
	}
}

public struct GroundAlignmentResult
{	
	public GroundDetectionRay leftRay;
	public GroundDetectionRay rightRay;

	public void Reset()
	{		
		leftRay.Reset();
		rightRay.Reset();

	}
}

[RequireComponent( typeof( CharacterBody ) )]
[RequireComponent( typeof( CharacterMotor ) )]
public class CharacterCollisions : MonoBehaviour
{

	CharacterBody characterBody = null;
	CharacterMotor characterMotor = null;
	
	public PhysicsComponent PhysicsComponent { get; private set;}

	void Awake()
	{
		hideFlags = HideFlags.HideInInspector;

		characterBody = transform.root.GetComponentInChildren<CharacterBody>();
		characterMotor = transform.root.GetComponentInChildren<CharacterMotor>();

		if( characterBody.Is3D )
			PhysicsComponent = gameObject.AddComponent<PhysicsComponent3D>();
		else
			PhysicsComponent = gameObject.AddComponent<PhysicsComponent2D>();

		
	}

			
	/// <summary>
	/// Performs a collision test in any of the four cardinal directions.
	/// </summary>
	public HitInfo CardinalCollision( Vector3 position , CardinalCollisionType cardinalCollisionType, float skin , float extraDistance , LayerMask layerMask)
	{		
		Vector3 origin = Vector3.zero;

		float castDistance = skin + extraDistance;

		Vector3 direction = Vector3.zero;

		Vector3 center = position + characterMotor.Up * characterBody.HeightExtents;

		switch(cardinalCollisionType)
		{
			case CardinalCollisionType.Up:
				direction = characterMotor.Up;
				origin = center + direction * (characterBody.height/2 - skin);
			break;
			case CardinalCollisionType.Down:
				direction = - characterMotor.Up;
				origin = center + direction * (characterBody.height/2 - skin);
			break;
			case CardinalCollisionType.Left:
				direction = - characterMotor.Right;
				origin = center + direction * (characterBody.width/2 - skin);
			break;
			case CardinalCollisionType.Right:
				direction = characterMotor.Right;
				origin = center + direction * (characterBody.width/2 - skin);
			break;
		}

		HitInfo hitInfo;
		PhysicsComponent.Raycast( 
			out hitInfo ,
			origin ,
			direction * castDistance ,
			layerMask
		);					

		

		return hitInfo;
	}
	


	/// <summary>
	/// Performs a collision detection in the horizontal grounded direction.
	/// </summary>
	public HitInfo HorizontalGroundedCollision( Vector3 position , Vector3 displacement ,  LayerMask layerMask )
	{	

		// if( displacement.magnitude < 0 )
		// 	return new HitInfo();
							
		Vector3 castDisplacement = displacement.normalized * ( displacement.magnitude + CharacterConstants.SkinWidth );
		
		HitInfo hitInfo;
		PhysicsComponent.BoxCast( 
			out hitInfo ,
			characterBody.GetCenter_StepOffset( position ) ,
			characterBody.CollisionBodySize_StepOffset,			
			characterMotor.Up ,
			castDisplacement ,
			layerMask
		);	

		return hitInfo;
	}

	/// <summary>
	/// Performs a collision detection in the horizontal direction, considering the whole character characterBody.
	/// </summary>
	public HitInfo HorizontalNotGroundedCollision( Vector3 position , float deltaMovement , LayerMask layerMask)
	{	

		float movementSign = Mathf.Sign( deltaMovement );
		float movementAmount = Mathf.Abs( deltaMovement );		

		float castDistance = CharacterConstants.SkinWidth + movementAmount;						
		Vector3 castDirection = movementSign * characterMotor.Right;
		

		// Vector3 boxCenter = characterBody.GetCenter( position ) + 
		// movementSign * characterMotor.Right * ( characterBody.WidthExtents - CharacterConstants.SkinWidth - ( CharacterConstants.BoxThickness )/2 );						

		HitInfo hitInfo;
		PhysicsComponent.BoxCast( 
			out hitInfo ,
			characterBody.GetCenter( position ) ,
			characterBody.CollisionBodySize,			
			characterMotor.Up ,
			castDirection * castDistance ,
			layerMask
		);

		return hitInfo;

	}

	/// <summary>
	/// Performs a collision detection in the vertical direction, considering the whole character characterBody.
	/// </summary>
	public HitInfo VerticalNotGroundedCollision( Vector3 position , float deltaMovement , LayerMask layerMask )
	{

		float movementSign = Mathf.Sign( deltaMovement );
		float movementAmount = Mathf.Abs( deltaMovement );
		
		float castDistance = CharacterConstants.SkinWidth + movementAmount;	
		Vector3 castDirection = movementSign * characterMotor.Up;	
		

		Vector3 boxCenter = characterBody.GetCenter( position );
		
		HitInfo hitInfo;
		PhysicsComponent.BoxCast( 
			out hitInfo ,
			boxCenter ,
			characterBody.CollisionBodySize,			
			characterMotor.Up ,
			castDirection * castDistance ,
			layerMask
		);


		return hitInfo;

	}

	/// <summary>
	/// Performs a collision test towards the ground.
	/// </summary>
	public HitInfo ProbeGroundCollision( Vector3 position , float groundClampingDistance , LayerMask layerMask )
	{			

		if( groundClampingDistance < 0)
			return new HitInfo();
		
		float castDistance = characterBody.StepOffset + groundClampingDistance;
		
		
		// BOXCAST ----------------------------------------------------------------------------------------------------------------
		// Vector3 boxCenter = position + 
		// characterMotor.Up * ( characterBody.StepOffset + CharacterConstants.BoxThickness/2 );

		Vector3 boxCenter = characterBody.GetCenter_FullStepOffset( position );
		
		
		
		HitInfo hitInfo;
		PhysicsComponent.BoxCast( 
			out hitInfo ,
			boxCenter ,
			characterBody.CollisionBodySize_StepOffset ,  //characterBody.VerticalBoxSize ,			
			characterMotor.Up ,
			- characterMotor.Up * castDistance ,
			layerMask
		);

		return hitInfo;
		

	}

	/// <summary>
	/// Performs a collision test in the horizontal direction (depending of the mode selected) in order to
	/// depenetrate the character from moving colliders.
	/// </summary>
	public HitInfo HorizontalDepenetrationCollision( Vector3 position , bool grounded , bool positiveDirection , LayerMask layerMask)
	{ 
		float xDirection = positiveDirection ? 1 : -1;	
		Vector3 castDirection = xDirection * characterMotor.Right;
		float castDistance = characterBody.width - CharacterConstants.DepenetrationBoxThickness - CharacterConstants.SkinWidth;	
		

		Vector3 boxCenter = Vector3.zero;
		Vector3 boxSize = Vector3.zero;

		if( grounded )
		{
			boxCenter = positiveDirection ?
			characterBody.GetBottomLeftCollision_StepOffset( position ) +
			characterMotor.Up * ( characterBody.HorizontalCollisionArea_StepOffset / 2f ) +
			characterMotor.Right * ( CharacterConstants.DepenetrationBoxThickness / 2f ) :
			characterBody.GetBottomRightCollision_StepOffset( position ) +
			characterMotor.Up * ( characterBody.HorizontalCollisionArea_StepOffset / 2f ) -
			characterMotor.Right * ( CharacterConstants.DepenetrationBoxThickness / 2f );

			boxSize = new Vector3( 
				CharacterConstants.DepenetrationBoxThickness  ,
				characterBody.HorizontalCollisionArea_StepOffset ,
				characterBody.depth
			);		
		}
		else
		{				
			boxCenter = positiveDirection ?
			characterBody.GetMiddleLeftCollision( position ) + characterMotor.Right * ( CharacterConstants.DepenetrationBoxThickness / 2f ) :
			characterBody.GetMiddleRightCollision( position ) - characterMotor.Right * ( CharacterConstants.DepenetrationBoxThickness / 2f );	
		
			boxSize = new Vector3( 
				CharacterConstants.DepenetrationBoxThickness  ,
				characterBody.HorizontalCollisionArea ,
				characterBody.depth
			);		
		}	
	

		HitInfo hitInfo;
		PhysicsComponent.BoxCast( 
			out hitInfo ,
			boxCenter ,
			boxSize ,			
			characterMotor.Up ,
			xDirection * characterMotor.Right * castDistance ,
			layerMask
		);


		return hitInfo;

	}

	/// <summary>
	/// Performs a collision test in the horizontal direction (depending of the mode selected) in order to
	/// depenetrate the character from moving colliders.
	/// </summary>
	public HitInfo VerticalDepenetrationCollision( Vector3 position , bool positiveDirection , LayerMask layerMask )
	{ 		
		float yDirection = positiveDirection ? 1 : -1;

		Vector3 castDirection = yDirection * characterMotor.Up;				
		float castDistance = characterBody.height - CharacterConstants.DepenetrationBoxThickness - CharacterConstants.SkinWidth;		
		
		// BOXCAST -------------------------------------------------------------------------------------------------------------------
		Vector3 boxCenter = positiveDirection ?
			position + characterMotor.Up * ( CharacterConstants.SkinWidth + CharacterConstants.DepenetrationBoxThickness / 2 ) :
			position + characterMotor.Up * ( characterBody.height - ( CharacterConstants.SkinWidth + CharacterConstants.DepenetrationBoxThickness / 2 ) ); 
		
		Vector3 boxSize = new Vector3( 
			characterBody.width - 2 * CharacterConstants.SkinWidth  ,
			CharacterConstants.DepenetrationBoxThickness  ,
			characterBody.depth
		);

		HitInfo hitInfo;
		PhysicsComponent.BoxCast( 
			out hitInfo ,
			boxCenter ,
			boxSize ,			
			characterMotor.Up ,
			yDirection * characterMotor.Up * castDistance ,
			layerMask
		);

		return hitInfo;

	}


	/// <summary>
	/// Performs the collision detection method used to align the character to the ground.
	/// </summary>
	public GroundAlignmentResult GroundRaysCollisions( Vector3 position , float skin, float extraDistance , float maxSlopeAngle , LayerMask layerMask )
	{		
		GroundAlignmentResult result = new GroundAlignmentResult();
		result.Reset();

		
		
		if(extraDistance < 0)
			return result;
		
		float castDistance = skin + extraDistance;
		
		Vector3 leftRayOrigin = position - characterMotor.Right * characterBody.VerticalCollisionArea/2 + characterMotor.Up * skin;
		Vector3 rightRayOrigin = position + characterMotor.Right * characterBody.VerticalCollisionArea/2 + characterMotor.Up * skin;
			
		
		HitInfo leftHitInfo;
		PhysicsComponent.Raycast( 
			out leftHitInfo ,
			leftRayOrigin , 
			- characterMotor.Up * castDistance ,
			layerMask
		);
		
		HitInfo rightHitInfo;		
		PhysicsComponent.Raycast( 
			out rightHitInfo ,
			rightRayOrigin , 
			- characterMotor.Up * castDistance ,
			layerMask
		);


		float leftLocalSlopeSignedAngle = Vector3.SignedAngle(characterMotor.Up , leftHitInfo.normal , transform.forward );
		float leftLocalSlopeAngle = Mathf.Abs(leftLocalSlopeSignedAngle);

		float rightLocalSlopeSignedAngle = Vector3.SignedAngle(characterMotor.Up , rightHitInfo.normal , transform.forward );
		float rightLocalSlopeAngle = Mathf.Abs(rightLocalSlopeSignedAngle);

	
		
		if( ( leftHitInfo.hit && Utilities.CustomUtilities.isCloseTo( leftLocalSlopeAngle , 90 , 0.01f ) ) || 
			( rightHitInfo.hit && Utilities.CustomUtilities.isCloseTo( rightLocalSlopeAngle , 90 , 0.01f ) ) )
			return result;


		// Left result
		result.leftRay.collision = leftHitInfo.hit;
		if( leftHitInfo.hit )
		{
			result.leftRay.point = leftHitInfo.point;
			result.leftRay.distance = leftHitInfo.distance;
			result.leftRay.normal = leftHitInfo.normal;
			result.leftRay.verticalSlopeSignedAngle = Vector3.SignedAngle(characterMotor.CurrentVerticalDirection , leftHitInfo.normal , Vector3.forward );
			result.leftRay.verticalSlopeAngle = Mathf.Abs( result.leftRay.verticalSlopeSignedAngle );
			result.leftRay.stable = result.leftRay.verticalSlopeAngle <= maxSlopeAngle;
		}
		else
		{
			result.leftRay.Reset();
		}
		

		// Right result
		result.rightRay.collision = rightHitInfo.hit;
		if( rightHitInfo.hit )
		{
			result.rightRay.point = rightHitInfo.point;
			result.rightRay.distance = rightHitInfo.distance;
			result.rightRay.normal = rightHitInfo.normal;
			result.rightRay.verticalSlopeSignedAngle = Vector3.SignedAngle(characterMotor.CurrentVerticalDirection , rightHitInfo.normal , transform.forward );
			result.rightRay.verticalSlopeAngle = Mathf.Abs( result.rightRay.verticalSlopeSignedAngle );
			result.rightRay.stable = result.rightRay.verticalSlopeAngle <= maxSlopeAngle;
		}
		else
		{
			result.rightRay.Reset();
		}

		return result;

	}

			


}

}


