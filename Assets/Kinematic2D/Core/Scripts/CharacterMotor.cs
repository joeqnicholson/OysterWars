using UnityEngine;
using System.Collections.Generic;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{

public enum CharacterMotorState
{
	NotGrounded ,
	StableGrounded ,
	UnstableGrounded
}


/// <summary>
/// The vertical direction modes availables for the character vertical alignment.
/// </summary>
public enum VerticalAlignmentMode 
{
	World , 
	Local ,
	Object
}

/// <summary>
/// Container of information regarding the collision process.
/// </summary>
public struct CollisionInfo
{
	// Ground Info	
	public GameObject groundObject;
	public int groundLayer;
	public Vector3 groundNormal;
	public Vector3 groundContactPoint;

	// Collision flags	
	public bool bottom;
	public bool top;	
	public bool left;
	public bool right;

	// Slope info
	public float verticalSlopeSignedAngle;
	public float verticalSlopeAngle;
	public float verticalSlopeAngleSign;

	public bool onStableGround;

	// Wall info
	public GameObject wallObject;
	public float wallSignedAngle;
	public float wallAngle;
	public float wallAngleSign;

	//Movement
	public Vector3 groundMovementDirection;

		
	/// <summary>
	/// Reset the fields values to the default.
	/// </summary>
	public void Reset()
	{
		bottom = false;
		top = false;
		left = false;
		right = false;

		verticalSlopeSignedAngle = 0;
		verticalSlopeAngle = 0;
		verticalSlopeAngleSign = 1;

		onStableGround = false;

		groundObject = null;
		groundLayer = 0;
		groundNormal = Vector3.up;
		groundContactPoint = Vector3.zero;
		groundMovementDirection = Vector3.right;

		wallObject = null;
		wallSignedAngle = 0;
		wallAngle = 0;
		wallAngleSign = 1;		

	}

	public override string ToString()
	{		
		string output = string.Concat( 
			"Debug : \n\n"  , 
			"Ground Info : \n" ,
			"Bottom = "  + bottom.ToString() + '\n' ,
			"Top = " + top.ToString() + '\n' ,
			"Left = " + left.ToString() + '\n' ,
			"Right = " + right.ToString() + "\n" , 
			"Slopes Info : \n" ,
			"VerticalSlopeSignedAngle = " + verticalSlopeSignedAngle.ToString() + '\n',
			"VerticalSlopeAngle = " + verticalSlopeAngle.ToString() + '\n' ,
			"VerticalSlopeAngleSign = " + verticalSlopeAngleSign.ToString() + '\n' ,
			"OnStableGround = " + onStableGround.ToString() + "\n\n" ,
			"Ground Info : \n" ,
			"GroundObject = " + ( groundObject != null ? groundObject.name : " ----- " ) + '\n',
			"GroundLayer = " + LayerMask.LayerToName(groundLayer) + '\n' ,
			"GroundNormal = " + groundNormal.ToString() + '\n' ,
			"GroundContactPoint = " + groundContactPoint.ToString() + "\n\n" ,
			"Wall Info : \n" ,
			"WallObject = " + ( wallObject != null ? wallObject.name : " ----- " ) + '\n' ,
			"WallSignedAngle = " + wallSignedAngle.ToString() + '\n' ,
			"WallAngle = " + wallAngle.ToString() + '\n' ,
			"WallAngleSign = " + wallAngleSign.ToString() + '\n'
		);

		return output;
	}
	
}



[AddComponentMenu("Kinematic2D/Core/Character Motor")]
public class CharacterMotor : MonoBehaviour
{	

	#region Events	
	
	/// <summary>
	/// This event is called when the character became grounded.
	/// </summary>
	public event System.Action OnGroundCollision;

	/// <summary>
	/// This event is called when the character hits something with its head.
	/// </summary>
	public event System.Action OnHeadCollision;

	/// <summary>
	/// This event is called when the character hits something to the right.
	/// </summary>
	public event System.Action OnRightCollision;

	/// <summary>
	/// This event is called when the character hits something to the left.
	/// </summary>
	public event System.Action OnLeftCollision;

	/// <summary>
	/// This event is called when the character is crushed by an external platform (only when "collision detection" is enabled).
	/// </summary>
	public event System.Action OnCrushedCollision;

	/// <summary>
	/// Same as OnRightCollision, but when the character is also grounded.
	/// </summary>
	public event System.Action OnGroundedRightCollision;	

	/// <summary>
	/// Same as OnLeftCollision, but when the character is also grounded.
	/// </summary>
	public event System.Action OnGroundedLeftCollision;

	/// <summary>
	/// Same as OnRightCollision, but when the character is also not grounded.
	/// </summary>
	public event System.Action OnNotGroundedRightCollision;

	/// <summary>
	/// Same as OnLeftCollision, but when the character is also not grounded.
	/// </summary>
	public event System.Action OnNotGroundedLeftCollision;
	
	#endregion
	
	public CharacterBody CharacterBody{ get; private set; }

	protected CharacterCollisions characterCollisions = null;
	public CharacterCollisions CharacterCollisions
	{
		get
		{
			return characterCollisions;
		}
	}
	

	#region Settings

	[Space(10)]
	

	[CustomClassDrawer]
	public DebugSettings debugSettings = new DebugSettings();
	
	[CustomClassDrawer]
	public LayerMaskSettings layerMaskSettings = new LayerMaskSettings();

	[CustomClassDrawer]
	public GroundSettings groundSettings = new GroundSettings();

	[CustomClassDrawer]
	public VerticalAlignmentSettings verticalAlignmentSettings = new VerticalAlignmentSettings();

	[CustomClassDrawer]
	public GroundAlignmentSettings groundAlignmentSettings = new GroundAlignmentSettings();
	
	[CustomClassDrawer]
	public DynamicGroundSettings dynamicGroundSettings = new DynamicGroundSettings();

	[CustomClassDrawer]
	public VelocitySettings velocitySettings = new VelocitySettings();

	[CustomClassDrawer]
	public DepenetrationSettings depenetrationSettings = new DepenetrationSettings();

	#endregion

	//-----------------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Returns the current character motor state. This enum variable contains the information about the grounded and stable state, all in one.
	/// </summary>
	public CharacterMotorState CurrentState
	{
		get
		{
			if( IsGrounded )
				return IsStable ? CharacterMotorState.StableGrounded : CharacterMotorState.UnstableGrounded;
			else			
				return CharacterMotorState.NotGrounded;
		}
	}

	/// <summary>
	/// Returns the character motor state from the previous frame.
	/// </summary>
	public CharacterMotorState PreviousState
	{
		get
		{
			if( WasGrounded )
				return WasStable ? CharacterMotorState.StableGrounded : CharacterMotorState.UnstableGrounded;
			else			
				return CharacterMotorState.NotGrounded;
		}
	}
	
	Vector3 currentVerticalDirection = Vector3.up;
	public Vector3 CurrentVerticalDirection
	{ 
		get
		{ 
			return currentVerticalDirection; 
		} 
	}

	
	public bool PreviousForceNotGroundedFlag { get ; private set; }

	DynamicGroundInfo dynamicGroundInfo;

	public GameObject CollidedTrigger
	{ 
		get
		{ 
			return PhysicsComponent.Triggers.Count != 0 ? PhysicsComponent.Triggers[0].gameObject : null; 
		} 
	}

	public string collidedTriggerTag
	{ 
		get
		{ 
			return CollidedTrigger != null ? CollidedTrigger.tag : null;
		} 
	}


	Vector2 velocity = Vector2.zero;

	/// <summary>
	/// Gets the character local velocity.
	/// </summary>
	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
	}
	
	/// <summary>
	/// Gets the PhysicsComponent associated with this character.
	/// </summary>
	public PhysicsComponent PhysicsComponent
	{
		get
		{
			return characterCollisions.PhysicsComponent;
		}
	}

	// ------------------------------------------------------------------------------------------------------------------
	// COLLISION INFO ---------------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------------------------


	CollisionInfo collisionInfo = new CollisionInfo();
	
	public override string ToString()
	{
		return string.Concat( 
			collisionInfo.ToString() , 
			"\n\nCollided Trigger : " ,			
			PhysicsComponent.Triggers.Count != 0 ? PhysicsComponent.Triggers[0].gameObject.name : " --- " ,
			"\n\nis Facing : " , 
			IsFacingRight ? "Right" : "Left"
		); 
		 
	}

	
	
	// Collision flags ------------------------------------------------------------------------------
	
	/// <summary>
	/// Gets a struct that contains all the information related the current collision events.
	/// </summary>
	public CollisionInfo CollisionInfo
	{
		get
		{
			return collisionInfo;
		}
	}

	/// <summary>
	/// Gets the "bottom" flag from the collision info struct.
	/// </summary>
	public bool IsGrounded 
	{ 
		get
		{ 
			return collisionInfo.bottom; 
		} 
	}
	
	
	/// <summary>
	/// Gets the "top" flag from the collision info struct.
	/// </summary>
	public bool IsHead 
	{ 
		get 
		{ 
			return collisionInfo.top; 
		} 
	} 

	/// <summary>
	/// Gets the "left" flag from the collision info struct.
	/// </summary>
	public bool IsAgainstLeftWall 
	{ 
		get 
		{
			return collisionInfo.left; 
		} 
	} 

	/// <summary>
	/// Gets the "right" flag from the collision info struct.
	/// </summary>
	public bool IsAgainstRightWall 
	{ 
		get 
		{ 
			return collisionInfo.right;  
		} 
	}
	

	// Wall Info ----------------------------------------------------------------------------------------------------

	/// <summary>
	/// Gets the "wallObject" GameObject from the collision info struct.
	/// </summary>
	public GameObject WallObject 
	{ 
		get
		{  
			return collisionInfo.wallObject; 
		} 
	}

	/// <summary>
	/// Gets the "wallSignedAngle" value from the collision info struct.
	/// </summary>
	public float WallSignedAngle 
	{ 
		get
		{  
			return collisionInfo.wallSignedAngle; 
		} 
	}

	/// <summary>
	/// Gets the "wallAngle" value from the collision info struct.
	/// </summary>
	public float WallAngle 
	{ 
		get
		{  
			return collisionInfo.wallAngle; 
		} 
	}

	/// <summary>
	/// Gets the "wallAngleSign" value from the collision info struct.
	/// </summary>
	public float WallAngleSign 
	{ 
		get
		{  
			return collisionInfo.wallAngleSign; 
		} 
	}


	// Ground Info ---------------------------------------------------------------------------------------------------

	/// <summary>
	/// Gets the "verticalSlopeSignedAngle" value from the collision info struct.
	/// </summary>
	public float VerticalSlopeSignedAngle 
	{ 
		get
		{  
			return collisionInfo.verticalSlopeSignedAngle; 
		} 
	}

	/// <summary>
	/// Gets the "verticalSlopeSignedAngle" value from the collision info struct.
	/// </summary>
	public float VerticalSlopeAngle 
	{ 
		get
		{  
			return collisionInfo.verticalSlopeAngle; 
		} 
	}

	/// <summary>
	/// Gets the "verticalSlopeSignedAngle" value from the collision info struct.
	/// </summary>
	public float VerticalSlopeAngleSign 
	{ 
		get
		{  
			return collisionInfo.verticalSlopeAngleSign; 
		} 
	}
	
	/// <summary>
	/// This property is true if the verticalSlopeSignedAngle > 0.
	/// </summary>
	public bool IsOnRightVerticalSlope 
	{ 
		get
		{  
			return collisionInfo.verticalSlopeSignedAngle > 0; 
		} 
	}

	/// <summary>
	/// This property is true if the verticalSlopeSignedAngle < 0.
	/// </summary>
	public bool IsOnLeftVerticalSlope 
	{ 
		get
		{  
			return collisionInfo.verticalSlopeSignedAngle < 0; 
		} 
	}

	/// <summary>
	/// Gets the "onStableGround" value from the collision info struct.
	/// </summary>
	public bool IsStable
	{ 
		get
		{ 
			return collisionInfo.onStableGround;			
		}
	}

	/// <summary>
	/// Gets the "groundMovementDirection" direction from the collision info struct.
	/// </summary>
	public Vector3 GroundMovementDirection 
	{ 
		get
		{  
			return collisionInfo.groundMovementDirection; 
		} 
	}

	/// <summary>
	/// Gets the "groundContactPoint" position from the collision info struct.
	/// </summary>
	public Vector3 GroundContactPoint 
	{ 
		get
		{  
			return collisionInfo.groundContactPoint; 
		} 
	}

	/// <summary>
	/// Gets the "groundLayer" layer value from the collision info struct.
	/// </summary>
	public int GroundLayer 
	{ 
		get
		{  
			return collisionInfo.groundLayer; 
		} 
	}

	/// <summary>
	/// Gets the "groundNormal" vector from the collision info struct.
	/// </summary>
	public Vector3 GroundNormal 
	{ 
		get
		{  
			return collisionInfo.groundNormal; 
		} 
	}

	/// <summary>
	/// Gets the "groundObject" GameObject from the collision info struct.
	/// </summary>
	public GameObject GroundObject 
	{ 
		get
		{  
			return collisionInfo.groundObject; 
		} 
	}
	
	
	// ------------------------------------------------------------------------------------------------------------------
	// ------------------------------------------------------------------------------------------------------------------
	
	
	/// <summary>
    /// Gets the RigidbodyComponent component associated with the character.
    /// </summary>
	public RigidbodyComponent RigidbodyComponent
	{
		get
		{
			return CharacterBody.RigidbodyComponent;
		}
	}
		
	

	public bool IsOnDynamicGround
	{
		get
		{
			return dynamicGroundInfo.IsActive;
		}
	}


	HitInfo hitInfo = new HitInfo(); 
	GroundAlignmentResult groundAlignInfo = new GroundAlignmentResult(); 

	bool forceNotGroundedStateFlag = false;
	

	protected virtual void Awake()
	{				
		CharacterBody = transform.root.GetComponentInChildren<CharacterBody>();
		characterCollisions = gameObject.GetOrAddComponent<CharacterCollisions>();	
		
		if( layerMaskSettings.profile == null )
		{
			Debug.Log("Missing layerMask settings profile!");
			this.enabled = false;
		}		
		
	}

	protected virtual void Start()
	{
		collisionInfo.groundMovementDirection = transform.right;

		CalculateVerticalDirection();
		AlignCharacterTowardsUp();
	}

	protected virtual void OnEnable()
	{		
		OnTeleport += OnTeleportMethod;

		if( SceneController.Instance == null )
		{
			Debug.Log("Missing scene controller. Add a scene controller into the scene.\nTIP: Kinematic 2D includes a \"Scene Controller\" prefab.");
			return;
		}
		
		// Add this actor to the scene controller list
		SceneController.Instance.AddActor( this );

		
	}

	protected virtual void OnDisable()
	{
		OnTeleport -= OnTeleportMethod;

		if( SceneController.Instance == null )
			return;

		// Remove this actor from the scene controller list
		SceneController.Instance.RemoveActor( this );

		
	}

	void OnTeleportMethod( Vector3 position , Quaternion rotation )
	{
		// calculate the world direction
		if( verticalAlignmentSettings.mode == VerticalAlignmentMode.World )
			verticalAlignmentSettings.worldVerticalDirection = rotation * Vector3.up;
	}	
	
	// Vertical Direction --------------------------------------------------------------------------------------
	
		
	public void SetVerticalDirection( Vector2 direction )
	{
		currentVerticalDirection = direction;
	}

	void CalculateVerticalDirection()
	{
		switch ( verticalAlignmentSettings.mode )
		{
			case VerticalAlignmentMode.World:
				currentVerticalDirection = verticalAlignmentSettings.worldVerticalDirection.normalized;
		    	break;

			case VerticalAlignmentMode.Local:
				currentVerticalDirection = Up;
		    	break;

			case VerticalAlignmentMode.Object:

				if( verticalAlignmentSettings.verticalReferenceObject == null )
				{
					Debug.Log("Missing vertical reference object!");
					break;
				}

				float verticalSign = verticalAlignmentSettings.towardsTheReference ? -1 : 1;

				Vector3 position = RigidbodyComponent != null ? Position : transform.position;
				currentVerticalDirection = verticalSign * ( position - verticalAlignmentSettings.verticalReferenceObject.position ).normalized;
				
			break;
		}

		currentVerticalDirection.z = 0;
	}

	//-------------------------------------------------

	/// <summary>
	/// This event is called everytime the character teleports.
	/// 
	/// The teleported position and rotation are passed as arguments.
	/// </summary>
	public event System.Action<Vector3,Quaternion> OnTeleport;


	/// <summary>
	/// Sets the teleportation position and rotation using an external Transform reference. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Transform reference )
	{
		RigidbodyComponent.Position = reference.position;
		RigidbodyComponent.Rotation = reference.rotation;


		if( OnTeleport != null )
			OnTeleport( RigidbodyComponent.Position , RigidbodyComponent.Rotation );
	}	

	/// <summary>
	/// Sets the teleportation position and rotation. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position , Quaternion rotation )
	{
		RigidbodyComponent.Position = position;
		RigidbodyComponent.Rotation = rotation;

		if( OnTeleport != null )
			OnTeleport( RigidbodyComponent.Position , RigidbodyComponent.Rotation );
	}	

	/// <summary>
	/// Sets the teleportation position. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position )
	{
		RigidbodyComponent.Position = position;

		if( OnTeleport != null )
			OnTeleport( RigidbodyComponent.Position , Rotation );

	}	


	// Velocity --------------------------------------------------------------------------------------------------

	/// <summary>
	/// Sets the velocity vector.
	/// </summary>
	public void SetVelocity( Vector2 velocity , Space space = Space.Self )
	{		 
		if( space == Space.Self )
		{
			this.velocity = velocity;
		}
		else
		{
			this.velocity = transform.InverseTransformDirection( velocity );
		}
	}	

	/// <summary>
	/// Sets the x component of the velocity vector.
	/// </summary>
	public void SetVelocityX( float value , Space space = Space.Self )
	{
		velocity.x = value;
	}
	
	/// <summary>
	/// Sets the y component of the velocity vector.
	/// </summary>
	public void SetVelocityY( float value , Space space = Space.Self )
	{
		velocity.y = value;
	}

	/// <summary>
	/// Adds a value to the x component of the velocity vector.
	/// </summary>
	public void AddVelocityX( float value , Space space = Space.Self )
	{
		velocity.x += value;		
	}

	/// <summary>
	/// Adds a value to the y component of the velocity vector.
	/// </summary>
	public void AddVelocityY( float value , Space space = Space.Self )
	{
		velocity.y += value;		
	}

	/// <summary>
	/// Adds a vector to the velocity vector.
	/// </summary>
	public void AddVelocity( Vector2 velocity , Space space = Space.Self )
	{
		if( space == Space.Self )
		{
			this.velocity += velocity;
		}
		else
		{
			this.velocity += (Vector2)transform.InverseTransformDirection( velocity );
		}
	}

	/// <summary>
	/// Resets the velocity vector to Zero.
	/// </summary>
	public void ResetVelocity()
	{
		velocity = Vector2.zero;
	}

	/// <summary>
	/// Gets the previous displacement calculated.
	/// </summary>
	public Vector2 PreviousDisplacement { get; private set; }
	
	/// <summary>
	/// Gets the previous displacement calculated (local coordinates).
	/// </summary>
	public Vector2 PreviousLocalDisplacement
	{
		get
		{
			return transform.InverseTransformVectorUnscaled( PreviousDisplacement );
		}
	}


	public Vector3 Right
	{
		get
		{
			return CharacterBody.RigidbodyComponent.Rotation * Vector2.right;
		}
	}

	public Vector3 Up
	{
		get
		{			
			return CharacterBody.RigidbodyComponent.Rotation * Vector2.up;
		}
	}

	public Vector3 Position
	{
		get
		{
			return CharacterBody.RigidbodyComponent.Position;
		}
		set
		{
			CharacterBody.RigidbodyComponent.Position = value;
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return CharacterBody.RigidbodyComponent.Rotation;
		}
		set
		{
			CharacterBody.RigidbodyComponent.Rotation = value;
		}
	}
	
	// Ground Alignment -------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Aligns the character towards the up reference vector.
	/// </summary>
	public void AlignCharacterTowardsUp()
	{
		// if( OverrideRotation )
		// 	return;
		
		CalculateVerticalDirection();

		Vector3 verticalDirection = verticalAlignmentSettings.mode == VerticalAlignmentMode.Local ? 
		Up : currentVerticalDirection;
		Quaternion targetRotation = Quaternion.LookRotation( Vector3.forward , verticalDirection );

		Rotation = targetRotation;
	}

	/// <summary>
	/// Sets the forceNotGroundedStateFlag to true. 
	/// The CharacterMotor will internally force the character to the not grouned state 
	/// (after the dynamic ground movement has been processed).
	/// </summary>
	public void ForceNotGroundedState()
	{
		forceNotGroundedStateFlag = true;		
	}


	/// <summary>
	/// Sets the grounded state to false (not grounded).
	/// </summary>
	void InternalForceNotGroundedState()
	{
		collisionInfo.bottom = false;
		dynamicGroundInfo.Reset();

		if( ( verticalAlignmentSettings.mode != VerticalAlignmentMode.Local ) && groundAlignmentSettings.isEnabled )
		{
			SlopeAlignReverse();		
		}

		collisionInfo.Reset();	// must be reset after the "SlopeAlignReverse" Method
		
	}


	/// <summary>
	/// Limits the minumun horizontal velocity to the "minimumMovement" amount.
	/// </summary>
	void ClampDisplacement( ref Vector3 deltaPosition )
	{		
		if( Mathf.Abs( deltaPosition.x ) < CharacterConstants.MinimumMovement )
			deltaPosition.x = 0;		

		if( Mathf.Abs( deltaPosition.y ) < CharacterConstants.MinimumMovement )
			deltaPosition.y = 0;
	}


	protected virtual void UpdateBehaviour( float dt ){}

	/// <summary>
	/// The time elapsed since grounded.
	/// </summary>
	public float GroundedTime{ get; private set; }    

	/// <summary>
	/// The time elapsed since not grounded.
	/// </summary>
	public float NotGroundedTime{ get; private set; }

	/// <summary>
	/// Gets the previous grounded state.
	/// </summary>
    public bool WasGrounded { get; private set; }	
	
	/// <summary>
	/// Gets the previous stability state.
	/// </summary>
    public bool WasStable { get; private set; }

	public void UpdateCharacter( float dt )
	{
		if( !this.enabled )
			return;
		
		// Movement logic
		UpdateBehaviour( dt );


		WasGrounded = IsGrounded;
		WasStable = IsStable;
		PreviousForceNotGroundedFlag = forceNotGroundedStateFlag;
		
		
		if( forceNotGroundedStateFlag )
		{
			InternalForceNotGroundedState();
			forceNotGroundedStateFlag = false;
		}


		// Overlaps -------------------------------------------
		if( depenetrationSettings.isEnabled )
			Depenetrate();	


		// initialPosition need to be calculated after InternalForceNotGroundedState in order to avoid SlopeAlignReverse.
		Vector3 initialPosition = Position;
		Vector3 deltaPosition = velocity * dt;

		Move( deltaPosition , IsGrounded , true , dt );
		
		PreviousDisplacement = Position - initialPosition;

		if( IsGrounded )
		{
			NotGroundedTime = 0f;
			GroundedTime += dt;			
		}
		else
		{
			NotGroundedTime += dt;
			GroundedTime = 0f;
		}


		PhysicsComponent.ClearContacts();
		
	}

	

	
	public void Move( Vector3 deltaPosition , bool groundedMovement , bool updateData , float dt )
	{
		if( groundSettings.alwaysNotGrounded )
			InternalForceNotGroundedState();
		

		if( groundedMovement )
		{			
			if( dynamicGroundSettings.isEnabled )
				ProcessDynamicGround( dt );	

			if( !IsGrounded	)
				return;	
			
			GroundMovement( deltaPosition , dt );			
		}
		else
		{
			AlignCharacterTowardsUp();
			
			NotGroundedMovement( deltaPosition , updateData );		
		}		
		
		if( dynamicGroundSettings.isEnabled )
			UpdateDynamicGround();


		CalculateVerticalDirection();
		
		
		
	}

	
	const float MaxOneWayPlatformHorizontalAngle = 60f;

	/// <summary>
	/// It performs the collision detection and movement of the character if the grounded state is false (not grounded state).
	/// </summary>
	void NotGroundedMovement( Vector3 deltaPosition , bool updateData )
	{	
				
		
		// LayerMask positiveHorizontalLayermask = layerMaskSettings.profile.obstacles;
		LayerMask negativeHorizontalLayermask = layerMaskSettings.profile.obstacles | 
		layerMaskSettings.profile.oneWayPlatforms;

		// LayerMask horizontalMask = velocity.y > 0f ? positiveHorizontalLayermask : negativeHorizontalLayermask;
		
		hitInfo = characterCollisions.HorizontalNotGroundedCollision(
			Position , 			
			deltaPosition.x ,
			negativeHorizontalLayermask 
		);
				 
		
		if( hitInfo.hit && hitInfo.distance != 0f )
		{	
			float slopeAngle = Vector3.Angle( Up , hitInfo.normal );
			if( slopeAngle > MaxOneWayPlatformHorizontalAngle && CustomUtilities.BelongsToLayerMask( hitInfo.gameObject.layer , layerMaskSettings.profile.oneWayPlatforms ) )
			{				
				Position += Right * deltaPosition.x;
			}
			else
			{
				float distance = hitInfo.distance - CharacterConstants.SkinWidth;
				Position += Mathf.Sign(deltaPosition.x) * Right * distance;
			}
			
		}
		else
		{
			Position +=  Right * deltaPosition.x;
		}

		if( updateData )
			FillHorizontalCollisionData( false );
		
		
		//Vertical
		LayerMask positiveVerticalLayermask = layerMaskSettings.profile.obstacles;
		LayerMask negativeVerticalLayermask = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

		LayerMask verticalMask = velocity.y > 0f ? positiveVerticalLayermask : negativeVerticalLayermask;

				
		hitInfo = characterCollisions.VerticalNotGroundedCollision(
			Position , 
			deltaPosition.y , 
			verticalMask 
		);
		
		
		if( hitInfo.hit && hitInfo.distance != 0f )
		{								
			
			float distance = hitInfo.distance - CharacterConstants.SkinWidth;
			Position += Mathf.Sign(deltaPosition.y) * Up * distance;

			if( deltaPosition.y < 0)
			{
				if(OnGroundCollision != null)
					OnGroundCollision();
				
				CalculateVerticalDirection();

				float verticalSlopeSignedAngle = Vector3.SignedAngle(currentVerticalDirection, hitInfo.normal , transform.forward );
				float verticalSlopeAngle = Mathf.Abs( verticalSlopeSignedAngle );
				bool isStableOnGround = verticalSlopeAngle <= groundSettings.maxSlopeAngle;

				if( !groundSettings.alwaysNotGrounded )
				{
					bool grounded = Utilities.CustomUtilities.BelongsToLayerMask( hitInfo.gameObject.layer , verticalMask );
										
					if( updateData )
						FillGroundData( grounded , isStableOnGround );

				}
				

			}
			else if (deltaPosition.y > 0)
			{
				collisionInfo.top = true;

				if(OnHeadCollision != null)
					OnHeadCollision();

				
			}

		}
		else
		{	
			Position += deltaPosition.y * Up;

			if( updateData )
			{
				collisionInfo.top = false;
				collisionInfo.bottom = false;
			}
		}


		
	}


	
	/// <summary>
	/// It performs the collision detection and movement of the character if the grounded state is true (grounded state).
	/// </summary>
	void GroundMovement( Vector3 deltaPosition , float dt )
	{	
		bool wasStable = collisionInfo.onStableGround;

		if( groundAlignmentSettings.isEnabled )
		{
			if( collisionInfo.onStableGround )
				StableGroundAlignmentMovement( deltaPosition );
			else
				UnstableGroundAlignmentMovement( deltaPosition , dt );
		}
		else
		{
			
			if( collisionInfo.onStableGround )
				StableGroundMovement( deltaPosition );
			else
				UnstableGroundMovement( deltaPosition , dt );
		}				
		
		
	}


	/// <summary>
	/// It handles the movement of the character when it is stable on the ground.
	/// </summary>	
	void MoveHorizontallyOnGround( Vector3 deltaPosition )
	{	
		// if( deltaPosition.x == 0f )
		// 	return;

		// float inputXMagnitud = Mathf.Abs( deltaPosition.x );
		// float inputXSign = Mathf.Sign( deltaPosition.x );

		Vector3 displacement = collisionInfo.groundMovementDirection * deltaPosition.x;
				
		hitInfo = characterCollisions.HorizontalGroundedCollision(
			Position , 
			displacement ,
			layerMaskSettings.profile.obstacles
		);
		
		if( hitInfo.hit )
		{
			float distance = hitInfo.distance - CharacterConstants.SkinWidth;
			Position += Mathf.Sign( deltaPosition.x ) * collisionInfo.groundMovementDirection * distance;

		}
		else
		{
			Position += deltaPosition.x * collisionInfo.groundMovementDirection;
		}
		

		FillHorizontalCollisionData( true );
		
	}


	/// <summary>
	/// Depenetrates the character from moving/rotating colliders. Only horizontal depenetration is performed if the character is grounded, for not groundeded state 
	/// vertical depenetration is added to the process.
	/// </summary>
	void Depenetrate()
	{			
		
		// Horizontal Depenetration ----------------------------------------------------------------------------------------------------
		float horizontalCastDistance = CharacterBody.width - CharacterConstants.DepenetrationBoxThickness - CharacterConstants.SkinWidth;
				
		// Right		
		hitInfo = characterCollisions.HorizontalDepenetrationCollision( 
			Position , 
			IsGrounded , 
			true ,
			layerMaskSettings.profile.obstacles
		);

		if(hitInfo.hit)
		{
			if( hitInfo.distance != 0f )
			{				
				float distance = horizontalCastDistance - hitInfo.distance;
				Position -= Right * distance;
			}
		}
		

		// Left				
		hitInfo = characterCollisions.HorizontalDepenetrationCollision( 
			Position , 
			IsGrounded ,
			false ,
			layerMaskSettings.profile.obstacles
		);

		if(hitInfo.hit)
		{
			if( hitInfo.distance != 0f )
			{
				float distance = horizontalCastDistance - hitInfo.distance;
				Position += Right * distance;
			}			
		}

		// Vertical Depenetration ----------------------------------------------------------------------------------------------------
		float verticalCastDistance = CharacterBody.height - CharacterConstants.DepenetrationBoxThickness - CharacterConstants.SkinWidth;
		
		if( !IsGrounded )
		{
			
			
			hitInfo = characterCollisions.VerticalDepenetrationCollision( Position , true , layerMaskSettings.profile.obstacles );

			if( hitInfo.hit )
			{		
				if( hitInfo.distance != 0f )
				{
					float distance = verticalCastDistance - hitInfo.distance;	
					Position -= Up * distance;
				}
			}

			
			hitInfo = characterCollisions.VerticalDepenetrationCollision( Position , false , layerMaskSettings.profile.obstacles );
			
			if( hitInfo.hit )
			{			
				if( hitInfo.distance != 0f )
				{
					float distance = verticalCastDistance - hitInfo.distance;	
					Position += Up * distance;
				}
			}

			
		}

	}	

		
	/// <summary>
	/// It Performs a collision test towards the ground, updating the grounded horizontal direction vector and setting up the position of the character (based on the ground clamping distance).
	/// </summary>
	void ProbeGround( bool stableGroundProbing , bool depenetrateFromSlope )
	{
		LayerMask groundedLayerMask = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;

		
		hitInfo = characterCollisions.ProbeGroundCollision(
			Position , 
			groundSettings.groundClampingDistance ,
			groundedLayerMask
		);
		
		if( hitInfo.hit && hitInfo.distance == 0f )
			return;
		
		StableGroundProbing( stableGroundProbing , depenetrateFromSlope );

		if( !stableGroundProbing )
			UnstableGroundProbing();
		
		

	}


	/// <summary>
	/// Sets the grounded state (along with the collision information) based on the result of the ProbeGround collision test.
	/// </summary>
	void FillGroundData( bool grounded , bool stable )
	{

		if( !grounded )
		{			
			

			collisionInfo.Reset();
			collisionInfo.bottom = false;			
			collisionInfo.groundMovementDirection = Right;	
			return;
		}
		
		

		collisionInfo.bottom = true;

		CalculateVerticalDirection();

        collisionInfo.verticalSlopeSignedAngle = Vector3.SignedAngle( currentVerticalDirection, hitInfo.normal , transform.forward );
		collisionInfo.verticalSlopeAngle = Mathf.Abs( collisionInfo.verticalSlopeSignedAngle );
		collisionInfo.verticalSlopeAngleSign = Mathf.Sign( collisionInfo.verticalSlopeSignedAngle );
	
		collisionInfo.groundMovementDirection = Vector3.Cross( hitInfo.normal , Vector3.forward );
		collisionInfo.groundNormal = hitInfo.normal;
		collisionInfo.groundObject = hitInfo.gameObject;
		collisionInfo.groundLayer = hitInfo.gameObject.layer;
		collisionInfo.groundContactPoint = hitInfo.point;
										
		collisionInfo.onStableGround = stable;

		if( !stable )
			ResetHorizontalData();			
		

	}

	/// <summary>
	/// Sets the grounded state (along with the collision information) based on the result of the ProbeGroundRays collision test.
	/// </summary>
	void SetGroundDataSlopeAlignment( bool grounded , bool stable , GroundDetectionRay ray )
	{

		if( !grounded )
		{			
			collisionInfo.Reset();
			collisionInfo.bottom = false;
			collisionInfo.groundMovementDirection = Right;	
			return;
		}
		
		collisionInfo.bottom = true;

		CalculateVerticalDirection();

        collisionInfo.verticalSlopeSignedAngle = Vector3.SignedAngle(currentVerticalDirection, ray.normal , transform.forward );
		collisionInfo.verticalSlopeAngle = Mathf.Abs( collisionInfo.verticalSlopeSignedAngle );
		collisionInfo.verticalSlopeAngleSign = Mathf.Sign( collisionInfo.verticalSlopeSignedAngle );
		collisionInfo.groundMovementDirection = Right;
		collisionInfo.groundNormal = ray.normal;
		collisionInfo.groundContactPoint = ray.point;
										
		collisionInfo.onStableGround = stable;

		if( !stable )
			ResetHorizontalData();			
		
		
	}

	

	/// <summary>
	/// Resets the left, right and wall collision information.
	/// </summary>
	void ResetHorizontalData()
	{
		collisionInfo.right = collisionInfo.left = false;
		collisionInfo.wallSignedAngle = 0;
		collisionInfo.wallAngle = 0;
		collisionInfo.wallAngleSign = 1;
		collisionInfo.wallObject = null;
	}

	/// <summary>
	/// Fills the collision information, based on the horizontal collision test.
	/// </summary>
	void FillHorizontalCollisionData( bool grounded )
	{		
		if(!hitInfo.hit)
		{
			ResetHorizontalData();
			return;
		}


        collisionInfo.wallSignedAngle = Vector3.SignedAngle(Up , hitInfo.normal , transform.forward );
		collisionInfo.wallAngle = Mathf.Abs( collisionInfo.wallSignedAngle );
		collisionInfo.wallAngleSign = Mathf.Sign( collisionInfo.wallSignedAngle );
		collisionInfo.wallObject = hitInfo.gameObject;

		// Against Right Wall
		if( collisionInfo.wallAngleSign == 1 )
		{
			if( !collisionInfo.right )
			{
				if(grounded)
				{
					if(OnGroundedRightCollision != null)
						OnGroundedRightCollision();
				}
				else
				{
					if(OnNotGroundedRightCollision != null)
						OnNotGroundedRightCollision();
				}

				if(OnRightCollision != null)
					OnRightCollision();
			} 

			collisionInfo.right = true;
		}
		else // Against Left Wall
		{
			if( !collisionInfo.left )
			{				
				if(grounded)
				{
					if(OnGroundedLeftCollision != null)
						OnGroundedLeftCollision();
				}else
				{
					if(OnNotGroundedLeftCollision != null)
						OnNotGroundedLeftCollision();
				}

				if(OnLeftCollision != null)
					OnLeftCollision();
			}				

			collisionInfo.left = true;
		}
	}
	
	/// <summary>
	/// Depenetrates the character from a steep slope when moving on the ground.
	/// </summary>
	/// <param name="penetration">The vertical penetration magnitude.</param>
	/// <param name="normal">The surface normal.</param>
	/// <returns>returns the correction vector used to depenetrate the character.</returns>
	Vector3 DepenetrateFromSteepSlope( float penetration , Vector3 normal )
	{	
		penetration = Mathf.Abs( penetration );	

		CalculateVerticalDirection();

		float newSlopeSignedAngle = Vector3.SignedAngle(
            currentVerticalDirection, 
			normal ,
            transform.forward 
		);

		float oldSlopeAngle = collisionInfo.verticalSlopeAngle;
		float newSlopeAngle = Mathf.Abs( newSlopeSignedAngle );
		
		float depenetrationSign = - Mathf.Sign( newSlopeSignedAngle );
		
		float alpha = newSlopeAngle - oldSlopeAngle;
		float beta = 90f - Vector3.Angle( 
			Up , 
			normal
		);

		float correctionDistance = alpha != 0f ? 
			penetration * ( Mathf.Sin( beta * Mathf.Deg2Rad) / Mathf.Sin( alpha * Mathf.Deg2Rad) ) : 0; 

		Vector3 correctionVector = depenetrationSign * correctionDistance * collisionInfo.groundMovementDirection;
		Position += correctionVector;

		return correctionVector;		

	}

	/// <summary>
	/// It performs the ground alignment collision test and stores the information in the groundAlignInfo struct.
	/// </summary>
	void GroundRaysCollisionTest()
	{
		LayerMask groundedLayerMask = layerMaskSettings.profile.obstacles | layerMaskSettings.profile.oneWayPlatforms;
		
		groundAlignInfo.Reset();
		
		groundAlignInfo = characterCollisions.GroundRaysCollisions(
			Position , 
			CharacterBody.StepOffset , 
			groundAlignmentSettings.detectionDistance ,
			groundSettings.maxSlopeAngle ,
			groundedLayerMask
		);
		
	}	


	/// <summary>
	/// Align the player to the ground slope based on the groundAlignInfo struct.
	/// </summary>
	void AlignToTheGround()
	{				
		Vector3 pivot = Vector3.zero;
		float rotationSignedAngle = 0;

		if( !IsGrounded )
			return;

		
		if( ( groundAlignInfo.leftRay.normal == groundAlignInfo.rightRay.normal ) && ( groundAlignInfo.leftRay.normal == Up ) )
			return;

		
		if( IsStable )
		{						
			if( groundAlignInfo.leftRay.verticalSlopeAngle > groundSettings.maxSlopeAngle ||
				groundAlignInfo.rightRay.verticalSlopeAngle > groundSettings.maxSlopeAngle )
			{
				AlignCharacterTowardsUp();
				return;
			}

			if( !groundAlignmentSettings.canAlignWithOneRay )
			{
				if( !groundAlignInfo.leftRay.collision || !groundAlignInfo.rightRay.collision )
				{				
					AlignCharacterTowardsUp();
					return;
				}
			}
		}
		else
		{
			if( !groundAlignInfo.leftRay.collision && !groundAlignInfo.rightRay.collision )			
				return;
			
		}

		if( groundAlignInfo.leftRay.collision && groundAlignInfo.rightRay.collision )
		{
			bool useRightPivot = groundAlignInfo.leftRay.distance > groundAlignInfo.rightRay.distance;
			Vector2 leftToRight = groundAlignInfo.rightRay.point - groundAlignInfo.leftRay.point;

			rotationSignedAngle = Vector3.SignedAngle(Right , leftToRight , transform.forward );
			pivot = useRightPivot ? CharacterBody.GetBottomRightCollision( Position ) : CharacterBody.GetBottomLeftCollision( Position );
			
		} 


		if( !groundAlignInfo.leftRay.collision && groundAlignInfo.rightRay.collision )
		{
			rotationSignedAngle = Vector3.SignedAngle(Up , groundAlignInfo.rightRay.normal , transform.forward );
			pivot = CharacterBody.GetBottomRightCollision( Position );
			
		} 
		
		if ( groundAlignInfo.leftRay.collision && !groundAlignInfo.rightRay.collision )
		{						
			rotationSignedAngle = Vector3.SignedAngle(Up , groundAlignInfo.leftRay.normal , transform.forward );
			pivot = CharacterBody.GetBottomLeftCollision( Position );
			
		}	
		
		RotateAround( pivot , Vector3.forward , rotationSignedAngle );

			
	}

	void RotateAround( Vector3 pivot, Vector3 axis, float angle )
	{		
		Vector3 position = Position;
		Quaternion rotation = Rotation;

		CustomUtilities.RotateAround( ref position , ref rotation , pivot , axis , angle , true );

		Position = position;
		Rotation = rotation;

		// Before --------------------------
		// Vector3 delta = Position - pivot;

		// Quaternion rotation = Quaternion.AngleAxis( angle , axis );
		// delta = rotation * delta;
		
		// Position = pivot + delta;
		// Rotation =  Rotation * rotation;

		
	}

	void StableGroundProbing( bool setGroundData , bool depenetrateFromSlope )
	{
		CalculateVerticalDirection();

		float verticalSlopeSignedAngle = Vector3.SignedAngle( currentVerticalDirection, hitInfo.normal , transform.forward );
		float verticalSlopeAngle = Mathf.Abs( verticalSlopeSignedAngle );
	
		
		bool isStableOnGround = verticalSlopeAngle <= groundSettings.maxSlopeAngle;
		
		float distance = hitInfo.distance - CharacterBody.StepOffset;


		if( hitInfo.hit )
		{

			if( collisionInfo.onStableGround )	// previously on stable ground
			{
				if( distance >= 0f )
				{
					if( isStableOnGround )
					{				
						Position += - Up * distance;
						if(setGroundData)
							FillGroundData( true , true );	
					}
					else
					{	
						if( verticalSlopeAngle == 90f )					
							return;


						if(setGroundData)
							FillGroundData( false , false );		
					}
				}
				else if( distance < 0f )	//a.k.a penetration
				{	
					if(isStableOnGround)
					{

						Position += - Up * distance;
						if(setGroundData)
							FillGroundData( true , true );
					}
					else
					{
						if( verticalSlopeAngle == 90f )					
							return;

						if( depenetrateFromSlope )
							DepenetrateFromSteepSlope( distance , hitInfo.normal );
					}
				}

			}
			else	// previously on unstable ground ( onStableGround = false )
			{
				if( distance >= 0f )
				{		
					if(isStableOnGround)
					{
						if(setGroundData)
							FillGroundData( true , true );
						
						Position += - Up * distance;
					}
					else
					{
						if( verticalSlopeAngle == 90f )					
							return;

						Position += - Up * distance;
						if(setGroundData)
							FillGroundData( true , false );
					}
				}
				else if( distance < 0f ) //a.k.a penetration
				{	
					
					if(isStableOnGround)
					{
						Position += - Up * distance;
						if(setGroundData)
							FillGroundData( true , true );
						
					}
					else
					{				
						if( verticalSlopeAngle == 90f )					
							return;		
						
						Position += - Up * distance;
						if(setGroundData)
							FillGroundData( true , false  );							
					}
		

				}

			}						
		}
		else
		{		
			if(setGroundData)				
				FillGroundData( false , true  );

		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// Ground Movement --------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------

	
	void StableGroundMovement( Vector3 deltaPosition )
	{
		MoveHorizontallyOnGround( deltaPosition );		
		ProbeGround( true , true );
	}

	void StableGroundAlignmentMovement( Vector3 deltaPosition )
	{		
		MoveHorizontallyOnGround( deltaPosition );
		
		// ----- 

		GroundRaysCollisionTest();

		float leftRayPenetration = groundAlignInfo.leftRay.distance - CharacterBody.StepOffset;
		float rightRayPenetration = groundAlignInfo.rightRay.distance - CharacterBody.StepOffset;

		if( groundAlignInfo.leftRay.collision && !groundAlignInfo.leftRay.stable )
		{			
			if( leftRayPenetration < 0f )
				DepenetrateFromSteepSlope( leftRayPenetration , groundAlignInfo.leftRay.normal );
		}
		else if( groundAlignInfo.rightRay.collision && !groundAlignInfo.rightRay.stable )
		{
			if( rightRayPenetration < 0f )
				DepenetrateFromSteepSlope( rightRayPenetration , groundAlignInfo.rightRay.normal );
		}
		else
		{
			AlignToTheGround();
		}

		ProbeGround( true , false );

	}

	void UnstableGroundMovement( Vector3 deltaPosition , float dt )
	{		
		MoveHorizontallyOnGround( deltaPosition );
		ProbeGround( true , true );
		
	}

	void UnstableGroundAlignmentMovement( Vector3 deltaPosition , float dt )
	{
		MoveHorizontallyOnGround( deltaPosition );
		ProbeGround( false , false );
		
		ProbeGroundRays( deltaPosition );
				
	}


	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------
	
	void UnstableGroundProbing()
	{
		
		CalculateVerticalDirection();
		
		
		float distance = hitInfo.distance - CharacterBody.StepOffset;
		Position += - Up * distance;


		
		GroundRaysCollisionTest();
		AlignToTheGround();
		
		if( IsOnRightVerticalSlope )
		{		

			collisionInfo.groundMovementDirection = Vector3.Cross( groundAlignInfo.rightRay.normal , Vector3.forward );

			collisionInfo.verticalSlopeSignedAngle = groundAlignInfo.rightRay.verticalSlopeSignedAngle;
			collisionInfo.verticalSlopeAngle = Mathf.Abs( collisionInfo.verticalSlopeSignedAngle );

			if( groundAlignInfo.rightRay.verticalSlopeAngle <= groundSettings.maxSlopeAngle  )
				collisionInfo.onStableGround = true;

		}
		else
		{			

			collisionInfo.groundMovementDirection = Vector3.Cross( groundAlignInfo.leftRay.normal , Vector3.forward );

			collisionInfo.verticalSlopeSignedAngle = groundAlignInfo.leftRay.verticalSlopeSignedAngle;
			collisionInfo.verticalSlopeAngle = Mathf.Abs( collisionInfo.verticalSlopeSignedAngle );

			if( groundAlignInfo.leftRay.verticalSlopeAngle <= groundSettings.maxSlopeAngle  )
				collisionInfo.onStableGround = true;
		}

	}

	/// <summary>
	/// Ground probing based exclusively on the ground alignment rays
	/// </summary>
	void ProbeGroundRays( Vector3 deltaPosition )
	{
		GroundRaysCollisionTest();

		GroundDetectionRay testingRay = new GroundDetectionRay();

		if( !groundAlignInfo.leftRay.collision && !groundAlignInfo.rightRay.collision )
		{
			SetGroundDataSlopeAlignment( false , true , testingRay );
			return;
		}

		if( !groundAlignInfo.leftRay.collision && groundAlignInfo.rightRay.collision)
		{			
			testingRay = groundAlignInfo.rightRay;
		}
		else if( groundAlignInfo.leftRay.collision && !groundAlignInfo.rightRay.collision)
		{
			testingRay = groundAlignInfo.leftRay;
		}
		else
		{
			testingRay = deltaPosition.x > 0f ? groundAlignInfo.leftRay : groundAlignInfo.rightRay;
		}

		CalculateVerticalDirection();
		
		float verticalSlopeSignedAngle = Vector3.SignedAngle(currentVerticalDirection, testingRay.normal , transform.forward );
		float verticalSlopeAngle = Mathf.Abs( verticalSlopeSignedAngle );
		
				
		
		bool isStableOnGround = verticalSlopeAngle <= groundSettings.maxSlopeAngle;
		
		float distance = testingRay.distance - CharacterBody.StepOffset;

		if( testingRay.collision )
		{
			if( collisionInfo.onStableGround )	// previously on stable ground
			{
				if( distance >= 0f )
				{
					if( isStableOnGround )
					{				
						Position += - Up * distance;
						SetGroundDataSlopeAlignment( true , true , testingRay );	
					}
					else
					{	
						if( verticalSlopeAngle == 90f )					
							return;

						SetGroundDataSlopeAlignment( false , false , testingRay );		
					}
				}
				else
				{	
					if(isStableOnGround)
					{
						Position += - Up * distance;
						SetGroundDataSlopeAlignment( true , true , testingRay );
					}
					else
					{
						if( verticalSlopeAngle == 90f )					
							return;

						DepenetrateFromSteepSlope( distance , hitInfo.normal );
					}
				}

			}
			else	// previously on unstable ground ( onStableGround = false )
			{
				if( distance >= 0f )
				{		
					if(isStableOnGround)
					{
						SetGroundDataSlopeAlignment( true , true , testingRay );
						Position += - Up * distance;
					}
					else
					{
						if( verticalSlopeAngle == 90f )					
							return;

						Position += - Up * distance;
						SetGroundDataSlopeAlignment( true , false , testingRay );
					}
				}
				else
				{	
					
					if(isStableOnGround)
					{
						Position += - Up * distance;
						SetGroundDataSlopeAlignment( true , true , testingRay );
						
					}
					else
					{				
						if( verticalSlopeAngle == 90f )					
							return;		
						
						Position += - Up * distance;
						SetGroundDataSlopeAlignment( true , false , testingRay );							
					}
		

				}
			}

			
		}
		else
		{						
			SetGroundDataSlopeAlignment( false , true , testingRay );

		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// Slope Alignment -------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------

	void SlopeAlignReverse()
	{		
		CalculateVerticalDirection();
		
		float signedAngle = Vector3.SignedAngle( Up , currentVerticalDirection, Vector3.forward);
		
		if( signedAngle < 0f )
		{
			RotateAround( CharacterBody.GetBottomRight( Position )  , transform.forward , signedAngle );
		}
		else
		{
			RotateAround( CharacterBody.GetBottomLeft( Position ) , transform.forward , signedAngle );
		}
		
	}

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// External movement ───────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


	public void AddExternalMovement( Vector3 externalDisplacement , Transform caller )
	{
		// If the object who is calling this method is the platform itself, then don't use it as a external movement source.
		if( dynamicGroundInfo.Transform == caller )
			return;
		
		if( externalDisplacement == Vector3.zero )
			return;
		

		bool result = ExternalMovementCollision( externalDisplacement , layerMaskSettings.profile.obstacles , true );

		// // if the displacement is upwards then reset the velocity.
		// float verticalAngle = Vector3.Angle( Up , externalDisplacement );

		// if( verticalAngle < 90f )
		// 	velocity = Vector3.zero;
	}


	bool ExternalMovementCollision( Vector3 displacement , LayerMask layerMask , bool ungroundOnVerticalCollision )
	{		
		
		bool result = false;

		// Horizontal -----------------------------------------------------------------------------------------------------				
		hitInfo = characterCollisions.HorizontalNotGroundedCollision(
			Position , 			
			displacement.x ,
			layerMask 
		);
		
		
		if( hitInfo.hit && hitInfo.distance != 0f )
		{	
			result = true;
			float distance = hitInfo.distance - CharacterConstants.SkinWidth;
			Position += Mathf.Sign( displacement.x ) * Right * distance;
		}
		else
		{
			Position +=  Right * displacement.x;
		}

		
		
		// Vertical -----------------------------------------------------------------------------------------------------	
		hitInfo = characterCollisions.VerticalNotGroundedCollision(
			Position , 
			displacement.y , 
			layerMask 
		);

		
		if( hitInfo.hit && hitInfo.distance != 0f )
		{									
			result = true;
			float distance = hitInfo.distance - CharacterConstants.SkinWidth;
			Position += Mathf.Sign( displacement.y ) * Up * distance;
			
			if( ungroundOnVerticalCollision && displacement.y > 0f && hitInfo.hit )
			{
				InternalForceNotGroundedState();
			}

			if( displacement.y < 0 && IsGrounded )
			{
				if( OnCrushedCollision != null )
					OnCrushedCollision();
			}

		}
		else
		{	
			Position += displacement.y * Up;
		}

		return result;
	}

	
	// -----------------------------------------------------------------------------------------------------------------
	// Dynamic Ground -------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------
	

	protected virtual void ProcessDynamicGround( float dt )
    {		
        if( !dynamicGroundInfo.IsActive )
			return;

		Vector3 initialUp = Up;
			
		Quaternion deltaRotation = dynamicGroundInfo.RigidbodyRotation * Quaternion.Inverse( dynamicGroundInfo.previousRotation );

		Vector3 centerToCharacter = Position - dynamicGroundInfo.previousPosition;	
		Vector3 rotatedCenterToCharacter = deltaRotation * centerToCharacter;

		Vector3 targetPosition = dynamicGroundInfo.RigidbodyPosition + rotatedCenterToCharacter;

		if( dynamicGroundInfo.KinematicPlatform.UseCollisionDetection )
			ExternalMovementCollision( targetPosition - Position , layerMaskSettings.profile.obstacles , true );
		else
			Position = targetPosition;
		
    }

	Dictionary< Transform , KinematicPlatform > kinematicPlatforms = new Dictionary< Transform , KinematicPlatform >();

	/// <summary>
	/// Checks and updates the dynamic ground information of the character.
	/// </summary>
	void UpdateDynamicGround()
	{
		if( !IsGrounded || !Utilities.CustomUtilities.BelongsToLayerMask(collisionInfo.groundLayer , layerMaskSettings.profile.dynamicGround ))
		{
            dynamicGroundInfo.Reset();
			return;
		}

		FindAndUpdateDynamicGround( collisionInfo.groundObject.transform , Position );			
		
	}

	void FindAndUpdateDynamicGround( Transform groundTransform , Vector3 footPosition )
    {
        KinematicPlatform kinematicPlatform;
		bool founded = kinematicPlatforms.TryGetValue( groundTransform , out kinematicPlatform );

		if( founded )
		{
			dynamicGroundInfo.UpdateTarget( kinematicPlatform , footPosition );
		}
		else
		{
			kinematicPlatform = GroundObject.GetComponent<KinematicPlatform>();

			if( kinematicPlatform )
			{
				kinematicPlatforms.Add( groundTransform , kinematicPlatform );
				dynamicGroundInfo.UpdateTarget( kinematicPlatform , footPosition );
			}
			else
			{
				dynamicGroundInfo.Reset();
			}
			
		}
    }    


	// --------------------------------------------------------------------------------------------------------------------------
	// Facing direction  --------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Gets/Sets the IsFacingRight property from the character. Alternatively you can use other methods like LookToTheRight, LookToTheLeft or LookToTheOppositeSide to achieve the same.
	/// </summary>
	public bool IsFacingRight { get; set; }


	/// <summary>
	/// It makes the character switch between left and right facing direction.
	/// </summary>
	public void LookToTheOppositeSide()
	{
		IsFacingRight = !IsFacingRight;		
	}

	/// <summary>
	/// It makes the character look to the right (positive horizontal direction).
	/// </summary>
	public void LookToTheRight()
	{
		IsFacingRight = true;
	}

	/// <summary>
	/// It makes the character look to the left (negative horizontal direction).
	/// </summary>
	public void LookToTheLeft()
	{
		IsFacingRight = false;
	}

		
	// --------------------------------------------------------------------------------------------------------------------------
	// Debug --------------------------------------------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR

	void OnDrawGizmos()
	{
		if( !debugSettings.drawGizmos )
			return;
		
		if( CharacterBody == null )
			CharacterBody = GetComponent<CharacterBody>();

				

		if( debugSettings.drawGroundAlignmentGizmos )
		{
			Gizmos.color = Color.green;

			Vector3 leftOrigin = transform.position - transform.right * ( CharacterBody.VerticalCollisionArea / 2f ) + 
			transform.up * CharacterBody.StepOffset;

			CustomUtilities.DrawArrowGizmo(
                leftOrigin ,
                leftOrigin - transform.up * (CharacterBody.StepOffset + groundAlignmentSettings.detectionDistance ) ,
                Color.gray ,
				0.15f 
			);

			Vector3 rightOrigin = transform.position + transform.right * ( CharacterBody.VerticalCollisionArea / 2f ) + 
			transform.up * CharacterBody.StepOffset;

			CustomUtilities.DrawArrowGizmo(
                rightOrigin ,
                rightOrigin - transform.up * (CharacterBody.StepOffset + groundAlignmentSettings.detectionDistance ) ,
                Color.gray ,
				0.15f  
			);

		}
		
		if( debugSettings.drawGroundMovementGizmos )
		{
            CustomUtilities.DrawArrowGizmo(transform.position , transform.position + 1.5f * GroundMovementDirection, Color.magenta );
		}
		
		
		if( debugSettings.drawVerticalAlignmentGizmos )
		{
			CalculateVerticalDirection();		

			if( verticalAlignmentSettings.mode == VerticalAlignmentMode.Object )
			{
				if( verticalAlignmentSettings.verticalReferenceObject != null )
				{
                        Utilities.CustomUtilities.DrawCross(verticalAlignmentSettings.verticalReferenceObject.position , 0.5f , Color.red );
                        Utilities.CustomUtilities.DrawArrowGizmo(
                        transform.position ,
                        transform.position + currentVerticalDirection * 2  ,
                        Color.white 
					);
				}
			}
			else
			{
                    Utilities.CustomUtilities.DrawArrowGizmo(
                    transform.position ,
                    transform.position + currentVerticalDirection * 2  ,
                    Color.white 
				);
			}
		}


		if( debugSettings.drawGroundClampingGizmos )
		{
			Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
			Matrix4x4 gizmoMatrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.localScale);

			Gizmos.matrix *= gizmoMatrix;

			Vector3 delta = Vector3.down * 0.5f * groundSettings.groundClampingDistance;


			Gizmos.color = new Color( 0f , 1f , 0f , 0.25f );
			Gizmos.DrawCube( 
				delta  ,
				new Vector3( CharacterBody.HorizontalCollisionArea , groundSettings.groundClampingDistance , CharacterBody.depth )
			);

			Gizmos.matrix = oldGizmosMatrix;

		}
		
	}

#endif

	


	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────






}

}

