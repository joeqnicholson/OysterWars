using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Extras
{

public class Destroyer : MonoBehaviour
{

	public float targetTime = 5;

	void Start ()
	{
		Destroy( gameObject , targetTime );
	}
	
	
}

}
