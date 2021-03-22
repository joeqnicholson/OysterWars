using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{

[AddComponentMenu("Kinematic2D/Core/Character Body 2D")]
public sealed class CharacterBody2D : CharacterBody
{
	const float Default2DDepth = 0.001f;

	public override bool Is3D
	{
		get
		{
			return false;
		}
	}

	BoxCollider2D boxCollider2D = null;

	protected override void Awake()
	{
		base.Awake();
		
		depth = Default2DDepth;
		
		boxCollider2D = gameObject.GetOrAddComponent<BoxCollider2D>();
		boxCollider2D.size = CharacterSize;
		boxCollider2D.offset = new Vector2( 0 , HeightExtents );

		Rigidbody2D rigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
		rigidbody2D.isKinematic = true;
		rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
		rigidbody2D.useFullKinematicContacts = true;

		rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent2D>();		
		
	}

	void OnDrawGizmos()
	{
		if( !drawBodyShapeGizmo )
			return;

		Gizmos.color = gizmosColor;
		
		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
		Matrix4x4 gizmoMatrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.localScale);

		Gizmos.matrix *= gizmoMatrix;

		Vector3 deltaCenter = Vector3.Magnitude( transform.position + transform.up * ( height / 2 ) - transform.position ) * Vector3.up;
		Vector3 deltaCenterStepOffset = Vector3.Magnitude( transform.position + transform.up * ( stepOffset + HorizontalCollisionArea_StepOffset / 2 ) - transform.position ) * Vector3.up;
		
		Gizmos.DrawWireCube( deltaCenter , new Vector3( CharacterSize.x , CharacterSize.y , 0.001f ) );
		Gizmos.DrawWireCube( deltaCenterStepOffset , new Vector3( VerticalCollisionArea , HorizontalCollisionArea_StepOffset , 0.001f ) );
				
		Gizmos.matrix = oldGizmosMatrix;
		
	}


	public override void SetCharacterSize( Vector3 size )
	{
		width = size.x;
		height = size.y;

		boxCollider2D.size = CharacterSize;
		boxCollider2D.offset = new Vector2( 0 , HeightExtents );
	}	
	

	
}

}
