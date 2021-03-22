using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{



/// <summary>
/// This class contains all the data regarding the character body shape.
/// </summary>
public abstract class CharacterBody : MonoBehaviour
{
	[Header ("Debug")]

	[SerializeField] 
	protected bool drawBodyShapeGizmo = true;
	
	[SerializeField] 
	protected Color gizmosColor = new Color( 0.52f , 1f , 0.24f );

	
	[Header("Body")]
	
	[Range_NoSlider(true)]
	public float width = 1;

	[Range_NoSlider(true)]
	public float height = 1;

	[Range_NoSlider(true)]
	public float depth = 1;

	
	[Tooltip("The Step Offset determine the maximum step height the character can walk. \n")]
	[Min( 0f )]
	[SerializeField]
	protected float stepOffset = 0.25f;

	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	void OnValidate()
	{
		stepOffset = Mathf.Max( stepOffset , 2f * CharacterConstants.SkinWidth );
	}

	public float StepOffset
	{ 
		get
		{ 
			return stepOffset; 
		} 
	}


	protected float initialWidth;
	public float InitialWidth
	{ 
		get
		{
			return initialWidth; 
		} 
	}

	protected float initialHeight;
	public float InitialHeight
	{ 
		get
		{ 
			return initialHeight; 
		} 
	}

	protected float initialDepth;
	public float InitialDepth
	{ 
		get
		{ 
			return initialDepth; 
		} 
	}
	
	
	CharacterMotor characterMotor = null;
	


	/// <summary>
	/// Half of the width.
	/// </summary>
	public float WidthExtents
	{ 
		get
		{ 
			return width / 2;
		} 
	}

	/// <summary>
	/// Half of the height.
	/// </summary>
	public float HeightExtents
	{ 
		get
		{ 
			return height / 2;
		} 
	}


	

	/// <summary>
	/// Size of the body < width , height , depth >.
	/// </summary>
	public Vector3 BodySize
	{ 
		get
		{ 
			return new Vector3( width , height , depth );		
		} 	
	}

	/// <summary>
	/// Effective box size of the character body shape excluding the skin width (Not Grounded).
	/// </summary>
	public Vector3 CollisionBodySize
	{ 
		get
		{ 
			return BodySize - Vector3.one * 2f * CharacterConstants.SkinWidth;		
		} 	
	}

	/// <summary>
	/// Effective box size of the character body shape excluding the skin width and the step offset area (Grounded).
	/// </summary>
	public Vector3 CollisionBodySize_StepOffset
	{ 
		get
		{ 
			return CollisionBodySize - Vector3.up * ( stepOffset - CharacterConstants.SkinWidth );
		} 	
	}


	/// <summary>
	/// The current three dimensional character size.
	/// </summary>
	public virtual Vector3 CharacterSize
	{ 
		get
		{ 
			return new Vector3( 
				width , 
				height , 
				depth 
			);
		} 
	}

	/// <summary>
	/// The initial character size (on Awake).
	/// </summary>
	public virtual Vector3 InitialSize
	{ 
		get
		{ 
			return new Vector3(
				initialWidth ,
				initialHeight ,
				initialDepth
			);
		} 
	}

	// Collision -----------------------------------------------------------------------------------------------------	
	
	/// <summary>
	/// Effective area used for vertical collision detection (Not grounded).
	/// </summary>
	public float VerticalCollisionArea
	{
		get
		{
			return CollisionBodySize.x;		
		}	
	}

	/// <summary>
	/// Effective area used for horizontal collision detection (Not grounded).
	/// </summary>
	public float HorizontalCollisionArea
	{ 
		get
		{ 
			return CollisionBodySize.y; 		
		} 
	}

	/// <summary>
	/// Effective area used for horizontal collision detection, considering the step offset (Grounded).
	/// </summary>
	public float HorizontalCollisionArea_StepOffset
	{
		get
		{ 
			return CollisionBodySize_StepOffset.y;
		}	
	}

	public Vector3 GetCenter( Vector3 footPosition )
	{ 
		
		return footPosition + characterMotor.Up * ( height / 2 );
		
	}

	public Vector3 GetCenter_StepOffset( Vector3 footPosition )
	{
		return footPosition + characterMotor.Up * ( stepOffset + HorizontalCollisionArea_StepOffset / 2 );
		
	}

	public Vector3 GetCenter_FullStepOffset( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Up * ( stepOffset + ( height - stepOffset ) / 2 );
		
	}

	public Vector3 GetBottomRight( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Right * ( width / 2);
		
	}

	public Vector3 GetBottomLeft( Vector3 footPosition )
	{ 
		return footPosition - characterMotor.Right * ( width / 2);
		
	}

	public Vector3 GetTopRight( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Up * height + characterMotor.Right * ( width / 2 );
		
	}

	public Vector3 GetTopLeft( Vector3 footPosition )
	{ 
		return  footPosition + characterMotor.Up * height - characterMotor.Right * ( width / 2 );
		 
	}

	public Vector3 GetMiddleRight( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Right * WidthExtents + characterMotor.Up * HeightExtents;
		
	}

	public Vector3 GetMiddleLeft( Vector3 footPosition )
	{ 
		return footPosition - characterMotor.Right * WidthExtents + characterMotor.Up * HeightExtents;
	}

	// Not Grounded Corners ------------------------------------------------------------------------------------------
	public Vector3 GetBottomRightCollision( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Right * (VerticalCollisionArea/2) + characterMotor.Up * CharacterConstants.SkinWidth;
		
	}

	public Vector3 GetBottomLeftCollision( Vector3 footPosition )
	{ 
		return footPosition - characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * CharacterConstants.SkinWidth;
		
	}

	public Vector3 GetTopRightCollision( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * ( height - CharacterConstants.SkinWidth);
		
	}

	public Vector3 GetTopLeftCollision( Vector3 footPosition )
	{ 
		return  footPosition - characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * ( height - CharacterConstants.SkinWidth);
		
	}

	public Vector3 GetMiddleRightCollision( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * ( height / 2 );
		
	}

	public Vector3 GetMiddleLeftCollision( Vector3 footPosition )
	{ 
		return footPosition - characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * ( height / 2 );
		
	}

	public Vector3 GetMiddleTopCollision( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Up * ( height - CharacterConstants.SkinWidth);
		 
	}

	public Vector3 GetMiddleBottomCollision( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Up * ( CharacterConstants.SkinWidth );
		
	}

	// Not Grounded Corners (StepOffset) ------------------------------------------------------------------------------------------
	public Vector3 GetBottomRightCollision_StepOffset( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * stepOffset;
		
	}

	public Vector3 GetBottomLeftCollision_StepOffset( Vector3 footPosition )
	{ 
		return footPosition - characterMotor.Right * (VerticalCollisionArea/2) + 
			characterMotor.Up * stepOffset;
		
	}
	
	public Vector3 GetMiddleBottomCollision_StepOffset( Vector3 footPosition )
	{ 
		return footPosition + characterMotor.Up * ( stepOffset );
		
	}

	//------------------------------------------------------------------------------------------------------------------
	public abstract bool Is3D{ get; }

	public abstract void SetCharacterSize( Vector3 size );

	protected RigidbodyComponent rigidbodyComponent = null;
	public RigidbodyComponent RigidbodyComponent
	{
		get
		{
			return rigidbodyComponent;
		}
	}

	

	protected virtual void Awake()
	{
		characterMotor = GetComponent<CharacterMotor>();

		initialHeight = height;
		initialWidth = width;
		initialDepth = depth;		
	}

	

}

}

	
