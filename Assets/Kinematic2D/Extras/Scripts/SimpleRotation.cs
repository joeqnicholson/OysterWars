using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Extras
{
public class SimpleRotation : MonoBehaviour {

	[SerializeField]
	float angularSpeed = 20f;
	
	
	
	void FixedUpdate ()
	{

		transform.Rotate( 0 , 0 , angularSpeed * Time.deltaTime );
	}
}

}
