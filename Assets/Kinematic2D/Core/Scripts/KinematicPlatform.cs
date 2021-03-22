using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{
    


/// <summary>
/// This abstract class represents a kinematic rigidbody that can be updated by setting its position and rotation in every FixedUpdate.
/// </summary>
public abstract class KinematicPlatform : MonoBehaviour
{
    [Header("Collision Detection")]

    [Tooltip("If this option is true the platform will make the character to use its own collision detection in order to move. Usually, "
    + "if the platform needs to rotate (e.g. a planet) disable this toggle for better results.")]
    [SerializeField]
    bool useCollisionDetection = false;

    [Tooltip("If \"useCollisionDetection\" is activated, this layer mask will should be the one containing all the characters (by default it should contain \"Character\").")]
    [SerializeField]
    LayerMask charactersLayerMask = 0;


    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public bool UseCollisionDetection
    { 
        get
        {
            return useCollisionDetection;
        }
    }

    const float PlatformSkinWidth = 0.08f;

    bool boxPlatform = false;

    public ColliderComponent ColliderComponent { get; private set; }

    public RigidbodyComponent RigidbodyComponent { get; private set; }

    public PhysicsComponent PhysicsComponent { get; private set; }


    /// <summary>
    /// Updates the platform position and rotation.
    /// </summary>
    protected abstract void UpdateBehaviour( ref Vector3 position , ref Quaternion rotation , float dt );


    protected virtual void Awake()
    {
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();        
        BoxCollider boxCollider3D = GetComponent<BoxCollider>();

        boxPlatform = boxCollider2D != null || boxCollider3D != null;

        if( boxCollider2D != null )
        {
            ColliderComponent = gameObject.AddComponent<BoxColliderComponent2D>();
            RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent2D>();
            PhysicsComponent = gameObject.AddComponent<PhysicsComponent2D>();

        }
        else if( boxCollider3D != null )
        {
            ColliderComponent = gameObject.AddComponent<BoxColliderComponent3D>();   
            RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
            PhysicsComponent = gameObject.AddComponent<PhysicsComponent3D>();
        }  
        else
        {                
            if( GetComponent<Collider2D>() != null )
            {
                RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent2D>();
            }
            else if( GetComponent<Collider>() != null )
            {
                RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
            }
            else
            {
                this.enabled = false;
                return;
            }
        }

        
        RigidbodyComponent.IsKinematic = true;
        RigidbodyComponent.UseInterpolation = true;
    }

    protected virtual void OnEnable()
	{		
		// Add this actor to the scene controller list
		SceneController.Instance.AddActor( this );
	}

	protected virtual void OnDisable()
	{
		// Remove this actor from the scene controller list
		SceneController.Instance.RemoveActor( this );
	}
    
    

    public void UpdatePlatform( float dt )
    {
        
        Vector3 position = RigidbodyComponent.Position;
        Quaternion rotation = RigidbodyComponent.Rotation;

        Vector3 initialPosition = position;

        UpdateBehaviour( ref position , ref rotation , dt );

        
        if( UseCollisionDetection )
        {
            Vector3 platformDisplacement = position - initialPosition;

            platformDisplacement.z = 0f;
            
            Vector3 size = new Vector3( 
                transform.localScale.x * ColliderComponent.Size.x , 
                transform.localScale.y * ColliderComponent.Size.y , 
                transform.localScale.z * ColliderComponent.Size.z 
            );
    

            Vector3 castDisplacement = platformDisplacement.normalized * ( platformDisplacement.magnitude + PlatformSkinWidth );
            Vector3 castSize = size - Vector3.one * PlatformSkinWidth;
            
            HitInfo hitInfo;
            PhysicsComponent.BoxCast(
                out hitInfo ,
                initialPosition ,
                castSize , 
                RigidbodyComponent.Up ,
                castDisplacement ,
                charactersLayerMask
            );

            if( hitInfo.hit )
            {
                CharacterMotor characterMotor = hitInfo.transform.GetComponent<CharacterMotor>();

                if( characterMotor != null )
                {
                    Vector3 characterDisplacement = castDisplacement.normalized * ( castDisplacement.magnitude - hitInfo.distance );
                    characterMotor.AddExternalMovement( characterDisplacement , transform );
                }
            }

        }
        
        RigidbodyComponent.SetPositionAndRotation( position ,  rotation );   

    }

    
    
}

}
