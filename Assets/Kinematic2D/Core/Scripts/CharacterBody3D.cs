using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.Kinematic2D.Core
{

[AddComponentMenu("Kinematic2D/Core/Character Body 3D")]
public sealed class CharacterBody3D : CharacterBody
{

	public override bool Is3D
	{
		get
		{
			return true;
		}
	}
	
	BoxCollider boxCollider3D = null;
	
	protected override void Awake()
	{
		base.Awake();
		
		boxCollider3D = gameObject.GetOrAddComponent<BoxCollider>();
		boxCollider3D.size = CharacterSize;
		boxCollider3D.center = new Vector3( 0 , HeightExtents , 0 );

		
		Rigidbody rigidbody3D = gameObject.GetOrAddComponent<Rigidbody>();
		rigidbody3D.isKinematic = true;
		rigidbody3D.interpolation = RigidbodyInterpolation.Interpolate;
		
		rigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
	}

	void OnDrawGizmos()
	{
		if( !drawBodyShapeGizmo )
			return;
		
		Gizmos.color = gizmosColor;

		Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
		Matrix4x4 gizmoMatrix = Matrix4x4.TRS( transform.position, transform.rotation, transform.localScale);

		Gizmos.matrix *= gizmoMatrix;

		// Vector3 center = transform.position + transform.up * height / 2f;
		// Vector3 offsetCenter = transform.position + transform.up * ( stepOffset + horizontalArea_StepOffset / 2 );
		
		Vector3 deltaCenter = (height / 2f) * Vector3.up;
		Vector3 deltaCenterStepOffset = ( stepOffset + HorizontalCollisionArea_StepOffset / 2 ) * Vector3.up;
		
		Gizmos.DrawWireCube( deltaCenter , CharacterSize );
		Gizmos.DrawWireCube( deltaCenterStepOffset , new Vector3( VerticalCollisionArea , HorizontalCollisionArea_StepOffset , CharacterSize.z ) );
				
		Gizmos.matrix = oldGizmosMatrix;
		
	}
	

	public override void SetCharacterSize( Vector3 size )
	{
		width = size.x;
		height = size.y;
		depth = size.z;

		boxCollider3D.size = CharacterSize;
		boxCollider3D.center = new Vector3( 0 , HeightExtents , 0 );

	}


}

}
