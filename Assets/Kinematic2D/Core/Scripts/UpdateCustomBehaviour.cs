using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Core
{


// public enum UpdateMethod
// {
// 	Update = 0,
// 	LateUpdate ,
// 	FixedUpdate
// }

// public class UpdateCustomBehaviour : MonoBehaviour
// {

// 	[Tooltip("What method should this monobehvaiour use to update itself.")]
// 	[SerializeField] 
// 	protected UpdateMethod updateMethod;

// 	public void SetUpdateType( UpdateMethod updateMethod )
// 	{
// 		this.updateMethod = updateMethod;
// 	}

// 	protected virtual void Awake(){}
// 	protected virtual void UpdateBehaviour(float dt){}

// 	// Default Unity Messages --------------------------------------------------

// 	void Update()
// 	{
// 		if( updateMethod != UpdateMethod.Update )
// 			return;

// 		UpdateBehaviour( Time.deltaTime );
// 	}

// 	void LateUpdate()
// 	{
// 		if( updateMethod != UpdateMethod.LateUpdate )
// 			return;

// 		UpdateBehaviour( Time.deltaTime );
// 	}

// 	void FixedUpdate()
// 	{
// 		if( updateMethod != UpdateMethod.FixedUpdate )
// 			return;

// 		UpdateBehaviour( Time.fixedDeltaTime );
// 	}


// }

}
