using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Kinematic2D.Implementation
{


[CreateAssetMenu( menuName = "Kinematic 2D/Implementation/Movement/Movement Profile" )]
public class CharacterMovementProfile : ScriptableObject
{	

	
	public HorizontalMovementProfile horizontalMovementData;

	
	public VerticalMovementProfile verticalMovementData;

}


}
