using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Kinematic2D.Implementation
{


[CreateAssetMenu( menuName = "Kinematic 2D/Implementation/Inputs/Human Inputs" )]
public class CharacterInputData : ScriptableObject
{
		
	[Header("Inputs names")]
	public string horizontalAxisName = "Horizontal";
	public string verticalAxisName = "Vertical";
	public string jumpName = "Jump";
	public string dashName = "Dash";
	public string jetPackName = "JetPack";
}

}